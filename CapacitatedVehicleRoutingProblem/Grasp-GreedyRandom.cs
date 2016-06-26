using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    static partial class Grasp
    {
        /*
            procedure Greedy Randomized Construction(α, Seed)
                1 Solution ← ∅;
                2 Initialize the candidate set: C ← E;
                3 Evaluate the incremental cost c(e) for all e ∈ C;
                4 while C 6= ∅ do
                5 cMin ← min{c(e) | e ∈ C};
                6 cMax ← max{c(e) | e ∈ C};
                7 RCL ← {e ∈ C | c(e) ≤ cMin + α(cMax − cMin)};
                8 Select an element s from the RCL at random;
                9 Solution ← Solution ∪ {s};
                10 Update the candidate set C;
                11 Reevaluate the incremental costs c(e) for all e ∈ C;
                12 end;
                13 return Solution;
            end Greedy Randomized Construction.
        */

        public static VCRPSolution GreedyRandomizedSolution(double alpha, int seed)
        {         
            // Instanciate solution
            VCRPSolution solution = new VCRPSolution(VCRPInstance.n_vehicles,VCRPInstance.n_nodes);

            // Get all clients as candidates for candidatesSet; Cost(Depot -> Client)  
            List<Tuple<int,int,double>> candidatesSet = getCandidates();

            // Restricted Candidate List (RCL)
            List<Tuple<int, int, double>> RCL = new List<Tuple<int, int, double>>();

            // Instanciate random 
            Random rand = new Random();

            while (candidatesSet.Count != 0)
            {
                // Evaluate Min and Max cost of the candidatesSet
                Tuple<double, double> cMinMax = getcMinMax(candidatesSet);
                double cMin = cMinMax.Item1;
                double cMax = cMinMax.Item2;

                // Creates new RCL
                foreach (var candidate in candidatesSet)
                {
                    double currentCost = candidate.Item3;
                    if (currentCost <= cMin + alpha * (cMax - cMin))
                    {
                        RCL.Add(candidate);
                    }
                }

                // Select a random candidate from RCL ; Candidate = <Client,Vehicle,Cost>
                int chosenIndex = rand.Next(0, RCL.Count-1);
                Tuple<int, int, double> chosenCandidate = RCL.ElementAt(chosenIndex);

                // Check if it's feasible to insert the candidate in the solution
                insertSolutionElement(chosenCandidate, solution);

                // Remove chosen candidate from candidateSet  
                candidatesSet.RemoveAll(candidate => candidate.Item1 == chosenCandidate.Item1);

                // List of elements to update cost, where route is the same of the chosenCandidate 
                IEnumerable<Tuple<int, int, double>> candidatesToUpdate = candidatesSet.Where(candidate => candidate.Item2 == chosenCandidate.Item2);
                List<Tuple<int, int, double>> candidatesToInsert = new List<Tuple<int, int, double>>();
                foreach (var candidate in candidatesToUpdate)
                {
                    double newCost = VCRPInstance.weight_matrix[chosenCandidate.Item1, candidate.Item1];
                    candidatesToInsert.Add(Tuple.Create(candidate.Item1, candidate.Item2, newCost));
                }
                candidatesSet.RemoveAll(candidate => candidate.Item2 == chosenCandidate.Item2);
                candidatesSet.AddRange(candidatesToInsert);

                // Clear RCL
                RCL.Clear();
            }
            // Routes must return to depot
            addDepotToRoutes(solution);

            return solution;
        }

        // Insert new element in the current solution 
        public static void insertSolutionElement(Tuple<int, int, double> currentCandidate, VCRPSolution solution)
        {
            int client = currentCandidate.Item1;
            int route = currentCandidate.Item2;
            double cost = currentCandidate.Item3;
            solution.routes[route].Add(client);
            solution.cost += cost;        
        }

        // Evaluate Min and Max cost of the current candidatesSet
        public static Tuple<double,double> getcMinMax(List<Tuple<int,int,double>> candidatesSet)
        {
            const int INFINITY = UInt16.MaxValue;
            double cMin = INFINITY;
            double cMax = -INFINITY;
            
            foreach(var candidate in candidatesSet)
            {
                if (cMin > candidate.Item3)
                {
                    cMin = candidate.Item3;
                }
                if(cMax < candidate.Item3)
                {
                    cMax = candidate.Item3;
                }
            }
            return Tuple.Create(cMin,cMax);
        }

        // Generate the initial list of candidates to be inserted in the solution 
        public static List<Tuple<int, int, double>> getCandidates()
        {
            List<Tuple<int, int, double>> candidateSet = new List<Tuple<int, int, double>>();

            for(int i = 1; i < VCRPInstance.n_nodes; i++)
            {
                for(int k = 0; k<VCRPInstance.n_vehicles; k++)
                {
                    candidateSet.Add(Tuple.Create(i, k, VCRPInstance.weight_matrix[VCRPInstance.depot, i]));
                }
            }

            return candidateSet;
        }

        // Force routes to return to depot and add the corresponding cost
        public static void addDepotToRoutes(VCRPSolution solution)
        {
            for(int k=0; k < VCRPInstance.n_vehicles; k++)
            {
                int lastClient = solution.routes[k].Last();

                solution.routes[k].Add(VCRPInstance.depot);
                solution.cost += VCRPInstance.weight_matrix[lastClient, VCRPInstance.depot];
            }
        }

        // Check if solution is feasible and repair it if not
        public static void GreedyPostProcessing(VCRPSolution solution)
        {
            // List contains the total sum of demands in each route
            List<int> routesDemand = new List<int>();            
            // List of unfeasible routes and it's excess over capacity 
            List<int> brokenRoutes = new List<int>();
            int[] excessError = new int[VCRPInstance.n_vehicles];

            // USE 2-OPT to organize routes for a better solution
            LocalSearch(solution);
            Console.WriteLine("DONE! - LOCAL SEARCH IN THE GREEDY SOLUTION [ NON FEASIBLE ]");

            // get sum of client's demand for each route
            for (int k = 0; k < VCRPInstance.n_vehicles; k++)
            {
                int currentRouteDemand = getRouteDemand(solution.routes[k]);
                routesDemand.Add(currentRouteDemand);
                if (currentRouteDemand > VCRPInstance.g_capacity)
                {
                    brokenRoutes.Add(k);
                    excessError[k] = currentRouteDemand - VCRPInstance.g_capacity;
                }
            }

            // TODO: Kill them!
            Tuple<double, int, int, int, int> bestFact = new Tuple<double, int, int, int, int>(double.MaxValue, -1, -1, -1, -1);
            Tuple<double, int, int, int, int> bestF = new Tuple<double, int, int, int, int>(double.MaxValue, -1, -1, -1, -1);



            // If there is any item in brokenRoutes, current solution is not feasible;
            // Then repair it;
            while (brokenRoutes.Count != 0)
            {
                // Find movement to improve feasibility of current solution
                foreach(int brokenRouteIndex in brokenRoutes)
                {
                    for(int k = 0; k < VCRPInstance.n_vehicles; k++)
                    {
                        if(brokenRouteIndex != k)
                        {
                            foreach(int client in solution.routes[brokenRouteIndex])
                            {
                                int localExcessError = 0;

                                if (VCRPInstance.nodes[client].demand + routesDemand.ElementAt(k) > VCRPInstance.g_capacity)
                                    localExcessError = (VCRPInstance.nodes[client].demand + routesDemand.ElementAt(k)) - VCRPInstance.g_capacity;

                                if (routesDemand.ElementAt(brokenRouteIndex) - VCRPInstance.nodes[client].demand > VCRPInstance.g_capacity)
                                    localExcessError += routesDemand.ElementAt(brokenRouteIndex) - VCRPInstance.nodes[client].demand;

                                if(localExcessError < excessError[brokenRouteIndex])
                                {
                                    Tuple<double, int> movement = Inter10(solution, client, brokenRouteIndex, k);

                                    if (localExcessError == 0 && bestFact.Item1 > movement.Item1)
                                        bestFact = new Tuple<double, int, int, int, int>(movement.Item1, movement.Item2, client, k, brokenRouteIndex);
                                    else
                                    {
                                        if (bestF.Item1 > movement.Item1)
                                            bestF = new Tuple<double, int, int, int, int>(movement.Item1, movement.Item2, client, k, brokenRouteIndex);
                                    }
                                }
                            }
                        }
                    }
                }

                // Execute movement
                if (bestFact.Item2 != -1)
                {
                    // There is a movement that makes the route feasible
                    // Update solution and remove route from brokenRoutes
                    Console.WriteLine("Movimento pra resolver o erro! Rota: " + bestFact.Item5);
                    solution.cost = bestFact.Item1;
                    solution.routes[bestFact.Item4].Insert(bestFact.Item2, bestFact.Item3);
                    solution.routes[bestFact.Item5].Remove(bestFact.Item3);
                    brokenRoutes.Remove(bestFact.Item5);
                }
                else if(bestF.Item2 != -1)
                {
                    Console.WriteLine("Movimento pra reduzir o erro");
                    solution.cost = bestF.Item1;
                    solution.routes[bestF.Item4].Insert(bestF.Item2, bestF.Item3);
                    solution.routes[bestF.Item5].Remove(bestF.Item3);
                }
            }
        }


        public static Tuple<double, int> Inter10(VCRPSolution solution, int client, int brokenRouteIndex, int k)
        {
            //passa c1 de k1 para k2, na melhor posicao possivel. Nao aplica realmente, só calcula
            List<int> full_route = solution.routes[k];
            int pos_c1 = solution.routes[brokenRouteIndex].IndexOf(client);
            Tuple<double, int> move = new Tuple<double, int>(double.MaxValue, -1);
            for (int i = 1; i < full_route.Count() - 1; i++)
            {
                double custo = solution.cost - (VCRPInstance.weight_matrix[i - 1, i]);
                custo += (VCRPInstance.weight_matrix[i - 1, client]);
                custo += (VCRPInstance.weight_matrix[client, i]);
                custo -= (VCRPInstance.weight_matrix[solution.routes[brokenRouteIndex][pos_c1] - 1, pos_c1]);
                custo -= (VCRPInstance.weight_matrix[pos_c1, solution.routes[brokenRouteIndex][pos_c1 + 1]]);
                custo += (VCRPInstance.weight_matrix[solution.routes[brokenRouteIndex][pos_c1 - 1], solution.routes[brokenRouteIndex][pos_c1 + 1]]);

                if (custo < move.Item1)
                    move = new Tuple<double, int>(custo, i);
            }
            return move;
        }

        public static int getRouteDemand(List<int> route)
        {
            return route.Sum(client => VCRPInstance.nodes[client].demand);
        }

    }
}

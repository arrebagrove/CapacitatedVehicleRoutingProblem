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
            int[] routesDemand = new int[VCRPInstance.n_vehicles];            
            // List of unfeasible routes and it's excess over capacity 
            List<int> brokenRoutes = new List<int>();
            int[] excessError = new int[VCRPInstance.n_vehicles];

            // get sum of client's demand for each route
            for (int k = 0; k < VCRPInstance.n_vehicles; k++)
            {
                int currentRouteDemand = getRouteDemand(solution.routes[k]);
                routesDemand[k]= currentRouteDemand;
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
                        // check movement to another route
                        if (brokenRouteIndex != k)
                        {
                            // One of the clients can reduce excessError?
                            foreach(int client in solution.routes[brokenRouteIndex].Where(x => x > 0))
                            {
                                // Check if the demand of a client inside a broken route can be inserted in a feasible route without breaking it
                                if (VCRPInstance.nodes[client].demand + routesDemand[k] <= VCRPInstance.g_capacity)
                                {
                                    if (routesDemand[brokenRouteIndex] - VCRPInstance.nodes[client].demand <= VCRPInstance.g_capacity)
                                    {
                                        // Get best movement
                                        Tuple<double, int> movement = Inter10(solution, client, brokenRouteIndex, k);
                                        if(bestFact.Item1 > movement.Item1)
                                            bestFact = new Tuple<double, int, int, int, int>(movement.Item1, movement.Item2, client, k, brokenRouteIndex);
                                    }
                                }
                                else if (routesDemand[brokenRouteIndex] - VCRPInstance.nodes[client].demand > VCRPInstance.g_capacity)
                                {
                                    Tuple<double, int> movement = Inter10(solution, client, brokenRouteIndex, k);
                                    // Movement reduces excess error
                                    if (bestF.Item1 > movement.Item1)
                                        bestF = new Tuple<double, int, int, int, int>(movement.Item1, movement.Item2, client, k, brokenRouteIndex);
                                } 
                            }
                        }
                    }
                }

                // Execute movement
                // There is a movement that makes broken route feasible without breking another one
                if (bestFact.Item2 != -1 && brokenRoutes.Contains(bestFact.Item5))
                {
                    // Update solution and remove route from brokenRoutes
                    Console.WriteLine("Movimento pra resolver o erro! Rota: " + bestFact.Item5);
                    solution.cost = bestFact.Item1;
                    solution.routes[bestFact.Item4].Insert(bestFact.Item2, bestFact.Item3);
                    solution.routes[bestFact.Item5].Remove(bestFact.Item3);
                    brokenRoutes.Remove(bestFact.Item5);
                    routesDemand[bestFact.Item4] += VCRPInstance.nodes[bestFact.Item3].demand;
                    routesDemand[bestFact.Item5] -= VCRPInstance.nodes[bestFact.Item3].demand;
                    bestFact = new Tuple<double, int, int, int, int>(double.MaxValue, -1, -1, -1, -1);
                }
                else if(bestF.Item2 != -1)
                {
                    Console.WriteLine("Movimento pra reduzir o erro rota:" + bestF.Item5);
                    solution.cost = bestF.Item1;
                    solution.routes[bestF.Item4].Insert(bestF.Item2, bestF.Item3);
                    solution.routes[bestF.Item5].Remove(bestF.Item3);
                    routesDemand[bestF.Item4] += VCRPInstance.nodes[bestF.Item3].demand;
                    routesDemand[bestF.Item5] -= VCRPInstance.nodes[bestF.Item3].demand;

                    if(!brokenRoutes.Contains(bestF.Item4))
                        brokenRoutes.Add(bestF.Item4);

                    bestF = new Tuple<double, int, int, int, int>(double.MaxValue, -1, -1, -1, -1);
                }
            }
        }


        public static Tuple<double, int> Inter10(VCRPSolution solution, int client, int brokenRouteIndex, int k)
        {
            //passa c1 de k1 para k2, na melhor posicao possivel. Nao aplica realmente, só calcula
            List<int> modifiedRoute = solution.routes[k];

            int clientPosition = solution.routes[brokenRouteIndex].IndexOf(client);

            Tuple<double, int> move = new Tuple<double, int>(double.MaxValue, -1);
            
            // Get best client to insert before 
            for (int i = 1; i < modifiedRoute.Count() - 1; i++)
            {
                //double cost = solution.cost - (VCRPInstance.weight_matrix[i - 1, i]);
                double cost = solution.cost - (VCRPInstance.weight_matrix[modifiedRoute.ElementAt(i - 1), modifiedRoute.ElementAt(i)]);

                // Costs of inserting client
                cost += (VCRPInstance.weight_matrix[modifiedRoute.ElementAt(i - 1), client]);
                cost += (VCRPInstance.weight_matrix[client, modifiedRoute.ElementAt(i)]);

                // Old costs involving client to be moved
                cost -= (VCRPInstance.weight_matrix[solution.routes[brokenRouteIndex][clientPosition] - 1, client]);
                cost -= (VCRPInstance.weight_matrix[client, solution.routes[brokenRouteIndex][clientPosition + 1]]);

                // New vert in the broken route
                cost += (VCRPInstance.weight_matrix[solution.routes[brokenRouteIndex][clientPosition - 1], solution.routes[brokenRouteIndex][clientPosition + 1]]);

                if (cost < move.Item1)
                    move = new Tuple<double, int>(cost, i);
            }
            return move;
        }

        public static int getRouteDemand(List<int> route)
        {
            return route.Sum(client => VCRPInstance.nodes[client].demand);
        }

    }
}

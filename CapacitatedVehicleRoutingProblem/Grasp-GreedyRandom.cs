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
        public static void GreedyPostProcessing(VCRPInstance solution)
        {
            // Check if it's feasible

        }

    }
}

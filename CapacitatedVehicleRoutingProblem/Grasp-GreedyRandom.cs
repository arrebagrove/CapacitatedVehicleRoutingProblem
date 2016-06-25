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
        procedure Greedy Randomized Construction(Seed)
            1 Solution ← ∅;
            2 Evaluate the incremental costs of the candidate elements;
            3 while Solution is not a complete solution do
            4 Build the restricted candidate list(RCL);
            5 Select an element s from the RCL at random;
            6 Solution ← Solution ∪ {s
                };
            7 Reevaluate the incremental costs;
            8 end;
            9 return Solution;
        end Greedy Randomized Construction.
        */

        public static VCRPSolution GreedyRandomizedSolution(int alpha, int seed)
        {            
            // Instanciate solution
            VCRPSolution solution = new VCRPSolution(VCRPInstance.n_vehicles,VCRPInstance.n_nodes);

            // Get all clients as candidates for candidatesSet; Cost(Depot -> Client)  
            List<Tuple<int,int,double>> candidatesSet = getCandidates();

            // Restricted Candidate List (RCL)
            List<Tuple<int, int, double>> RCL = new List<Tuple<int, int, double>>();

            // Evaluate Min and Max cost of the candidatesSet
            Tuple<double,double> cMinMax = getcMinMax(candidatesSet);
            double cMin = cMinMax.Item1;
            double cMax = cMinMax.Item2;

            // Instanciate random 
            Random rand = new Random();

            while (candidatesSet.Count != 0)
            {
                // Creates new RCL
                foreach (var candidate in candidatesSet)
                {
                    double currentCost = candidate.Item3;
                    if (currentCost <= cMin + alpha * (cMax - cMin))
                    {
                        RCL.Add(candidate);
                    }
                }

                if(RCL.Count != 0)
                {
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
                else
                {
                    // No more eligible candidate in the candidatesSet
                    candidatesSet.Clear();
                }
            }

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
            
            //TODO: Use x ?          
        }

        // Evaluate Min and Max cost of the current candidatesSet
        public static Tuple<double,double> getcMinMax(List<Tuple<int,int,double>> candidatesSet)
        {
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
            int depot = 0;

            for(int i = 1; i < VCRPInstance.n_nodes; i++)
            {
                for(int k = 0; k<VCRPInstance.n_vehicles; k++)
                {
                    candidateSet.Add(Tuple.Create(i, k, VCRPInstance.weight_matrix[depot,i]));
                }
            }

            return candidateSet;
        }
    
    }
}

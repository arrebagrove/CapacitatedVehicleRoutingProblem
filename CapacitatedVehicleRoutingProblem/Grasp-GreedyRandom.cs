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

            // Get all clients as candidates for C set
            double[,] CandidatesSet = getCandidates();   
                        
            // Restricted Candidate List
            List<Tuple<int,int>> RCL = new List<Tuple<int, int>>();

            // Evaluate cost Min and Max of c(e) of all elements in CandidatesSet
            Tuple<double,double> cMinMax = getcMinMax(CandidatesSet);
            double cMin = cMinMax.Item1;
            double cMax = cMinMax.Item2;

            // Creates RCL
            for (int i = 0; i < VCRPInstance.n_nodes; i++)
            {
                for (int k = 0; k < VCRPInstance.n_vehicles; k++)
                {
                    double currentCost = CandidatesSet[i, k];
                    if(currentCost <= cMin + alpha*(cMax - cMin))
                    {
                        RCL.Add(Tuple.Create(i,k));
                    }
                }
            }

            // Select a random candidate from RCL ; Candidate = <Client,Vehicle>
            Random rand = new Random();
            int chosenIndex = rand.Next(1, RCL.Count);
            Tuple<int,int> currentCandidate = RCL.ElementAt(chosenIndex);

            // Check if it's feasible to insert the candidate in the solution
            insertSolutionElement(currentCandidate, solution);
            

            return null;
        }

        // Function with containing restraints 
        public static void insertSolutionElement(Tuple<int, int> currentCandidate, VCRPSolution solution)
        {
            int client = currentCandidate.Item1;
            int route = currentCandidate.Item2;

            //Check
            /* 
            if ()
            {
                solution.routes[route].Add(client);
            }*/
          
        }

        public static Tuple<double,double> getcMinMax(double[,] CandidatesSet)
        {
            double cMin = INFINITY;
            double cMax = -INFINITY;


            for (int i = 0; i < VCRPInstance.n_nodes; i++)
            {
                for (int k = 0; k < VCRPInstance.n_vehicles; k++)
                {
                    // Update cMin and cMax
                    if (cMin > CandidatesSet[i, k])
                    {
                        cMin = CandidatesSet[i, k];
                    }
                    if (cMax < CandidatesSet[i, k])
                    {
                        cMax = CandidatesSet[i, k];
                    }

                }
            }

            return Tuple.Create(cMin,cMax);
        }
        
        public static double[,] getCandidates()
        {
            // ALL CLIENTS
            double[,] candidatesSet = new double[1, VCRPInstance.n_vehicles];
            int depot = 0;

            for(int i = 0; i < VCRPInstance.n_nodes; i++)
            {
                for(int k = 0; k < VCRPInstance.n_vehicles; k++)
                {
                    candidatesSet[i, k] = VCRPInstance.weight_matrix[i, depot];
                }
            }

            return candidatesSet;
        }


    }
}

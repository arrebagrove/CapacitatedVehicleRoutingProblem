using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    static partial class Grasp
    {
        public const int INFINITY = UInt16.MaxValue; // Positive Infinity

        private static int globalOptimal = INFINITY;

        public static int Execute(VCRPInstance instance)
        {                                                
            // Grasp Parameters
            int maxIterations = 10;
            int alpha = 0;
            // 
            int localOptimal = 0;
            int seed = 0; // ?


            // Constructive Heuristic Phase
            /*
            procedure GRASP(Max Iterations,Seed)
                1 Read Input();
                2 for k = 1, . . . , Max Iterations do
                3 Solution ← Greedy Randomized Construction(Seed);
                4 Solution ← Local Search(Solution);
                5 Update Solution(Solution,Best Solution);
                6 end;
                7 return Best Solution;
            end GRASP.
            */

            VCRPSolution solution = new VCRPSolution(VCRPInstance.n_vehicles, VCRPInstance.n_nodes);
            for (int k=0; k < maxIterations; k++)
            {
                //localOptimal = INFINITY;

                solution = GreedyRandomizedSolution(alpha, seed);
                solution = LocalSearch(solution);

                if (localOptimal < globalOptimal)
                {
                    globalOptimal = localOptimal;
                    //UpdateGlobalSolution(S, assignments, facID, bestLocalDist);
                }
            }
            
            return globalOptimal;
        }
    }
}

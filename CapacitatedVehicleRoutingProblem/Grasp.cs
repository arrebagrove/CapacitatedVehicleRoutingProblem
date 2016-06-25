using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    static partial class Grasp
    { 
        public static VCRPSolution Execute(VCRPInstance instance)
        {
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

            // Grasp Parameters
            int maxIterations = 1;
            int alpha = 1;
            int seed = 0; // ?

            // Initialize best solution with worst cost
            const int worstSolution = UInt16.MaxValue;
            VCRPSolution bestSolution = new VCRPSolution(VCRPInstance.n_vehicles, VCRPInstance.n_nodes);
            bestSolution.cost = worstSolution;

            // Solution instance used for grasp iterations
            VCRPSolution currentSolution;

            for (int k = 0; k < maxIterations; k++)
            {
                // Constructive Heuristic Phase
                currentSolution = GreedyRandomizedSolution(alpha, seed);

                // Local Search
                // TODO: Implement Local Search
                //solution = LocalSearch(solution);

                // Check and update best solution
                updateBestSolution(bestSolution, currentSolution);
            }

            return bestSolution;
        }

        private static void updateBestSolution(VCRPSolution bestSolution, VCRPSolution newSolution)
        {
            if(bestSolution.cost > newSolution.cost)
            {
                bestSolution.cost = newSolution.cost;
                bestSolution.routes = newSolution.routes;
                bestSolution.x = newSolution.x;
            }
        }
    }
}

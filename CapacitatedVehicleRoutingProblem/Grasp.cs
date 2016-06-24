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
        public const int INFINITY = UInt16.MaxValue; // Positive Infinity

        private static int globalOptimal = INFINITY;
        private static VCRPSolution bestSolution = new VCRPSolution(VCRPInstance.n_vehicles, VCRPInstance.n_nodes);

        public static VCRPSolution Execute(VCRPInstance instance)
        {
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

            // Grasp Parameters
            int maxIterations = 1;
            int alpha = 1;
            
            int seed = 0; // ?

            VCRPSolution solution = new VCRPSolution(VCRPInstance.n_vehicles, VCRPInstance.n_nodes);

            for (int k=0; k < maxIterations; k++)
            {
                solution = GreedyRandomizedSolution(alpha, seed);
               
                // TODO: REMOVER-  Output para testes
                for (int b = 0; b < VCRPInstance.n_vehicles; b++)
                {
                    Console.WriteLine("Rota "+b+":");
                    List<int> range = solution.routes[b];
                    foreach (int value in range)
                    {
                        Console.Write(value + " "); // bird, plant
                    }
                    Console.WriteLine("\n");
                              
                }

                // Local Search
                // TODO: Implement Local Search
                //solution = LocalSearch(solution);

                // Check and update best solution
                updateBestSolution(solution);

            }

            return bestSolution;
        }

        private static void updateBestSolution(VCRPSolution newSolution)
        {
            if(bestSolution.cost > newSolution.cost)
            {
                bestSolution = newSolution;
            }
        }
    }
}

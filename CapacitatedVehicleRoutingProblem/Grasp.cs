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
            int maxIterations = 3;
            double alpha = 1;
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

                Console.WriteLine("=============================================\n");
                Console.WriteLine("GREEDY - INITIAL SOLUTION\n");
                for (int v= 0; v < VCRPInstance.n_vehicles; v++)
                {
                    Console.Write("Rota " + v + ": ");
                    List<int> range = currentSolution.routes[v];
                    foreach (int value in range)
                    {
                        Console.Write(value + " - ");
                    }
                    Console.WriteLine("\n");
                }
                Console.WriteLine("Custo total: " + currentSolution.cost + "\n");

                // Check and repair solution in case it's not feasible (YET!)
                GreedyPostProcessing(currentSolution);

                Console.WriteLine("=============================================\n");
                Console.WriteLine("GREEDY - POST PROCESSING\n");
                for (int v = 0; v < VCRPInstance.n_vehicles; v++)
                {
                    Console.Write("Rota " + v + ":" );
                    List<int> range = currentSolution.routes[v];
                    foreach (int value in range)
                    {
                        Console.Write(value + " - ");
                    }
                    Console.WriteLine(getRouteDemand(currentSolution.routes[v]));
                    Console.WriteLine("\n");
                }
                Console.WriteLine("Custo total: " + currentSolution.cost + "\n");

                // Local Search
                LocalSearch(currentSolution);

                Console.WriteLine("=============================================\n");
                Console.WriteLine("AFTER LOCAL SEARCH\n");
                for (int v = 0; v < VCRPInstance.n_vehicles; v++)
                {
                    Console.Write("Rota " + v + ": ");
                    List<int> range = currentSolution.routes[v];
                    foreach (int value in range)
                    {
                        Console.Write(value + " - ");
                    }
                    Console.WriteLine("\n");
                }
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
            }
        }
    }
}

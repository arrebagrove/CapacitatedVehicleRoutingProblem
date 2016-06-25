using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    static partial class Grasp
    {
        public static void LocalSearch(VCRPSolution solution)
        {

            /*
            Console.WriteLine("Antes de OPT -----------\n");
            for (int k = 0; k < VCRPInstance.n_vehicles; k++)
            {
                Console.Write("Rota " + k + ":");
                List<int> range = solution.routes[k];
                foreach (int value in range)
                {
                    Console.Write(value + " - ");
                }
                Console.WriteLine("\n");
            }*/

            // Execute 2-opt for each route of the current solution
            double newTotalCost = 0;
            for (int k = 0; k < VCRPInstance.n_vehicles; k++)
            {
                newTotalCost += TwoOpt(solution.routes[k]);
            }

            solution.cost = newTotalCost;
            


            /*
            Console.WriteLine("Depois de OPT -----------\n");
            for (int k = 0; k < VCRPInstance.n_vehicles; k++)
            {
                Console.Write("Rota " + k + ":");
                List<int> range = solution.routes[k];
                foreach (int value in range)
                {
                    Console.Write(value + " - ");
                }
                Console.WriteLine("\n");
            }*/

            //return solution;
        }


        // Try to improve route cost
        public static double TwoOpt(List<int> route)
        {
            // best route
            List<int> newRoute = new List<int>();
            
            // repeat until no improvement is made 
            bool noChange = true;

            double bestCost = getRouteCost(route);

            route.RemoveAll(x => x == VCRPInstance.depot);
            
            // Get tour size
            int size = route.Count();

            while (noChange)
            {
                for (int i = 0; i < size - 1; i++)
                {
                    for (int k = i + 1; k < size; k++)
                    {
                        newRoute = TwoOptSwap(route, i, k);

                        newRoute.Insert(0, VCRPInstance.depot);
                        newRoute.Add(VCRPInstance.depot);     

                        double newCost = getRouteCost(newRoute);

                        if (newCost < bestCost)
                        {
                            // Improvement found so reset
                            route.Clear();
                            route.AddRange(newRoute);
                            bestCost = newCost;
                            route.RemoveAll(x => x == VCRPInstance.depot);
                        }
                        else
                        {
                            noChange = false;
                        }
                    }
                }
            }
            route.Insert(0, VCRPInstance.depot);
            route.Add(VCRPInstance.depot);

            return bestCost;
        }


        public static List<int> TwoOptSwap(List<int> route, int i, int k ) 
        {
            List<int> newRoute = new List<int>();

            double size = route.Count();
 
            // 1. take route[0] to route[i-1] and add them in order to new_route
            for ( int c = 0; c <= i - 1; ++c)
            {
                newRoute.Add(route.ElementAt(c));
            }

            // 2. take route[i] to route[k] and add them in reverse order to new_route
            int dec = 0;
            for ( int c = i; c <= k; ++c)
            {
                newRoute.Add(route.ElementAt(k - dec));
                dec++;
            }
 
            // 3. take route[k+1] to end and add them in order to new_route
            for ( int c = k + 1; c < size; ++c)
            {
                newRoute.Add(route.ElementAt(c));
            }

            return newRoute;
        }

        // Evaluate cost of a given route
        public static double getRouteCost(List<int> route)
        {
            double totalCost = 0;

            for (int i=0; i < route.Count-1; i++)
            {
                totalCost += VCRPInstance.weight_matrix[route.ElementAt(i), route.ElementAt(i+1)];
            }
 
            return totalCost;
        }

        public static void SwapRouteIndexes(IList<int> list, int indexA, int indexB)
        {
            int tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

    }
}

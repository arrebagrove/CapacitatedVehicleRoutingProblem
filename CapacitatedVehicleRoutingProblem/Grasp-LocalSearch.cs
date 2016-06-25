using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    static partial class Grasp
    {
        public static VCRPSolution LocalSearch(VCRPSolution solution)
        {
            

            return null;
        }


        // Selects randomly n clients positions and move them randomly inside the route
        public static void intraMovement(List<int> route, int n)
        {
            Random rand = new Random();
            
            List<int> chosenPositions = new List<int>;
            for (int i = 0; i < n; i++)
            {
                bool wasMoved = false;
                while (wasMoved != true)
                {
                    // Disconsider depot: Route -> (depot - n clients - depot )
                    int index1 = rand.Next(1, route.Count - 2);
                    int index2 = rand.Next(1, route.Count - 2);

                    if(index1 != index2)
                    {
                        SwapRouteIndexes(route, index1, index2);
                        wasMoved = true;
                    }
                }
            }
            
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

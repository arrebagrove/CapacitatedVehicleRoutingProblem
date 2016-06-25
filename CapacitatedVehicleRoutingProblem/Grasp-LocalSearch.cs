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

        public static double getCostOfRoute(List<int> route)
        {
            double totalCost = 0;

            for(int i=0; i < route.Count; i++)
            {
                totalCost += VCRPInstance.weight_matrix[route.ElementAt(i), route.ElementAt(i+1)];
            }
 
            return totalCost;
        }
    }
}

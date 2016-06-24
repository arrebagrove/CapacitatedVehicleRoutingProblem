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

        public static VCRPSolution GreedyRandomizedSolution(int seed)
        {
            VCRPSolution solution = new VCRPSolution();

            /*
            while(solution == VCRPInstance.n_vehicles)
            {


            }
            */

            return null;
        }

    }
}

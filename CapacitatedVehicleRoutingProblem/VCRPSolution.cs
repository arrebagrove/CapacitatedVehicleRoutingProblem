﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    class VCRPSolution
    {
        public double cost;
        public List<int>[] routes; //k routes, up to n-1 client nodes
 
        // K routes/vehicles, N nodes 
        public VCRPSolution(int k, int n)
        {
            cost = 0;
            routes = new List<int>[k];
            for (int i = 0; i < k; i++)
            {
                routes[i] = new List<int>();
                routes[i].Add(VCRPInstance.depot);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    class VCRPInstance
    {
        public static int n_nodes;
        public static int n_vehicles;
        public static int g_capacity;
        public static int depot;
        public static double[,] weight_matrix;
        public static Node[] nodes;
    }

    class Node
    {
        public int demand;
        public int p_x;
        public int p_y;

        public Node(int x, int y)
        {
            p_x = x;
            p_y = y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("=========================================================================");
            Console.WriteLine("Capacitated Vehicle Routing Problem (CVRP) with GRASP \n Mateus Riad (228157) \n Ricardo Pires (208784) \n");
            Console.WriteLine("(All the instances can be found inside this project, dir: instances) \n");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("Name of the instance to execute: ");
            string fileName = Console.ReadLine();

            // Pega as informaçoes do arquivo de instancia passado
            Parser.parserFile(fileName);

            Console.WriteLine("=== EXECUTING GRASP ===\n");
            // Inicializa timer 
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // GRASP
            VCRPInstance instance = new VCRPInstance();
            VCRPSolution bestSolution = Grasp.Execute(instance); 
            stopWatch.Stop();

            Console.WriteLine("=== SOLUTION FOUND ===\n");
            Console.WriteLine("Best solution found: " + bestSolution.cost);
            Console.WriteLine("Executiond Time:" + stopWatch.Elapsed + " seconds.");

            for(int k=0; k < VCRPInstance.n_vehicles; k++)
            {
                Console.Write("Rota " + k + ":");
                List<int> range = bestSolution.routes[k];
                foreach (int value in range)
                {
                    Console.Write(value + " - ");
                }
                Console.WriteLine("\n");
            }


            string wait = Console.ReadLine();
        }
    }
}

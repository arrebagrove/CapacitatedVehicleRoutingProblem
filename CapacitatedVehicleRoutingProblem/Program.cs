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
            Console.WriteLine(" Capacitated Vehicle Routing Problem (CVRP) Utilizando GRASP \n Mateus Riad (228157) \n Ricardo Pires (208784) \n");
            Console.WriteLine("(Instancias serao pegas da pasta instancias, junto com a pasta do projeto) \n");
            Console.WriteLine("Escolha o nome da instancia que deseja utilizar: ");
            string fileName = Console.ReadLine();

            // Pega as informaçoes do arquivo de instancia passado
            Parser.parserFile(fileName);

            // Inicializa timer 
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // TODO: GRASP 

            stopWatch.Stop();

            //Console.WriteLine("Valor da solucao final: " +);
            //Console.WriteLine("Solução final encontrada em " + stopWatch.Elapsed + " segundos.);
            string wait = Console.ReadLine();
        }
    }
}

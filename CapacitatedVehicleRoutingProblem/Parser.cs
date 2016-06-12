using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapacitatedVehicleRoutingProblem
{
    class Parser
    {
        public static void parserFile(String fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../../../instances/" + fileName))
                {
                    String line;

                    // First line contains: N(number os nodes) K(number of vehicles) C(Global Capacity)
                    if ((line = sr.ReadLine()) != null)
                    {
                        String[] words = line.Split(' ');
                        VCRPInstance.n_nodes = Convert.ToInt32(words[0]);
                        VCRPInstance.n_vehicles = Convert.ToInt32(words[1]);
                        VCRPInstance.g_capacity = Convert.ToInt32(words[2]);
                    }

                    // Set of nodes positions
                    int nodesCreated = 0;
                    VCRPInstance.nodes = new Node[VCRPInstance.n_nodes];
                    for (int i = 0; i < VCRPInstance.n_nodes; i++)
                    {
                        if ((line = sr.ReadLine()) != null)
                        {
                            String[] words = line.Trim().Split(' ');
                            if (words.Length == 3)
                            {
                                VCRPInstance.nodes[i] = new Node(Convert.ToInt32(words[1]), Convert.ToInt32(words[2]));
                                nodesCreated++;
                            }
                        }
                    }

                    // Set of nodes demands
                    int demandSet = 0;
                    for (int i = 0; i < VCRPInstance.n_nodes; i++)
                    {
                        if ((line = sr.ReadLine()) != null)
                        {
                            String[] words = line.Trim().Split(' ');
                            if (words.Length == 2)
                            {
                                VCRPInstance.nodes[i].demand = Convert.ToInt32(words[1]);
                                demandSet++;
                            }
                        }
                    }

                    // Inforce the integrity of the instance strutcture
                    if (nodesCreated != VCRPInstance.n_nodes || demandSet != VCRPInstance.n_nodes)
                    {
                        Console.WriteLine("\n Instancia invalida. Terminando Programa.\n");
                        Console.WriteLine("\n Pressione qualquer tecla.\n");
                        string wait = Console.ReadLine();
                        System.Environment.Exit(1);
                    }

                    VCRPInstance.weight_matrix = new double[VCRPInstance.n_nodes, VCRPInstance.n_nodes];
                    // Create weight matrix, containing the vert weight calc for each node
                    for (int i = 0; i < VCRPInstance.n_nodes; i++)
                    {
                        for (int j = 0; j < VCRPInstance.n_nodes; j++)
                        {
                            VCRPInstance.weight_matrix[i, j] = GetDistance(VCRPInstance.nodes[i], VCRPInstance.nodes[j]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("O arquivo nao pode ser lido:");
                Console.WriteLine(e.Message);
                string enter = Console.ReadLine();
            }

        }

        // Given two nodes, calculates the distance between them
        private static double GetDistance(Node n1, Node n2)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double a = (double)(n2.p_x - n1.p_x);
            double b = (double)(n2.p_y - n2.p_y);

            return Math.Sqrt(a * a + b * b);
        }
    }
}

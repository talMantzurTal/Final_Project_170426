using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Simulation
    {
        public Globals globals;
        private double Eps;
        private double Error_fraction;
        private double Delta;

        // For singleton
        private static Simulation ui = null;
        private static bool flag_instance;

        private Simulation()
        {
            Eps = 0;
            Error_fraction = 0;
            Delta = 0;
            flag_instance = true;
            globals = Globals.get_instance();
        }

        public void simulate(/*int number_of_gates,*/ int number_of_literals, ref List<int[]> input_vectors/*, ref List<int[]> error_vectors*/)
        {
            // 1. Black box - main in program

            // 2. Generate input vectors for f, F (size = number of literals)
            int[] bin_vector = new int[number_of_literals];
            /* for multiple input vectors
             * **************************
            globals.generate_alphabeth_vectors(bin_vector, 0, bin_vector.Length,2);
            List<int[]> tmp_input_vectors = new List<int[]>();
            tmp_input_vectors = globals.get_error_vectors_list();
            int[] copied_vector = new int[number_of_literals];

            foreach ( int[] vector in input_vectors )
            {
                for (int i = 0; i<number_of_literals; i++)
                {
                    copied_vector[i] = vector[i];
                }
                input_vectors.Add(copied_vector);
            }
            ******************************************************************************/
            int tree_depth = (int)Math.Log(number_of_literals, 2);
            globals.clear_error_vector_list();
            int[] input_vector = new int[number_of_literals];
            if (tree_depth <=/*Tal= */4)
            {
                
                globals.generate_alphabeth_vectors(input_vector, 0, input_vector.Length, 2);
                input_vectors = globals.get_error_vectors_list();
            }
            else
                globals.random_vectors(500, number_of_literals, 1, ref input_vectors);

            
            // random_vectors(1, number_of_literals, 1, ref input_vectors);

            // 3. Generate error vectors for F
            //globals.clear_error_vector_list();
#if t
            int[] error_vector = new int[number_of_gates];
            //globals.generate_alphabeth_vectors(error_vector, 0, error_vector.Length, 3);
            //error_vectors = globals.get_error_vectors_list();
            List<int[]> randomized_path = new List<int[]>();
            random_vectors(5, number_of_gates, 2, ref randomized_path);
            
            while (error_vectors.Count == 0)
            {
                //random_vectors(5, number_of_gates, 1, ref error_vectors);
                for (int num_of_vectors = 0; num_of_vectors < number_of_gates; num_of_vectors++)
                {
                    int[] vector = new int[number_of_gates];
                    for (int i = 0; i < 5; i++)
                    {
                        if (i % 8 == 0)
                            vector[i * 8] = 1;
                        else vector[i * 8] = 0;
                    }
                    error_vectors.Add(vector);
                }
                error_vectors = globals.limit_num_of_errors(error_vectors, globals.delta);
            }
            error_vectors = globals.generate_legel_vectors(error_vectors, randomized_path);
#endif
            // 4. Calculate formula trees: f, F

            // 5. Compare f_output, F_output and print results to a file
            return;


        }

        public void ui_input_parameters()
        {
            Console.WriteLine("Please enter a value for epsilon\n");
            Eps = globals.convert_string_fraction2double(Console.ReadLine());
            Console.WriteLine("Please enter a value for error_fraction\n");
            Error_fraction = globals.convert_string_fraction2double(Console.ReadLine());
            Console.WriteLine("Please enter a value for delta\n");
            Delta = globals.convert_string_fraction2double(Console.ReadLine());
            globals.set_ui_parameters(Eps, Error_fraction, Delta);
        }

        public void internal_input_parameters(double[] input_params)
        {
            Eps = input_params[0];
            Error_fraction = input_params[1];
            Delta = input_params[2];
            globals.set_ui_parameters(Eps, Error_fraction, Delta);
        }

        public void ui_output_parameters()
        {
            
        }
        public static Simulation get_instance()
        {
            if (!flag_instance)
                ui = new Simulation();
            return ui;
        }

       
   
        }
}

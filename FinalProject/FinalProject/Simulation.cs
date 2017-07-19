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

        public void simulate(int number_of_gates, int number_of_literals, List<int[]> input_vectors, List<int[]> error_vectors)
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
            Random rnd = new Random();
            int[] input_vector = new int[number_of_literals];
            for (int i = 0; i< number_of_literals; i++)
            {
                input_vector[i] = rnd.Next(0, 2); // Random number 0 or 1
            }
            input_vectors.Add(input_vector);

            // 3. Generate error vectors for F
            int[] error_vector = new int[number_of_gates];
            globals.generate_alphabeth_vectors(error_vector, 0, error_vector.Length, 3);
            error_vectors = globals.get_error_vectors_list();
            error_vectors = globals.limit_num_of_errors(error_vectors, globals.delta);

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

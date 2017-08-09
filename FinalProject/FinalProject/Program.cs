#define PRINT_F
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinalProject
{
    enum node_type
    {
        NA,
        AND,
        OR,
        ALICE,
        BOB
    };

    enum copy_flags
    {
        NA,
        SHALLOW,
        DEEP
    };

    enum reachable_type
    {
        NA,
        REACHABLE,
        UNREACHABLE
    };

    enum formula_result_type
    {
        FALSE,
        TRUE,
        ILLEGAL
    };

    class Program
    {


        static void Main(string[] args)
        {
            //TreeUtils.set_tree_names(6);
            Globals globals;
            globals = Globals.get_instance();
            Simulation simulation = Simulation.get_instance();
            //simulation.ui_input_parameters();
            List<double[]> simulations_param = new List<double[]>();
            double delta = 0.0, epsilon = globals.eps;
            // Run simulation for multiple values of epsilon:
            // ---------------------------------------------
            for (epsilon = /*2.0 / 15.0*/5.0/30.0; epsilon < 1.0 / 3.0; epsilon += 1.0 / 90.0)
            {
                double[] sim_params = { epsilon, 1.0/3.0-epsilon, 0 };
                simulation.internal_input_parameters(sim_params);

                FormulaTree formula_tree_input = new FormulaTree(), resilient_formula = new FormulaTree();
                main_black_box(ref formula_tree_input, ref resilient_formula);

            
                var FILE_PATH = string.Format(@"C:\Users\mantz\Documents\GitHub\Final_Project_170202\Final_Project_170426\simulation_results_eps_{0}.txt", globals.eps.ToString());
                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(FILE_PATH))
                {
                    // Run simulation for multiple values of delta:
                    // -------------------------------------------
                    for (delta = 0.0; delta <= globals.error_fraction; delta = delta + 0.01)
                    {
                        List<int[]> input_vectors = new List<int[]>();
                        List<int[]> error_vectors = new List<int[]>();
                        // Update globals.delta
                        globals.delta = delta;


                        // Simulate:
                        // --------
                        simulation.simulate(/*resilient_formula.get_number_of_int_nodes(),*/ formula_tree_input.get_number_of_leaves(), ref input_vectors/*, ref error_vectors*/);
                        generate_err_vectors(resilient_formula, ref error_vectors, 1, resilient_formula.get_number_of_int_nodes(), globals.delta);
                        formula_result_type f_output, F_output;
                        string msg1 = "", msg2 = "";
                        string symbol;
                        string[] pass_fail_msg = { "Pass", "Fail" };
                        int pass_fail_idx = 0, faild_counter = 0;

                        //using (System.IO.StreamWriter fs = new System.IO.StreamWriter(FILE_PATH))
                        //{
                        foreach (int[] err_vector in error_vectors)
                        {
                            for (int i = 0; i < err_vector.Length; i++)
                            {
                                symbol = err_vector[i].ToString();
                                if (symbol == "-2")
                                    symbol = "*";
                                msg1 += symbol;
                            }
                            fs.WriteLine("Error Vector = {0}", msg1);
                            ///////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            //int[] vec = new int[resilient_formula.get_number_of_int_nodes()];
                            ///////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            //var watch = System.Diagnostics.Stopwatch.StartNew();
                            foreach (int[] input_vector in input_vectors)
                            {
                                f_output = formula_tree_input.calculate_formula(input_vector, null);


                                F_output = resilient_formula.calculate_formula(input_vector, err_vector, true);

                                //F_output = resilient_formula.calculate_formula(input_vector, null, false);
                                //F_output = resilient_formula.calculate_formula(vec, null, false);

                                // print
                                for (int i = 0; i < input_vector.Length; i++)
                                {
                                    symbol = input_vector[i].ToString();
                                    if (symbol == "-2")
                                        symbol = "*";
                                    msg2 += symbol;
                                }

                                if (F_output == formula_result_type.ILLEGAL) continue;
                                if (f_output != F_output)
                                {
                                    pass_fail_idx = 1;
                                    faild_counter++;
                                }
                                else pass_fail_idx = 0;

                                fs.WriteLine("{0}    {1}    {2}    {3}", msg2 /*error_vector.ToString()*/, f_output, F_output, pass_fail_msg[pass_fail_idx]);
                                msg1 = "";
                                msg2 = "";
                            }
                            //watch.Stop();
                            //Console.WriteLine("Claculate formula for all inputs lasts = { 0 } ms", watch.ToString());
                        }
                        fs.WriteLine("Eps = {0}, Ro = {1}, Delta = {2}", globals.eps.ToString(), globals.error_fraction.ToString(), globals.delta.ToString());
                        fs.WriteLine("{0} failed out of {1}", faild_counter, ((input_vectors.Count) * (error_vectors.Count)).ToString());
                        fs.WriteLine("Resilient tree depth = {0}", resilient_formula.get_depth().ToString());
                        //}
                    }
                }
            } 
            return;
        }
        static void main_black_box(ref FormulaTree formula_tree_input, ref FormulaTree resilient_formula)
        {
            Globals globals = Globals.get_instance();
            formula_tree_input = TreeUtils.read_tree_from_file();
            ProtocolTree kw_tree = ProtocolTree.kw_trans(formula_tree_input);
            ProtocolTree egh_tree = ProtocolTree.egh(kw_tree);
            resilient_formula = ProtocolTree.reverse_kw(kw_tree, egh_tree);

        }
        public static void generate_err_vectors(FormulaTree resilient_formula, ref List<int[]> error_vectors, int num_of_vectors, int number_of_gates, double delta)
        {
            Globals globals;
            globals = Globals.get_instance();
            Node tmp_node = new Node();
            int total_err_per_path = 0, couter_err_per_path = 0, counter_total_error = 0;
            int alphabet_size = resilient_formula.get_num_of_children();
            globals.random_vectors(num_of_vectors, number_of_gates, 3, ref error_vectors);
            resilient_formula.set_leaves_array();
            List<Node> leaves_array = resilient_formula.get_leaves_array();
            Random rnd = new Random();

            foreach (int[] err_vector in error_vectors)
            {
                foreach (Node leaf in leaves_array)
                {
                    total_err_per_path = (int)(leaf.get_depth() * delta);
                    couter_err_per_path = 0;
                    tmp_node = leaf;
                    while (tmp_node.get_parent() != null)
                    {
                        tmp_node = tmp_node.get_parent();
                        //dummy_array[tmp_node.get_gate_idx()] = 1;
                        if ((err_vector[tmp_node.get_gate_idx()] != globals.NO_ERROR) && (err_vector[tmp_node.get_gate_idx()] != alphabet_size))
                        {
                            if (couter_err_per_path < total_err_per_path)
                            {
                                couter_err_per_path++;
                                // Verify that the error vector doesn't contain child index which aren't exist for the specific gate
                                while ((tmp_node.get_child(err_vector[tmp_node.get_gate_idx()]) == null))
                                {
                                    switch (err_vector[tmp_node.get_gate_idx()])
                                    {
                                        case 0:
                                            {
                                                err_vector[tmp_node.get_gate_idx()] = 1;
                                                break;
                                            }
                                        case 1:
                                            {
                                                err_vector[tmp_node.get_gate_idx()] = 2;
                                                break;
                                            }
                                        case 2:
                                            {
                                                err_vector[tmp_node.get_gate_idx()] = 0;
                                                break;
                                            }
                                    } // End if switch case
                                } // End of while
                            } // End of if
                            else  err_vector[tmp_node.get_gate_idx()] = globals.NO_ERROR;
                        }
                        // No error:
                        // --------
                        else if ((err_vector[tmp_node.get_gate_idx()] == alphabet_size))
                        {
                            err_vector[tmp_node.get_gate_idx()] = globals.NO_ERROR;
                        }
                    } // End of while - finish the traversal of a path
                } // foreach Node leaf
            
        
            // Print the number of error in the current vector
            for (int i = 0; i < err_vector.Length; i++)
            {
                if (err_vector[i] != globals.NO_ERROR)
                    counter_total_error++;
            }
            Console.WriteLine("{0} faulty gates out of {1} gates", counter_total_error.ToString(), number_of_gates);
            }
        } // foreach err_vector
    }
    
    public class Globals
    {
        private double Eps;
        private double Egh_immune;
        private double Error_fraction;
        private double Delta;
        private int NNO_ERROR; // used for generate error vectors in PartyNode

        private static Globals globals = null;

        private static bool flag_instance;
        public List<int[]> error_vectors_list;

        private Globals()
        {
          Eps = 1 / 6.0;
          Egh_immune = 1 / 3.0;
          Error_fraction = 1 / 3.0;
          Delta = 1 / 4.0;
          NNO_ERROR = -2;
          flag_instance = true;
          error_vectors_list = new List<int[]>();
        }

        public double eps
        {
            get { return Eps; }
            set { Eps = value; }
        }

        public double egh_immune
        {
            get { return Egh_immune; }
            set { Egh_immune = value; }
        }

        public double error_fraction
        {
            get { return Error_fraction; }
            set { Error_fraction = value; }
        }

        public double delta
        {
            get { return Delta; }
            set { Delta = value; }
        }

        public int NO_ERROR
        {
            get { return NNO_ERROR; }
            //set { NO_ERROR = value; }
        }

        public List<int[]> get_error_vectors_list()
        {
            return error_vectors_list;
        }

        public void clear_error_vector_list()
        {
            error_vectors_list = new List<int[]>(); 
        }

        public void set_ui_parameters(double eps_in, double error_fraction_in, double delta_in)
        {
            eps = eps_in;
            error_fraction = error_fraction_in;
            delta = delta_in;
        }

        public static Globals get_instance()
        {
            if (!flag_instance)
                globals = new Globals();
            return globals;
        }

        public double convert_string_fraction2double(string str_fraction)
        {
            int str_idx = 0;
            int numerator = 0, denominator = 0;
            
            while (str_fraction[str_idx] != '/')
            {
                numerator = numerator * 10 + (str_fraction[str_idx] - 48);
                str_idx++;
            }
            str_idx++; // for '/'
            while (str_idx < str_fraction.Length)
            {
                denominator = denominator * 10 + (str_fraction[str_idx] - 48);
                str_idx++;
            }

            return ((double)numerator/(double)denominator);
        }

        /* Globals::generate_alphabeth_vectors()
        * The recursive method generates all vectors with a d-ary alphabeth in size of a specific size. 
        * The vectors store in a data member of type List<int[]> 
        * 
        * [INPUT]:
        * error_vector     = The generated vector in each point
        * last_cell_in     = The last cell the function filled in the error_vector
        * vector_size      = depth of a PartyNode in the EGH tree
        * 
        * [OUTPUT]:
        * void
        * ******************************************************************************************************************* */
        public void generate_alphabeth_vectors(int[] error_vector, int last_cell_in, int vector_size, int alphabeth_size = 2)
        {
            // Stop condition
            if (last_cell_in == (vector_size))
            {
                int[] tmp_vector = (int[])error_vector.Clone();
                error_vectors_list.Add(tmp_vector);
                return;
            }

            for (int i = 0; i < alphabeth_size; i++)
            {
                error_vector[last_cell_in] = i;
                generate_alphabeth_vectors(error_vector, last_cell_in + 1, vector_size, alphabeth_size);

            }
        } // End of "generate_alphabeth_vectors"

        /* PartyNode::limit_num_of_errors()
         * This method goes over all binary vectors which define errors (1 = error , 0 = no error)
         * and for each vector checks: if the number of errors (cells that contains 1)
         * are bigger than error limit -> dispose this vector.
         * 
         * [INPUT]:
         * optional_binary_vectors = list of all binary vector with size n (2^n vectors)
         * 
         * [OUTPUT]:
         * limit_num_of_errors = list of legal error vectors - don't contain more than ERROR_FRACTION errors.
         * ********************************************************************************************************************/
        public List<int[]> limit_num_of_errors(List<int[]> optional_binary_vectors, double error_fraction)
        {
            int vector_sum = 0;
            int N = optional_binary_vectors[0].Length;
            int max_num_of_errors = (int)(N * error_fraction);
            bool flag_illegal_error = false;

            List<int[]> binary_vectors_to_return = new List<int[]>();

            foreach (int[] error_vector in optional_binary_vectors)
            {
                vector_sum = 0;
                for (int cell_idx = 0; cell_idx < error_vector.Length; cell_idx++)
                {
                    vector_sum += error_vector[cell_idx];
                    if (vector_sum > max_num_of_errors)
                    {
                        // Removes illegal vectors
                        flag_illegal_error = true;
                        break;
                    }
                    if (error_vector[cell_idx] == 0)
                        // (cells == 0) --> indicates that there is no error
                        error_vector[cell_idx] = globals.NO_ERROR;
                }
                if (flag_illegal_error == false)
                    binary_vectors_to_return.Add(error_vector);

            }
            return binary_vectors_to_return;
        } // End of "limit_num_of_errors"

        /* PartyNode::generate_legel_vectors()
         * This method generates all legal vectors (including some fraction of errors according to user's constraint)
         * which define path from root to a specified node.
         * 
         * [INPUT]:
         * legal_error_vectors = list of binary vectors (1 = error, 0 = no error), which contains only
         * vectors with a legal num of errors (restrictes amount of errors according to user's definition).
         * real_egh_path       = list of child indices which define a path from root to node in EGH tree.
         * 
         * [OUTPUT]:
         * error_vectors_per_node = List of error vectors which suitable to a specified node. each one of 
         * those vectors defines a path from the root to node including some errors. (contain all egal options).
         * ********************************************************************************************************************/
        public List<int[]> generate_legel_vectors(List<int[]> legal_error_vectors, List<int[]> real_egh_path)
        {
            List<int[]> error_vectors_per_node = new List<int[]>();
            int error_vector_length = legal_error_vectors[0].Length;

            foreach (int[] curr_path in real_egh_path)
            {
                foreach (int[] error_vector in legal_error_vectors)
                {
                    int[] tmp_legal_error_vector = new int[error_vector_length];
                    for (int cell_idx = 0; cell_idx < error_vector_length; cell_idx++)
                    {
                        if (error_vector[cell_idx] == (int)globals.NO_ERROR)
                        {
                            tmp_legal_error_vector[cell_idx] = globals.NO_ERROR;
                        }
                        else // ERROR
                        {
                            tmp_legal_error_vector[cell_idx] = curr_path[cell_idx];
                        }

                    }
                    error_vectors_per_node.Add(tmp_legal_error_vector);
                }
            }

            return error_vectors_per_node;
        } // End of "generate_leagel_vectors"

        public void random_vectors(int num_of_vectors, int vector_length, int max_value, ref List<int[]> randomized_vectors_out)
        {
            Random rnd = new Random();
            //int[] input_vector = new int[vector_length];

            while (randomized_vectors_out.Count /* count starts from 1*/ < num_of_vectors)
            {
                int[] input_vector = new int[vector_length];
                for (int i = 0; i < vector_length; i++)
                {
                    input_vector[i] = rnd.Next(0, max_value + 1); // Random number from 0 to max_value
                }
                randomized_vectors_out.Add(input_vector);
            }
        }
    }
}

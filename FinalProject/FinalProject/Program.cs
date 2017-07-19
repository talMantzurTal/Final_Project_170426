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

    class Program
    {
#if PRINT_F
        const string FILE_PATH = @"C:\Users\mantz\Documents\GitHub\Final_Project_170202\Final_Project_170426\simulation_results.txt";
#endif

        static void Main(string[] args)
        {
            Simulation simulation = Simulation.get_instance();
            simulation.ui_input_parameters();
            
            FormulaTree formula_tree_input = new FormulaTree(), resilient_formula = new FormulaTree();
            main_black_box(ref formula_tree_input, ref resilient_formula);

            List<int[]> input_vectors = new List<int[]>();
            List<int[]> error_vectors = new List<int[]>();
            simulation.simulate(resilient_formula.get_number_of_int_nodes(), formula_tree_input.get_number_of_leaves(), input_vectors, error_vectors);

            bool f_output, F_output;
            string[] pass_fail_msg = { "Pass", "Fail" };
            int pass_fail_idx = 0;
            foreach (int[] error_vector in error_vectors)
            {
                f_output = formula_tree_input.calculate_formula(input_vectors[0], null);
                F_output = resilient_formula.calculate_formula(input_vectors[0], error_vector, true);
                
                    // print
                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(FILE_PATH))
                {
                    if (f_output != F_output) pass_fail_idx = 1;
                    fs.WriteLine("{0}    {1}    {2}    {3}    {4}", input_vectors[0], error_vector, f_output, F_output, pass_fail_msg[pass_fail_idx]);
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

            /*
            String input_string = "10001000";
            List<int> error_vector = new List<int>();
            for (int i = 0; i < 7; i++)
                error_vector.Add(globals.NO_ERROR);
            formula_tree_input.calculate_formula(input_string, error_vector);
            */

            //List<int> error_vector = new List<int>();
            /*
            for (int i = 0; i < 55; i++)
                error_vector.Add(globals.NO_ERROR);
            resilient_formula.calculate_formula(input_string, error_vector);*/
        }

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
                generate_alphabeth_vectors(error_vector, last_cell_in + 1, vector_size);

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
         * those vectors defines a path from root to node including some errors. (contain all egal options).
         * ********************************************************************************************************************/
        public List<int[]> generate_legel_vectors(List<int[]> legal_error_vectors, List<int> real_egh_path)
        {
            List<int[]> error_vectors_per_node = new List<int[]>();
            int error_vector_length = legal_error_vectors[0].Length;

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
                        tmp_legal_error_vector[cell_idx] = real_egh_path[cell_idx];
                    }

                }
                error_vectors_per_node.Add(tmp_legal_error_vector);
            }

            return error_vectors_per_node;
        } // End of "generate_leagel_vectors"
    }
}

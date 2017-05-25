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
    class Program
    {
        static void Main(string[] args)
        {
            FormulaTree formula_tree_input;

            formula_tree_input = TreeUtils.read_tree_from_file();
            ProtocolTree kw_tree = ProtocolTree.kw_trans(formula_tree_input);
            ProtocolTree.egh(kw_tree);
            
            // Check the vector generator
            int vector_size = 2;
            PartyNode node = (PartyNode)kw_tree.get_root(); // TODO
            int[] error_vector_to_generate = new int[vector_size];
#if list_wrong
            int num_of_error_vectors = (int)Math.Pow(node.get_num_of_children() + 1, 2);
            int error_vector_length = node.get_depth();
            int[][] error_vectors_list = new int[num_of_error_vectors][];

            for (int i = 0; i < num_of_error_vectors; i++)
                error_vectors_list[i] = new int[error_vector_length];
#endif
            node.generate_alphabeth_vectors(error_vector_to_generate, 0, vector_size);

            return;
        }
    }

    public static class Globals
    {
        public const double eps = 1 / 6.0;
        public const double egh_immune = 1 / 3.0;
        public const int NO_ERROR = -2; // used for generate error vectors in PartyNode
    }
}

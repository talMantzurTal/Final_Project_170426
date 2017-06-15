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
        static void Main(string[] args)
        {
            FormulaTree formula_tree_input;

            formula_tree_input = TreeUtils.read_tree_from_file();
            ProtocolTree kw_tree = ProtocolTree.kw_trans(formula_tree_input);
            ProtocolTree egh_tree = ProtocolTree.egh(kw_tree);

            // Check the vector generator
            /* int vector_size = 2;
            PartyNode node = (PartyNode)kw_tree.get_root(); // TODO
            int[] error_vector_to_generate = new int[vector_size];
            node.generate_alphabeth_vectors(error_vector_to_generate, 0, vector_size);*/

            FormulaTree resilient_formula = ProtocolTree.reverse_kw(kw_tree, egh_tree);

            return;
        }
    }

    public static class Globals
    {
        public const double eps = 1 / 6.0;
        public const double egh_immune = 1 / 3.0;
        public const double error_fraction = 1 / 3.0;
        public const int NO_ERROR = -2; // used for generate error vectors in PartyNode
    }
}

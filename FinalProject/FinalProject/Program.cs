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
            String input_string = "10001000";
            List<int> error_vector = new List<int>();
            for (int i = 0; i < 7; i++)
                error_vector.Add(Globals.NO_ERROR);
            formula_tree_input.calculate_formula(input_string, error_vector);

            ProtocolTree kw_tree = ProtocolTree.kw_trans(formula_tree_input);
            ProtocolTree egh_tree = ProtocolTree.egh(kw_tree);

            FormulaTree resilient_formula = ProtocolTree.reverse_kw(kw_tree, egh_tree);
            //List<int> error_vector = new List<int>();
            for (int i = 0; i < 55; i++)
                error_vector.Add(Globals.NO_ERROR);
            resilient_formula.calculate_formula(input_string, error_vector);

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

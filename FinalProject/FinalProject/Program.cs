﻿using System;
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

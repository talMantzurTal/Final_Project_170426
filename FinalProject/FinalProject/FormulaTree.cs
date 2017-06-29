﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject;

namespace FinalProject
{
    class FormulaTree : Tree
    {
        private int m_num_of_variables;

        public FormulaTree()
            : base()
        {
            //Console.WriteLine("FormulaTree defualt c'tor");
            m_num_of_variables = 0;
            m_root = null;
            m_depth = 0;
            m_num_of_children = 0;
        }

        // TAL: public FormulaTree(int num_of_children, LogicNode root_node) :
        // Takes num_of_children from the node that was sent and not as an input
        public FormulaTree(LogicNode root_node) :
            base((Node)root_node)
        {
            //Console.WriteLine("FormulaTree c'tor");
            m_num_of_variables = 0;
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = root_node.get_num_of_children();
        }

        /* FormulaTree::add_child()
         * The method adds a child from type LogicNode to  specific LogicNode in the tree (shallow copy)
         * IN: name = name of the new LogicNode
         * IN: type = type of the new LogicNode: AND or OR (from enum node_type)
         * OUT: void
         * ********************************************************************************************** */
        public void add_child(string name, node_type type, LogicNode parent)
        {
            int new_node_depth = parent.get_depth() + 1;
            Node node_to_add = new LogicNode(name, type, new_node_depth, m_num_of_children, parent);
            bool result = parent.add_child((Node)node_to_add);
            // The tree depth is the longest path in the tree
            if (new_node_depth > m_depth) m_depth = new_node_depth;
        }

        // TAL
        // TODO: implementation
        /* FormulaTree::calculate_formula()
         * The method calculate the formula implemented as a FormulaTree with the input to the formula
         * IN: formula_input = the input to the formula that should be calculated
         * ******************************************************************************************** */
        public bool calculate_formula(string formula_input_string, List<int> error_vector)
        {
            Boolean[] bits = new Boolean[formula_input_string.Length];

            for (int i = 0; i< formula_input_string.Length; i++)
            {
                // Fill bits with the boolean values of formula input:
                // true for formula_input[i] == 1
                // false for formula_input[i] == 0
                bits[i] = (formula_input_string[i] == '1');
            }

            BitArray formula_input_bits = new BitArray(bits);
            //List<Node> leave_array = new List<Node>;
            this.set_leaves_array();
            LogicNode tmp_l_node = new LogicNode();
            LogicNode tmp_l_child_node = new LogicNode();
            LogicNode tmp_l_node_2 = new LogicNode();
            int idx = 0;

            foreach ( Node node in m_leaves_array )
            {
                tmp_l_node = (LogicNode)node;
                tmp_l_node.set_value(formula_input_bits[tmp_l_node.get_leaf_idx()]);
                //idx++;
            }

            node_type type;
            Node[] node_children;
            Boolean value;
            while ( this.get_leaves_array().Count != 1 )
            {
                this.update_leaves_array();
                foreach (Node node in m_leaves_array)
                {
                    tmp_l_node = (LogicNode)node;
                    type = tmp_l_node.get_type();
                    //
                    if (error_vector[tmp_l_node.get_gate_idx()] != Globals.NO_ERROR)
                    {
                        // Check if current node has a child with idx = 2 (avoid null reference)
                        tmp_l_node_2 = (LogicNode)tmp_l_node.get_child(error_vector[tmp_l_node.get_gate_idx()]);
                        if (tmp_l_node_2 == null) return false; // the error_vector is illegal for current tree
                        value = tmp_l_node_2.get_value();
                    }
                    else // No error
                    { 
                        node_children = tmp_l_node.get_children();
                        if (type == node_type.AND)
                        {
                            value = true;
                            idx = 0;
                            while ((idx < m_num_of_children) && (node_children[idx] != null))
                            {
                                tmp_l_child_node = (LogicNode)node_children[idx];
                                value = value && tmp_l_child_node.get_value();
                                idx++;
                            }
                        }
                        else
                        {
                            value = false;
                            idx = 0;
                            while ((idx < m_num_of_children) && (node_children[idx] != null))
                            {
                                tmp_l_child_node = (LogicNode)node_children[idx];
                                value = value || tmp_l_child_node.get_value();
                                idx++;
                            }
                        }
                    }
                    // Set the value of current node
                    tmp_l_node.set_value(value);
                }
            }
            return true; // the error_vector is legal for current tree
        }

        public Tree deep_copy() //vered!!!
        {
            /* perform deep copy of the root and use this copy in order to create a sub FormulaTree */
            Node cloned_root = m_root.deep_copy();
            LogicNode cloned_formula_root = new LogicNode(cloned_root);
            FormulaTree cloned_formula_tree = new FormulaTree(cloned_formula_root);
            return (this.deep_copy(/*cloned_formula_root,*/cloned_formula_tree));
        }

#if v
            Node curr_node = new Node();
            curr_node = m_root;
            int leaf_idx = 0;
            int curr_last_child_idx = curr_node.get_last_child_idx();
            // Number the leaves while the parent isn't the root or it is the root but the program haven't checked all root's children yet
            while ((curr_node.get_name() != m_root.get_name()) || ((curr_last_child_idx + 1) < m_num_of_children))
            {
                curr_node.inc_last_child_idx();
                curr_last_child_idx = curr_node.get_last_child_idx();
                Node tmp_head = new Node();
                tmp_head = curr_node.get_child(curr_last_child_idx);
                tmp_head.inc_last_child_idx();
                int tmp_last_child_idx = tmp_head.get_last_child_idx();
                // If a node with depth 1 is a leaf, number it and don't try to get to it's children
                if (tmp_head.get_child(0) == null)
                {
                    tmp_head.set_leaf_idx(leaf_idx);
                    leaf_idx++;
                }
                else
                {
                    while (tmp_last_child_idx < m_num_of_children)
                    {
                        // If the grandchild is null, the child is a leaf
                        while (tmp_head.get_child(tmp_last_child_idx).get_child(0) != null)
                        {
                            tmp_head = tmp_head.get_child(tmp_last_child_idx);
                        }
                        // Reached a leaf ->  number it
                        tmp_head.get_child(tmp_last_child_idx).set_leaf_idx(leaf_idx);
                        leaf_idx++;
                        tmp_head.inc_last_child_idx();
                        tmp_last_child_idx = tmp_head.get_last_child_idx();
                    }
                }
                curr_node = tmp_head.get_parent();
            }
            m_num_of_variables = leaf_idx;
        }


    }
}
#endif
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Isam.Esent.Interop;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class ProtocolTree : Tree
    {
        Globals globals = Globals.get_instance();

        // C'TORS
        public ProtocolTree() :
            base()
        {
            
        }
        public ProtocolTree(PartyNode root_node) :
            base((Node)root_node)
        {

        }
        public ProtocolTree(Tree tree) :
            base(tree)
        {

        }

        // GETTERS


        // SETTERS 


        // METHODS:
        public Tree deep_copy()
        {
            /* perform deep copy of the root and use this copy in order to create a sub ProtocolTree */
            Node cloned_root = m_root.deep_copy();
            ProtocolTree cloned_protocol_tree = new ProtocolTree((PartyNode)cloned_root);
            return (this.deep_copy(cloned_protocol_tree));
        }

        /* ProtocolTree::kw_trans(FormulaTree f_tree)
         * This method implements KW transformation: from a formula tree with nodes contain OR and AND gates
         * to a protocol tree with party nodes: Alice and Bob nodes. 
         * [INPUT]: 
         * f_tree = Formula tree - a boolean formula which contains AND,OR gates implemented as a binarey tree.
         * [OUTPUT]:
         * p_tree = Protocol tree - an implementation of communication protocol contains two parties: Alice and Bob
         * [Algoritm]: Iterate on given formula tree and for each Logic node, create a corresponding party node and 
         * add it to KW tree (a new protocol tree).
         * For each logic node in formula tree perform: if the type is an AND gate, create a corresponding party node with
         * type ALICE, else(OR gate) create one with type BOB.
         * assign a pointer from each party node to its corresponding formula node (in order to calculate the logic node
         * value given an input [x,y]).
         ************************************************************************************************************/
        public static ProtocolTree kw_trans(FormulaTree f_tree)
        {
            // Get formula tree root, and create a protocol tree from this f_root
            Node f_root = new LogicNode();
            f_root = f_tree.get_root();
            string root_name = f_root.get_name();
            node_type root_type;
            if (f_root.get_type() == node_type.AND)
            {
                root_type = node_type.ALICE;
            }
            else
            {
                root_type = node_type.BOB;
            }
            int root_depth = f_root.get_depth();
            int root_num_of_children = f_root.get_num_of_children();
            int p_num_of_children = root_num_of_children + 1; // Protocol tree with number_of_children + 1, for the EGH
            PartyNode p_root = new PartyNode(root_name, root_type, root_depth, p_num_of_children, /* parent = */null, f_root);

            // Create protocol tree with p_root as a root
            ProtocolTree p_tree = new ProtocolTree(p_root);

            // pre-order traversal on the formula tree
            Stack<Node> s = new Stack<Node>();
            s.Push(f_root); // Push f_root to stack in order to traverse the formula tree
            int[] x = { 1, 0, 0, 1 };
            // f_root.calculate_value(x);
            while (s.Count > 0)
            {
                var n = s.Pop();   // LogicNode
                // Do Action
                PartyNode p_tmp_node = new PartyNode((Node)n);
                // Find node's parent and add the node to the protocol tree
                if (!p_tmp_node.get_name().Equals(f_root.get_name()))
                {
                    Node parent_node = Tree.preOrder(p_root, n.get_parent().get_name(), true);
                    parent_node.add_child(p_tmp_node);
                }

                foreach (var child in n.get_children().ToArray().Reverse())
                {
                    if (child != null)
                    {
                        s.Push(child);
                    }
                }
            }
            /* After creating the KW tree, calculate its depth (by using set_depth function) */
            p_tree.set_depth();
            return p_tree;
        }

        /* ProtocolTree::egh(ProtocolTree kw_tree)
         * This method implements EGH coding scheme: conver a communication protocol tree 
         * to a resilient protocol which can withstand up to fraction of 1/3 - eps errors.
         * [INPUT]:
         * kw_tree = a communication protocol tree, which was built from a Formula tree using KW transformation.
         * [OUTPUT]:
         * egh_tree = a protocol tree which is resilient to a fraction of 1/3 - eps errors.
         * [Algoritm]:
         * for each node, set its last child (the error child) as the sub tree induced    
         * by the node's grandparent - (each corruption causes at most three recovery rounds), 
         * "going back" from the node to its grandparent equals to those recovery rounds.      
         **********************************************************************************************************/
        public static ProtocolTree egh(ProtocolTree kw_tree)
        {
            // Deep copy the kw_tree in order to use it for reacability
            ProtocolTree egh_tree = (ProtocolTree)kw_tree.deep_copy();
            PartyNode tmp_node_egh = new PartyNode(); // TAL: changed from Node to PartyNode in order to use the method get_protocol_node_reference
            int num_of_children = kw_tree.get_root().get_num_of_children();
            int num_of_children_idx = num_of_children - 1;
            double depth_strech_param = (egh_tree.globals.egh_immune / egh_tree.globals.eps);
            int egh_desired_depth = (int)Math.Ceiling(depth_strech_param * kw_tree.get_depth());
            int child_idx = 0;
            

            // Expand the EGH tree to the target depth (egh_desired_depth) by zero padding the EGH tree.
            // After that expantion, traverse the tree and change the nodes to their values after egh coding.
            egh_tree.zero_padding(egh_desired_depth);

            foreach (PartyNode egh_node in egh_tree)
            {
                if ((egh_node.get_parent() != null) && (egh_node.get_parent().get_name() == "0")) continue;
                //if ((egh_node.get_depth() * 2 - 2) > egh_desired_depth) continue;
                Node sub_tree_root_kw = new PartyNode();
                // List <Node> sub_formula_arr = new List<Node>();
                if (egh_node.get_depth() < 2)
                {
                    /* The node's grandparent is the root itself,               */
                    /* set the current node's error child as the root's subtree */
                    //tmp_node_egh = egh_node;
                    sub_tree_root_kw = egh_node.get_protocol_node_reference();
                }
                else
                {
                    /* Replace the current node last child with the current node's grandparent sub_tree */
                    tmp_node_egh = (PartyNode)egh_node.get_parent(copy_flags.SHALLOW).get_parent(copy_flags.SHALLOW);
                    sub_tree_root_kw = tmp_node_egh.get_protocol_node_reference();
                }
                if (sub_tree_root_kw == null) continue;
                //if (sub_tree_root_kw.get_if_leaf()) continue;
                // Replace the last child with it's corresponding sub_tree (deep copy) with the sub_tree_root_kw as a root
                if ((egh_node.get_child(num_of_children_idx) != null) && (egh_node.get_child(num_of_children_idx).get_name() == "0") )
                {
                    
                    ProtocolTree sub_tree_to_copy = new ProtocolTree((PartyNode)sub_tree_root_kw);
                    sub_tree_to_copy.set_depth();

                    // Verify that the new EGH tree depth is less than the egh_desired_depth 
                    
                    if (sub_tree_to_copy.get_depth() + egh_node.get_depth() + 1 < egh_desired_depth) // TAL + 1
                    {
                        ProtocolTree cloned_sub_tree = new ProtocolTree(sub_tree_to_copy.deep_copy());
                        cloned_sub_tree.set_depth( sub_tree_to_copy.get_depth() );
                       // child_idx = egh_node.get_last_child_idx() + 1;
                       // while ( (child_idx < egh_node.get_num_of_children() ) && (egh_node.get_child(child_idx).get_name() != "0"))
                       // {
                       //    egh_node.inc_last_child_idx();
                       //     child_idx = egh_node.get_last_child_idx();
                       // }
                        egh_tree.copy_sub_tree((PartyNode)egh_node.get_child(/*TAL child_idx*/ num_of_children_idx), cloned_sub_tree);
                        
                    
                        //foreach (PartyNode sub_node in cloned_sub_tree)
                        ///////////////
                        //egh_node.add_child(cloned_sub_tree.get_root(), num_of_children_idx, copy_flags.SHALLOW);
                    }
                    //egh_node.get_child(num_of_children_idx).set_node(sub_tree_root_kw);
                 }
            }

            //TreeUtils.write_tree_to_file(egh_tree);
            // Perform Zero padding
            egh_tree.set_depth();
            egh_tree.init_last_child_idx();
            return egh_tree;

        }

        /* ProtocolTree::reverse_kw()
         * This method implements reverse KW transformation (from a protocol tree to a resilient formula, 
         * resilient to a Global.error_fraction - eps errors in each path input-output)
         * [INPUT]:
         * kw_tree = a communication protocol tree, which was built from a Formula tree using KW transformation.
         * egh_tree = a communication protocol tree, which was built from a Formula tree. The tree is not resilient 
         *            to errors.
         * [OUTPUT]:
         * resilient_formula_Tree = a formula tree specified a resilient formula Global.error_fraction - eps errors 
         *                          in each path input-output
         * [Algoritm]:
         * for each node (except from the root- always resilient), create the leagal error vectors and traverse the ]
         * kw_tree. If aftre that traversion, it reached to a leaf, the node is reachale.
         **********************************************************************************************************/
        public static FormulaTree reverse_kw(ProtocolTree kw_tree, ProtocolTree egh_tree)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            PartyNode curr_node = null;
            PartyNode curr_reachable_node = null;
            int zero_leaves_counter = 0;
            int num_of_children = egh_tree.get_root().get_num_of_children();
            // Set the root's rewind child as unreachable
            ProtocolTree subtree_root = new ProtocolTree((PartyNode)egh_tree.get_root().get_child(num_of_children - 1));
            subtree_root.set_subtree_unreachable();

            // Set leaves array
            egh_tree.set_leaves_array();
            
            while (curr_node != egh_tree.get_root())
            {
                if (egh_tree.get_leaves_array().Count == 0) //list is empty - all nodes are reachable!
                    break;
                zero_leaves_counter = 0;
                /* set zero nodes as UNREACHABLE and look for the first node in leaves array which isn't 
                 * zero in order to check if it's reachable */
                
                for (int idx = 0; idx < egh_tree.get_leaves_array().Count; idx++)
                {
                    curr_node = (PartyNode)egh_tree.get_leaves_array_cell(idx);
                    if (curr_node.get_name() == "0")
                    {
                        curr_node.set_reachable(reachable_type.UNREACHABLE);
                        zero_leaves_counter++;
                    }
                }

                /* if current leaves array contains only 0 leaves,continue to upper level in tree */
                if (zero_leaves_counter == egh_tree.get_leaves_array().Count - 1)
                {
                    egh_tree.update_leaves_array();
                    curr_node = (PartyNode)egh_tree.get_leaves_array_cell(0);
                    continue;
                }
                // Generate error binary vectors with size curr_node.depth() and limit the legal vectors by the desired 
                // error fraction
                int[] error_vector_to_generate = new int[curr_node.get_depth()];
                // TAL curr_node.generate_alphabeth_vectors(error_vector_to_generate, 0, error_vector_to_generate.Length, 2);
                kw_tree.globals.clear_error_vector_list();
                kw_tree.globals.generate_alphabeth_vectors(error_vector_to_generate, 0, error_vector_to_generate.Length, 2);
                List<int[]> error_binary_vectors = kw_tree.globals.get_error_vectors_list();
                error_binary_vectors = kw_tree.globals.limit_num_of_errors(error_binary_vectors, kw_tree.globals.error_fraction);

                // Foreach node in m_leaves_array:
                // 1. Get it's path from root in egh_tree
                // 2. Generate leagl error vectors from "error_binary_vectors"
                // 3. Check if the node is reachable
                List<Node> leaves_array = egh_tree.get_leaves_array();
                PartyNode tmp_p_node = new PartyNode();
                foreach (PartyNode node in leaves_array)
                {
                    tmp_p_node = (PartyNode)node;
                    if (tmp_p_node.get_name() == "0") continue;
                    if ((tmp_p_node.get_reachable()) == reachable_type.NA)
                    {
                        List<int[]> real_path_to_node = new List<int[]>();
                        real_path_to_node.Add(node.get_path_from_root().ToArray());
                        List<int[]> legal_vectors_per_node = kw_tree.globals.generate_legel_vectors(error_binary_vectors, real_path_to_node);
                        node.is_reachable(kw_tree, legal_vectors_per_node, real_path_to_node[0]);
                    }
                }

                egh_tree.update_leaves_array();
                curr_node = (PartyNode)egh_tree.get_leaves_array_cell(0);
            }
            FormulaTree resilient_formula = egh_tree.convert2FormulaTree();
            resilient_formula.number_gates_preorder();
            resilient_formula.set_depth();
            return resilient_formula;
            //return null; // TODO: change
        }

        /* ProtocolTree::zero_padding(int tree_depth_after_padding)
         * This method perform zerro pedding of tree in order to create a complete tree.
         * [INPUT]:
         * tree_depth_after_padding = tree's desired depth after zero padding.
         * [OUTPUT]:
         * None
         * [Algoritm]:
         * Go over all tree's nodes and for each node do:                                 
         * If the node depth is less than the desired tree depth and at least one of it's childrens               
         * is null, set this children with a zero subtree with depth = (egh_desired_depth - current node's depth)
         * Note that in order to create a zero subtree, this function calls create_zero_sub_tree.                   
         **********************************************************************************************************/
        void zero_padding(int tree_depth_after_padding)
        {
            ProtocolTree zero_sub_tree = new ProtocolTree();
            foreach (Node node in this)
            {
                for (int child_idx = 0; child_idx < m_num_of_children; child_idx++)
                {
                    if ((node.get_depth() < tree_depth_after_padding) && (node.get_child(child_idx) == null))
                    {
                        zero_sub_tree = (ProtocolTree)create_zero_sub_tree(tree_depth_after_padding, node.get_type(), node.get_depth() + 1);
                        node.add_child(zero_sub_tree.get_root());
                    }
                }
            }
            //TreeUtils.write_tree_to_file(this);
        } // End of "zero_padding"

        /* ProtocolTree::create_zero_sub_tree()
         * This method create a ternary tree of zeroes in a specific depth
         * [INPUT]:
         * tree_depth_after_padding = tree's desired depth after zero padding.
         * type_parent              = a node_type variable. Indicates the parent's type in order to build a tree with an 
         *                            alternating type order
         * start_depth              = a desired tree depth.
         * [OUTPUT]:
         * Tree of zeroes. Each node in the tree has a type of ALICE/BOB in an alternating order.                  
         **********************************************************************************************************/
        public Tree create_zero_sub_tree(int tree_depth_after_padding, node_type type_parent = node_type.BOB, int start_depth = 0)
        {
            string name = "0";
            node_type type;
            // Set the root type of the zero sub tree different of it's parent
            if (type_parent == node_type.ALICE) type = node_type.BOB;
            else if (type_parent == node_type.BOB) type = node_type.ALICE;
            else throw new System.ArgumentException("create_zero_sub_tree1: Illegal type for party node", type_parent.ToString());

            int cur_depth_child = start_depth;
            PartyNode curr_node_parent = new PartyNode();
            PartyNode root_zero_tree = new PartyNode(name, type, cur_depth_child, this.m_num_of_children);

            curr_node_parent = root_zero_tree;

            // Build a zero sub tree in a specific depth (tree_depth_after_padding - start_depth).
            // Preorder 
            if (curr_node_parent.get_depth() != tree_depth_after_padding)
            {
                // Keep add children while the root hasn't finished to add children.
                while (((curr_node_parent == root_zero_tree) && (root_zero_tree.get_last_child_idx() < (m_num_of_children - 1))) || (curr_node_parent != root_zero_tree))
                {
                    // Add children to an internal nodes (curr_node)
                    while ((curr_node_parent.get_depth() < tree_depth_after_padding) && (curr_node_parent.get_last_child_idx() < (m_num_of_children - 1)))
                    {
                        // Type
                        if (curr_node_parent.get_type() == node_type.ALICE) type = node_type.BOB;
                        else if (curr_node_parent.get_type() == node_type.BOB) type = node_type.ALICE;
                        else throw new System.ArgumentException("create_zero_sub_tree2: Illegal type for party node {0}", curr_node_parent.get_type().ToString());
                        // Depth
                        cur_depth_child = curr_node_parent.get_depth() + 1;

                        // Create a new PartyNode with the above parameters
                        PartyNode child_to_add = new PartyNode(name, type, cur_depth_child, this.m_num_of_children);

                        // Add the new PartyNode as a child to curr_node
                        child_to_add.set_is_zero_padding(true);
                        curr_node_parent.inc_last_child_idx();
                        curr_node_parent.add_child(child_to_add, curr_node_parent.get_last_child_idx());
                        curr_node_parent = child_to_add;

                    }
                    // If curr_node != root, move upstream
                    if (curr_node_parent.get_parent() != null)
                    {
                        curr_node_parent = (PartyNode)curr_node_parent.get_parent();
                    }
                    // If curr_node == root, move orizontally to the next child
                    else
                    {
                        curr_node_parent = (PartyNode)curr_node_parent.get_child(curr_node_parent.get_last_child_idx());
                    }
                }
            }
            ProtocolTree zero_sub_tree = new ProtocolTree(root_zero_tree);
            //TreeUtils.write_tree_to_file(zero_sub_tree);
            this.init_last_child_idx();
            return zero_sub_tree;
        } // End of "create_zero_sub_tree"

        /* ProtocolTree::copy_leaves_array_from_idx()
         * This method copies m_leaves_array to itself starting an intput index to the end.
         * [INPUT]:
         * start_idx = the first index we want to copy
         * [OUTPUT]:
         * void               
         **********************************************************************************************************/
        void copy_leaves_array_from_idx_to_end(int start_idx = 0)
        {
            List<Node> leave_array_cpy = new List<Node>();
            for (int idx = start_idx; idx < this.get_leaves_array().Count; idx++)
            {
                leave_array_cpy.Add(m_leaves_array[idx]);
            }
            m_leaves_array = leave_array_cpy;
        } // End of "copy_leaves_array_from_idx_to_end"

        /* ProtocolTree::copy_sub_tree()
         * This method copies sub_tree2copy to zero_sub_tree_root. The method iterates both trees at the same time.
         * [INPUT]:
         * zero_sub_tree_root = A tree with zeroes. after the method, this tree contains all sub_tree2copy content.
         * sub_tree2copy      = Tree with content. We want to copy the content from it.
         * [OUTPUT]:
         * void               
         **********************************************************************************************************/
        public void copy_sub_tree(PartyNode zero_sub_tree_root, ProtocolTree sub_tree2copy) 
        {
           
            PartyNode curr_parent_zero_sub_tree = new PartyNode();
            curr_parent_zero_sub_tree = zero_sub_tree_root;

            PartyNode curr_parent_sub_tree2copy = (PartyNode)sub_tree2copy.get_root();
            int sub_tree_depth_after_copying = curr_parent_zero_sub_tree.get_depth() + sub_tree2copy.get_depth();

            PartyNode tmp_node = new PartyNode();

            // Build a zero sub tree in a specific depth (tree_depth_after_padding - start_depth).
            // Preorder 
            // Keep add children while the root hasn't finished to add children.
            while (((curr_parent_zero_sub_tree == zero_sub_tree_root) && (zero_sub_tree_root.get_last_child_idx() < (m_num_of_children - 1))) || (curr_parent_zero_sub_tree != zero_sub_tree_root))
            {
                // Add children to an internal nodes (curr_node)
                while ((curr_parent_zero_sub_tree.get_depth() <= sub_tree_depth_after_copying) && (curr_parent_zero_sub_tree.get_last_child_idx() < (m_num_of_children - 1)))
                {
                    if (!curr_parent_sub_tree2copy.get_is_copied())
                    {
                        curr_parent_zero_sub_tree.set_node(curr_parent_sub_tree2copy);
                        curr_parent_sub_tree2copy.set_is_copied(true);
                    }
                    // curr_parent_zero_sub_tree.set_depth(curr_parent_zero_sub_tree.get_depth() + 1);
                    // Add the new PartyNode as a child to curr_node

                    tmp_node = (PartyNode)curr_parent_sub_tree2copy.get_child(curr_parent_sub_tree2copy.get_last_child_idx() + 1);
                    curr_parent_sub_tree2copy.inc_last_child_idx();
                    curr_parent_zero_sub_tree.inc_last_child_idx();
                    if (tmp_node == null) break;
                    
                    curr_parent_sub_tree2copy = tmp_node;
                    
                    curr_parent_zero_sub_tree = (PartyNode)curr_parent_zero_sub_tree.get_child(curr_parent_zero_sub_tree.get_last_child_idx());
                    
                    
                }
                // If curr_node != root, move upstream
                if (curr_parent_sub_tree2copy.get_parent() != null)
                {
                    curr_parent_zero_sub_tree = (PartyNode)curr_parent_zero_sub_tree.get_parent();
                    curr_parent_sub_tree2copy = (PartyNode)curr_parent_sub_tree2copy.get_parent();
                }
                // If curr_node == root, move orizontally to the next child
                else if (curr_parent_sub_tree2copy.get_last_child_idx() < m_num_of_children-1 )
                {
                    // If curr_node == root and doesn't have children --> is a leaf in the tree
                    if (curr_parent_sub_tree2copy.get_child(0) == null) break;
                    curr_parent_zero_sub_tree = (PartyNode)curr_parent_zero_sub_tree.get_child(curr_parent_zero_sub_tree.get_last_child_idx());
                    curr_parent_sub_tree2copy = (PartyNode)curr_parent_sub_tree2copy.get_child(curr_parent_sub_tree2copy.get_last_child_idx());
                }
                else
                {
                    break;
                }
            }
            //TreeUtils.write_tree_to_file(zero_sub_tree);
            ProtocolTree p_tmp_sub_tree = new ProtocolTree(zero_sub_tree_root);
            p_tmp_sub_tree.init_last_child_idx();
            sub_tree2copy.init_is_copied();
        } // End of "copy_sub_tree"

        public FormulaTree convert2FormulaTree()
        {
            ProtocolTree p_tree = this;
            LogicNode curr_parent_formula_tree = new LogicNode( p_tree.get_root() );
            LogicNode f_tree_root = curr_parent_formula_tree;
            PartyNode curr_parent_protocol_tree = (PartyNode)p_tree.get_root();
            PartyNode p_tree_root = (PartyNode)p_tree.get_root();

            PartyNode p_tmp_node = new PartyNode();
            int tmp_last_child_idx = 0;
            
            // Preorder 
            // Keep add children while the root hasn't finished to add children.
            while (((curr_parent_protocol_tree == p_tree_root) && (p_tree_root.get_last_child_idx() < (m_num_of_children - 1))) || (curr_parent_protocol_tree != p_tree_root))
            {
                // Add children to an internal nodes (curr_node)
                while (curr_parent_protocol_tree.get_last_child_idx() < (m_num_of_children - 1))
                {
                    curr_parent_protocol_tree.inc_last_child_idx();
                    curr_parent_formula_tree.inc_last_child_idx();
                    tmp_last_child_idx = curr_parent_protocol_tree.get_last_child_idx();
                    p_tmp_node = (PartyNode)curr_parent_protocol_tree.get_child(tmp_last_child_idx);
                    if (p_tmp_node == null) break;
                    // Check if the node is reachable, if not -> don't add it to the formula tree
                    if (p_tmp_node.get_reachable() == reachable_type.UNREACHABLE) continue;
                    if (p_tmp_node.get_reachable() == reachable_type.NA)  throw new System.ArgumentException("Invalid reachble_type: NA");

                    LogicNode logic_child_to_add = new LogicNode(p_tmp_node);
                    curr_parent_formula_tree.add_child(logic_child_to_add, tmp_last_child_idx);

                    // Update current nodes of Formula Tre and Protocol Tree
                    curr_parent_formula_tree = logic_child_to_add;
                    curr_parent_protocol_tree = p_tmp_node;
                }
                // If curr_node != root, move upstream
                if (curr_parent_protocol_tree.get_parent() != null)
                {
                    curr_parent_protocol_tree = (PartyNode)curr_parent_protocol_tree.get_parent();
                    curr_parent_formula_tree = (LogicNode)curr_parent_formula_tree.get_parent();
                }
                // If curr_node == root, move orizontally to the next child
                else if (curr_parent_protocol_tree.get_last_child_idx() < m_num_of_children - 1)
                {
                    curr_parent_protocol_tree = (PartyNode)curr_parent_protocol_tree.get_child(curr_parent_protocol_tree.get_last_child_idx());
                    curr_parent_formula_tree = (LogicNode)curr_parent_formula_tree.get_child(curr_parent_formula_tree.get_last_child_idx());
                }
                else
                {
                    break;
                }
            }
            
            FormulaTree resilient_formula_Tree = new FormulaTree(f_tree_root);
            TreeUtils.write_tree_to_file(resilient_formula_Tree);
            resilient_formula_Tree.init_last_child_idx();
            p_tree.init_last_child_idx();

            return resilient_formula_Tree;
        } // End of "convert2FormulaTree"
        
        public void set_subtree_unreachable()
        {
            foreach ( PartyNode p_node in this )
            {
                p_node.set_reachable(reachable_type.UNREACHABLE);
            }
        }
    }
}


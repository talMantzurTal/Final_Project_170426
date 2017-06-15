using System;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Isam.Esent.Interop;
using System.Text;
using System.Threading.Tasks;


namespace FinalProject
{
    class ProtocolTree : Tree
    {
        // Data members:
        List<PartyNode> m_leaves_array;

        // C'TORS
        public ProtocolTree() :
            base()
        {
            Console.WriteLine("ProtocolTree c'tor");
            m_leaves_array = new List<PartyNode>();
        }
        public ProtocolTree(PartyNode root_node) :
            base((Node)root_node)
        {
            Console.WriteLine("ProtocolTree c'tor");
            m_leaves_array = new List<PartyNode>();
        }
        public ProtocolTree(Tree tree) :
            base(tree)
        {
            Console.WriteLine("ProtocolTree c'tor");
            m_leaves_array = new List<PartyNode>();
        }

        // GETTERS
        PartyNode get_leaves_array_cell(int idx = 0)
        {
            return m_leaves_array[idx];
        }

        List<PartyNode> get_leaves_array()
        {
            return m_leaves_array;
        }
        // SETTERS 
        public void set_leaves_array()
        {
            foreach ( Node node in this )
            {
                if (node.get_if_leaf())
                    m_leaves_array.Add((PartyNode)node);
            }
        }

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
                Console.WriteLine("ALICE");
            }
            else
            {
                root_type = node_type.BOB;
                Console.WriteLine("BOB");
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
                Console.WriteLine(n.get_name());

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
            double depth_strech_param = (Globals.egh_immune / Globals.eps);
            int egh_desired_depth = (int)depth_strech_param*kw_tree.get_depth();

            Console.WriteLine("EGH");
            Console.WriteLine("---");
            
            foreach (PartyNode egh_node in egh_tree)
            {
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

                // Replace the last child with it's corresponding sub_tree (deep copy) with the sub_tree_root_kw as a root
                if (egh_node.get_child(num_of_children_idx) == null)
                {
                    ProtocolTree sub_tree_to_copy = new ProtocolTree((PartyNode)sub_tree_root_kw);
                    sub_tree_to_copy.set_depth();

                    // Verify that the new EGH tree depth is less than the egh_desired_depth 
                    if (sub_tree_to_copy.get_depth() + egh_node.get_depth() < egh_desired_depth)
                    {
                        ProtocolTree cloned_sub_tree = new ProtocolTree(sub_tree_to_copy.deep_copy());
                        //egh_node.add_child(cloned_sub_tree.get_root(), num_of_children_idx, copy_flags.SHALLOW);
                        egh_node.add_child(cloned_sub_tree.get_root(), num_of_children_idx, copy_flags.SHALLOW);
                    }
                }
               }

            // TreeUtils.write_tree_to_file(egh_tree);
            // Perform Zero padding
            egh_tree.set_depth();
            egh_tree.zero_padding(egh_desired_depth);

            return egh_tree;
        }

        public static FormulaTree reverse_kw(ProtocolTree kw_tree, ProtocolTree egh_tree)
        {
            PartyNode curr_node = null;
            // Set leaves array
            egh_tree.set_leaves_array();
            while (curr_node != egh_tree.get_root())
            {
                curr_node = egh_tree.get_leaves_array_cell();
                // Generate error binary vectors with size curr_node.depth() and limit the legal vectors
                int[] error_vector_to_generate = new int[curr_node.get_depth()];
                curr_node.generate_alphabeth_vectors(error_vector_to_generate, 0, error_vector_to_generate.Length, 2);
                List<int[]> error_binary_vectors = curr_node.get_error_vectors_list();
                error_binary_vectors = curr_node.limit_num_of_errors(error_binary_vectors);

                // Foreach node in m_leaves_array, get it's path from root in egh_tree
                List<PartyNode> leaves_array = egh_tree.get_leaves_array();
                foreach ( PartyNode node in leaves_array)
                {
                    List<int> real_path_to_node = node.get_real_egh_path();
                    List<int[]> legal_vectors_per_node = node.generate_legel_vectors(error_binary_vectors, real_path_to_node);
                    // PartyNode::reachability()
                }
                
                egh_tree.update_leaves_array();
                
            }
            //return convert2FormulaTree()
            return null; // TODO: change
        }

        /* ProtocolTree::zero_padding(int tree_depth_after_padding)
         * This method perform zerro pedding of tree in order to create a complete tree.
         * [INPUT]:
         * tree_depth_after_padding = tree's desired depth after zero padding.
         * [OUTPUT]:
         * None
         * [Algoritm]:
         * go over all tree's nodes and for each node do:                                 
         * if the node depth is less than the desired tree depth and at least one of it's childrens               
         * is null, set this children with a zero subtree with depth = (egh_desired_depth - current node's depth)
         * Note that in order to create a zero subtree, this function calls create_zero_sub_tree.                   
         **********************************************************************************************************/
        void zero_padding(int tree_depth_after_padding)
        {
            ProtocolTree zero_sub_tree = new ProtocolTree();
            foreach (Node node in this)
            {
                for (int child_idx = 0; child_idx < m_num_of_children; child_idx++ )
                {
                   if ((node.get_depth() < tree_depth_after_padding) && (node.get_child(child_idx) == null))
                   {
                       zero_sub_tree =(ProtocolTree) create_zero_sub_tree(tree_depth_after_padding, node.get_type(), node.get_depth() + 1 );
                        //node.add_child(zero_sub_tree.get_root());
                        node.add_child(zero_sub_tree.get_root());
                    }
                }               
            }
            TreeUtils.write_tree_to_file(this);
        }
        
        Tree create_zero_sub_tree(int tree_depth_after_padding, node_type type_parent = node_type.BOB ,int start_depth = 0)
        {
            string name = "0";
            int idx_for_name = 0;
            node_type type;
            if ( type_parent == node_type.ALICE ) type= node_type.BOB;
            else if (type_parent == node_type.BOB) type = node_type.ALICE;
            else throw new System.ArgumentException("create_zero_sub_tree1: Illegal type for party node", type_parent.ToString());
            int cur_depth_child = start_depth;
            
            PartyNode cur_node_parent = new PartyNode();
            PartyNode root_zero_tree = new PartyNode(name,type,cur_depth_child, this.m_num_of_children);
            cur_node_parent = root_zero_tree;

            if (cur_node_parent.get_depth() != tree_depth_after_padding)
            {
                while (((cur_node_parent == root_zero_tree) && (root_zero_tree.get_last_child_idx() < (m_num_of_children - 1))) || (cur_node_parent != root_zero_tree))
                {
                    while ((cur_node_parent.get_depth() < tree_depth_after_padding) && (cur_node_parent.get_last_child_idx() < (m_num_of_children - 1)))
                    {
                        // Type
                        if (cur_node_parent.get_type() == node_type.ALICE) type = node_type.BOB;
                        else if (cur_node_parent.get_type() == node_type.BOB) type = node_type.ALICE;
                        else throw new System.ArgumentException("create_zero_sub_tree2: Illegal type for party node", cur_node_parent.get_type().ToString());
                        // Depth
                        cur_depth_child = cur_node_parent.get_depth() + 1;
                        // Name 
                        idx_for_name++;
                        name = idx_for_name.ToString();

                        // Create a new PartyNode
                        PartyNode child_to_add = new PartyNode(name, type, cur_depth_child, this.m_num_of_children);

                        child_to_add.set_is_zero_padding(true);
                        cur_node_parent.inc_last_child_idx();
                        //cur_node_parent.add_child(child_to_add, cur_node_parent.get_last_child_idx());
                        cur_node_parent.add_child(child_to_add, cur_node_parent.get_last_child_idx());
                        //child_to_add.set_parent(cur_node_parent);
                        cur_node_parent = child_to_add;

                    }
                    if (cur_node_parent.get_parent() != null)
                    {
                        cur_node_parent = (PartyNode)cur_node_parent.get_parent();
                    }
                    else
                    {
                        cur_node_parent = (PartyNode)cur_node_parent.get_child(cur_node_parent.get_last_child_idx());
                    }
                }
            }
            ProtocolTree zero_sub_tree = new ProtocolTree(root_zero_tree);
            //TreeUtils.write_tree_to_file(zero_sub_tree);
            return zero_sub_tree;
        } // End of "create_zero_sub_tree"

        void update_leaves_array()
        {
            PartyNode curr_parent = new PartyNode();
            PartyNode prev_parent = new PartyNode();
            //int idx = 0;
            List<PartyNode> leave_array_cpy = new List<PartyNode>();
            foreach( Node node in m_leaves_array )
            {
                curr_parent = (PartyNode)node.get_parent();
                if ( prev_parent != curr_parent)
                {
                    leave_array_cpy.Add(curr_parent);
                    prev_parent = curr_parent;
                    //idx++;
                }
            }
            m_leaves_array = leave_array_cpy;
        } // End of "update_leaves_array"

      

    }
}


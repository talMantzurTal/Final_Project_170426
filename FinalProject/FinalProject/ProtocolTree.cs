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
            foreach (PartyNode node in this)
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
            int egh_desired_depth = (int)depth_strech_param * kw_tree.get_depth();
            int child_idx = 0;

            Console.WriteLine("EGH");
            Console.WriteLine("---");

            // Expand the EGH tree to the target depth (egh_desired_depth) by zero padding the EGH tree.
            // After that expantion, traverse the tree and change the nodes to their values after egh coding.
            egh_tree.zero_padding(egh_desired_depth);

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
                if (sub_tree_root_kw.get_if_leaf()) continue;
                // Replace the last child with it's corresponding sub_tree (deep copy) with the sub_tree_root_kw as a root
                if ((egh_node.get_child(num_of_children_idx) != null) && (egh_node.get_child(num_of_children_idx).get_name() == "0") )
                {
                    ProtocolTree sub_tree_to_copy = new ProtocolTree((PartyNode)sub_tree_root_kw);
                    sub_tree_to_copy.set_depth();

                    // Verify that the new EGH tree depth is less than the egh_desired_depth 
                    if (sub_tree_to_copy.get_depth() + egh_node.get_depth() < egh_desired_depth)
                    {
                        ProtocolTree cloned_sub_tree = new ProtocolTree(sub_tree_to_copy.deep_copy());
                        cloned_sub_tree.set_depth( sub_tree_to_copy.get_depth() );
                        child_idx = egh_node.get_last_child_idx() + 1;
                        while ( (child_idx < egh_node.get_num_of_children() ) && (egh_node.get_child(child_idx).get_name() != "0"))
                        {
                            egh_node.inc_last_child_idx();
                            child_idx = egh_node.get_last_child_idx();
                        }
                        egh_tree.copy_sub_tree((PartyNode)egh_node.get_child(child_idx), cloned_sub_tree);
                        //foreach (PartyNode sub_node in cloned_sub_tree)
                        ///////////////
                        //egh_node.add_child(cloned_sub_tree.get_root(), num_of_children_idx, copy_flags.SHALLOW);
                    }
                }
            }

            TreeUtils.write_tree_to_file(egh_tree);
            // Perform Zero padding
            egh_tree.set_depth();
            egh_tree.init_last_child_idx();
            return egh_tree;
#if bug_fix
            // Deep copy the kw_tree in order to use it for reacability
            ProtocolTree egh_tree = (ProtocolTree)kw_tree.deep_copy();
            PartyNode tmp_node_egh = new PartyNode(); // TAL: changed from Node to PartyNode in order to use the method get_protocol_node_reference
            int num_of_children = kw_tree.get_root().get_num_of_children();
            int num_of_children_idx = num_of_children - 1;
            double depth_strech_param = (Globals.egh_immune / Globals.eps);
            int egh_desired_depth = (int)depth_strech_param * kw_tree.get_depth();

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
#endif
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
            PartyNode curr_node = null;
            int zero_leaves_counter = 0;
            int num_of_children = egh_tree.get_root().get_num_of_children();
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
                    curr_node = egh_tree.get_leaves_array_cell(idx);
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
                    continue;
                }
                // Generate error binary vectors with size curr_node.depth() and limit the legal vectors by the desired 
                // error fraction
                int[] error_vector_to_generate = new int[curr_node.get_depth()];
                curr_node.generate_alphabeth_vectors(error_vector_to_generate, 0, error_vector_to_generate.Length, 2);
                List<int[]> error_binary_vectors = curr_node.get_error_vectors_list();
                error_binary_vectors = curr_node.limit_num_of_errors(error_binary_vectors);

                // Foreach node in m_leaves_array:
                // 1. Get it's path from root in egh_tree
                // 2. Generate leagl error vectors from "error_binary_vectors"
                // 3. Check if the node is reachable
                List<PartyNode> leaves_array = egh_tree.get_leaves_array();
                foreach (PartyNode node in leaves_array)
                {
                    if (node.get_name() == "0") continue;
                    if ((node.get_reachable()) == reachable_type.NA)
                    {
                        List<int> real_path_to_node = node.get_real_egh_path();
                        List<int[]> legal_vectors_per_node = node.generate_legel_vectors(error_binary_vectors, real_path_to_node);
                        node.is_reachable(kw_tree, legal_vectors_per_node, real_path_to_node);
                    }
                }

                egh_tree.update_leaves_array();

            }
            TreeUtils.write_tree_to_file(egh_tree);
            return egh_tree.convert2FormulaTree();
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
            TreeUtils.write_tree_to_file(this);
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
        Tree create_zero_sub_tree(int tree_depth_after_padding, node_type type_parent = node_type.BOB, int start_depth = 0)
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

        /* ProtocolTree::update_leaves_array()
         * This method updates m_leaves_array. The method enters the nodes of a specific depth (parents of the current 
         * m_leaves_array), only if their m_reachable is NA (hasn;t been assigned yet). The main target of the method 
         * is the reachability method.
         * [INPUT]:
         * void
         * [OUTPUT]:
         * void               
         **********************************************************************************************************/
        void update_leaves_array()
        {
            PartyNode curr_parent = new PartyNode();
            PartyNode prev_parent = new PartyNode();
            List<PartyNode> leave_array_cpy = new List<PartyNode>();

            // Iterate over all nodes in m_leave_array. Foreach node and add it's parent to m_leaves_array if it's an reachable_type.NA 
            // reachble and not a zero leaf  
            foreach (Node node in m_leaves_array)
            {
                if (node.get_parent() == null) continue;
                curr_parent = (PartyNode)node.get_parent();
                if (prev_parent != curr_parent)
                {
                    /*if ((curr_parent.get_reachable()) == reachable_type.UNREACHABLE)
                        throw new System.ArgumentException("update_leaves_array: node {0} is UNREACHABLE?!", curr_parent.get_name().ToString());
                    if (curr_parent.get_name() == "0")
                        curr_parent.set_reachable(reachable_type.UNREACHABLE);
                    if ((curr_parent.get_reachable()) == reachable_type.NA)*/
                        /* Node wasn't examine if it's reachable yet - Add it to list in order to check its reachability */
                        leave_array_cpy.Add(curr_parent);

                    prev_parent = curr_parent;

                }
            }
            // Note: in case that all nodes are reachable, m_leaves_array will be empty. 
            m_leaves_array = leave_array_cpy;
        } // End of "update_leaves_array"

        /* ProtocolTree::copy_leaves_array_from_idx()
         * This method copies m_leaves_array to itself starting an intput index to the end.
         * [INPUT]:
         * start_idx = the first index we want to copy
         * [OUTPUT]:
         * void               
         **********************************************************************************************************/
        void copy_leaves_array_from_idx_to_end(int start_idx = 0)
        {
            List<PartyNode> leave_array_cpy = new List<PartyNode>();
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


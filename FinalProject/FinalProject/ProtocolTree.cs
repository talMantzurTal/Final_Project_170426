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
        public ProtocolTree(int num_of_children, PartyNode root_node) :
            base(num_of_children, (Node)root_node)
        {
            Console.WriteLine("ProtocolTree c'tor");
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = num_of_children;
        }
        public ProtocolTree(/*ProtocolTree*/ Tree tree) :
            base(/*(Tree)*/tree)
        {
            Console.WriteLine("ProtocolTree c'tor");
        }
#if v
        public Node get_root(copy_flags flag = copy_flags.SHALLOW)
        {
            if (flag == copy_flags.SHALLOW)
                return m_root;   
            //Node root_cpy = base.get_root();
            //PartyNode protocol_root_cpy = new PartyNode(root_cpy);
            //return 
        }
#endif
        public Tree deep_copy() //vered!!!
        {
            /* perform deep copy of the root and use this copy in order to create a sub ProtocolTree */
            Node cloned_root = m_root.deep_copy();
            ProtocolTree cloned_protocol_tree = new ProtocolTree(m_num_of_children,(PartyNode)cloned_root);
            return (this.deep_copy(/*cloned_root,*/cloned_protocol_tree));
        }


        public static ProtocolTree kw_trans(FormulaTree f_tree)
        {
            // Get formula tree root, and create a protocol tree from this f_root
            Node f_root = new LogicNode();
            f_root = f_tree.get_root();
            string root_name = f_root.get_node_name();
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
            ProtocolTree p_tree = new ProtocolTree(p_num_of_children, p_root);

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
                if (!p_tmp_node.get_node_name().Equals(f_root.get_node_name()))
                {
                    Node parent_node = Tree.preOrder(p_root, n.get_parent().get_node_name(), true);
                 //   parent_node.set_child((Node)p_tmp_node);   // Set_child
                      parent_node.set_child(p_tmp_node); //vered!!
                }
                Console.WriteLine(n.get_node_name());

                foreach (var child in n.get_children().ToArray().Reverse())
                {
                    if (child != null)
                    {
                        s.Push(child);
                    }
                }
            }

            p_tree.set_depth();
            return p_tree;
        }

        public static ProtocolTree egh(ProtocolTree kw_tree)
        {
            // TODO: Deep clone of the kw_tree (extends IDeepClonable) for reacability
            ProtocolTree egh_tree = (ProtocolTree)kw_tree.deep_copy();
            PartyNode tmp_node_egh = new PartyNode(); // TAL: changed from Node to PartyNode in order to use the method get_protocol_node_reference
            int num_of_children = kw_tree.get_root().get_num_of_children();
            int num_of_children_idx = num_of_children - 1;
            double depth_strech_param = (Globals.egh_immune / Globals.eps);
            int egh_desired_depth = (int)depth_strech_param*kw_tree.get_depth();

            Console.WriteLine("EGH");
            Console.WriteLine("---");
            foreach (Node egh_node in egh_tree)
            {
                Node sub_tree_root_kw = new PartyNode();
                List <Node> sub_formula_arr = new List<Node>();
                if (egh_node.get_depth() < 2)   // Replace the last child with root's sub_tree
                {
                    tmp_node_egh = (PartyNode)egh_tree.get_root(copy_flags.SHALLOW);
                    sub_tree_root_kw = kw_tree.get_root();
                    
                }
                else  // Replace the last child with the grandparent's sub_tree <--> the parent of the current node
                {

                    tmp_node_egh = (PartyNode)egh_node.get_parent(copy_flags.SHALLOW).get_parent(copy_flags.SHALLOW);
                    sub_tree_root_kw = tmp_node_egh.get_protocol_node_reference(); 

                }

                // Replace the last child with it's corresponding sub_tree (deep copy) with the sub_tree_root_kw as a root
                if (egh_node.get_child(num_of_children_idx) == null)
                {
                    ProtocolTree sub_tree_to_copy = new ProtocolTree(num_of_children, (PartyNode)sub_tree_root_kw);
                    sub_tree_to_copy.set_depth();

                    // Verify that the new EGH tree is larger than egh_desired_depth
                    if (sub_tree_to_copy.get_depth() + egh_node.get_depth() <= egh_desired_depth)
                    {
                        ProtocolTree cloned_sub_tree = new ProtocolTree(sub_tree_to_copy.deep_copy());
                        egh_node.add_child(cloned_sub_tree.get_root(), num_of_children_idx, copy_flags.SHALLOW);
                    }
                }
               }

            // TreeUtils.write_tree_to_file(egh_tree);
            // Zero padding
            egh_tree.zero_padding(egh_desired_depth);

            return null;
        }

        void zero_padding(int tree_depth_after_padding, int start_depth = 0)
        {
            create_zero_sub_tree(tree_depth_after_padding, start_depth);
        }
        
        void create_zero_sub_tree(int tree_depth_after_padding, int start_depth = 0)
        {
            string name = "0";
            int idx_for_name = 0;
            node_type type = node_type.ALICE;
            int cur_depth_child = start_depth;
            
            PartyNode cur_node_parent = new PartyNode();
            PartyNode root_zero_tree = new PartyNode(name,type,cur_depth_child, this.m_num_of_children);
            cur_node_parent = root_zero_tree;

            while ( ((cur_node_parent == root_zero_tree)&&(root_zero_tree.get_last_child_idx() < (m_num_of_children - 1))) || (cur_node_parent != root_zero_tree) )
            {
                while ( (cur_node_parent.get_depth() < tree_depth_after_padding) && (cur_node_parent.get_last_child_idx() < (m_num_of_children-1)) )
                {
                    // Type
                    if (cur_node_parent.get_type() == node_type.ALICE) type = node_type.BOB;
                    else if (cur_node_parent.get_type() == node_type.BOB) type = node_type.ALICE;
                    else throw new System.ArgumentException("zero_padding: Illegal type for party node", cur_node_parent.get_type().ToString());
                    // Depth
                    cur_depth_child = cur_node_parent.get_depth() + 1;
                    // Name 
                    idx_for_name++;
                    name = idx_for_name.ToString();

                    // Create a new PartyNode
                    PartyNode child_to_add = new PartyNode(name, type, cur_depth_child, this.m_num_of_children);

                    child_to_add.set_is_zero_padding(true);
                    cur_node_parent.inc_last_child_idx();
                    cur_node_parent.add_child(child_to_add,cur_node_parent.get_last_child_idx());
                    cur_node_parent = child_to_add;
                    
                }
                if (cur_node_parent.get_parent() != null)
                {
                    cur_node_parent = (PartyNode)cur_node_parent.get_parent();
                } else
                {
                    cur_node_parent = (PartyNode)cur_node_parent.get_child(cur_node_parent.get_last_child_idx());
                }
            }
            
            ProtocolTree zero_sub_tree = new ProtocolTree(root_zero_tree.get_num_of_children(), root_zero_tree);
            TreeUtils.write_tree_to_file(zero_sub_tree);
        } // End of "create_zero_sub_tree"
    }
}


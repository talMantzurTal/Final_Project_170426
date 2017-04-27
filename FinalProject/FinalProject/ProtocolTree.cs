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
        public ProtocolTree(Tree tree) :
            base(tree)
        {
            Console.WriteLine("ProtocolTree c'tor");
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
                    parent_node.set_child((Node)p_tmp_node);   // Set_child
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


            return p_tree;
        }

        public static ProtocolTree egh(ProtocolTree kw_tree)
        {
            // TODO: Deep clone of the kw_tree (extends IDeepClonable) for reacability
            ProtocolTree tmp_tree = kw_tree;
            Node tmp_node = new Node();
            int num_of_children = kw_tree.get_root().get_num_of_children();
            int num_of_children_idx = num_of_children - 1;

            Console.WriteLine("EGH");
            Console.WriteLine("---");
            foreach (Node node in kw_tree)
            {
                Node sub_tree_root = new Node();
                if (node.get_depth() < 2)   // Replace the last child with root's sub_tree
                {
                    sub_tree_root = kw_tree.get_root(copy_flags.SHALLOW);
                }
                else  // Replace the last child with the grandparent's sub_tree <--> the parent of the current node
                {
                   // sub_tree_root = node.get_parent(copy_flags.SHALLOW);
                    sub_tree_root = node.get_parent(copy_flags.SHALLOW).get_parent(copy_flags.SHALLOW);

                }

                // Replace the last child with it's corresponding sub_tree (deep copy) with the sub_tree_root as a root
                if (node.get_child(num_of_children_idx) == null)
                {
                    ProtocolTree sub_tree = new ProtocolTree(num_of_children, (PartyNode)sub_tree_root);
                    ProtocolTree cloned_sub_tree = new ProtocolTree(sub_tree.deep_copy());
                }
            }



            return null;
        }
    }
}

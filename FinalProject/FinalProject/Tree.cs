using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject;

namespace FinalProject
{
    class Tree : IEnumerable
    {
        protected int m_depth;
        protected int m_num_of_children;
        protected Node m_root;
        public const int INVALID_VALUE = -1;

        public Tree()
        {
            Console.WriteLine("Tree c'tor");
            m_root = null;
            m_depth = 0;
            m_num_of_children = 0;
        }

        public Tree(int num_of_children, Node root_node)
        {
            Console.WriteLine("Tree c'tor");
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = num_of_children;
        }

        public Node get_root()
        {
            return m_root;
        }

        public void set_initial_tree(int i_alphabeth_size, Node root_node)
        {
            Console.WriteLine("Tree set_initial_tree");
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = i_alphabeth_size;
        }

        public static Node preOrder(Node root, string name_to_search = "", bool search_flag = false)
        {
            Stack<Node> s = new Stack<Node>();
            s.Push(root);
            while (s.Count > 0)
            {
                var n = s.Pop();
                // if search_flag then search a node by name and return it
                if ( search_flag && (n.get_node_name().Equals(name_to_search)) )
                {
                    return n;
                }
                // Do Action
                Console.Write(n.get_node_name());
                foreach (var child in n.get_children().ToArray().Reverse())
                {
                    if (child != null)
                    {
                        s.Push(child);
                    }
                }
            }
            return null;
        }

        // Enumerator
        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public TreeEnum GetEnumerator()
        {
            return new TreeEnum(this);
        }

        public Tree deep_copy()
        {
            Tree tree_to_return = new Tree(m_num_of_children, m_root);
            string name;
            node_type type;
            int depth;
            int number_of_children;
            Node parent;
            Node[] children;
            bool if_leaf;
            int leaf_idx;
            int last_child_idx;

            foreach (Node node in this)
            {
                name = node.get_node_name();
                type = node.get_type();
                depth = node.get_depth();
                number_of_children = node.get_num_of_children();
                parent = node.get_parent();
                children = node.get_children();
                if_leaf = node.get_if_leaf();
                leaf_idx = node.get_leaf_idx();

                Node tmp_node = new Node(name, type, depth, number_of_children, parent);
                tmp_node.set_leaf_idx(leaf_idx);
                tmp_node.set_if_leaf(if_leaf);

            }
            return null;
        }
    }
}

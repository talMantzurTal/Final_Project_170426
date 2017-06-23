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

        public Tree(Node root_node)
        {
            Console.WriteLine("Tree c'tor");
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = root_node.get_num_of_children();
        }

        public Tree(Tree tree)
        {
            Console.WriteLine("Tree c'tor");
            m_root = tree.get_root();
            m_depth = tree.get_depth();
            m_num_of_children = m_root.get_num_of_children();
        }

        public Node get_root(copy_flags flag = copy_flags.SHALLOW)
        {
            if (flag == copy_flags.SHALLOW) return m_root;

            return m_root.deep_copy();
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
                if (search_flag && (n.get_name().Equals(name_to_search)))
                {
                    return n;
                }
                // Do Action
                Console.Write(n.get_name());
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

        public Tree deep_copy(Tree cloned_tree)
        {
            /* Todo: 1.Deep copy should be virtual function in all node kinds. at each "special" node, this routine will call the base routine
             *       2.check the type of m_root and create clone_tree and cloned_root according to that type.*/

            Node cloned_root = cloned_tree.get_root();
            string name;

            // Deep copy and add to the tree all nodes axcept the root (which is already in it)

            foreach (Node node in this)
            {
                name = node.get_name();

                Node[] children = node.get_children();
                Node clone_curr_node = Tree.preOrder(cloned_root,name,true);
                foreach (Node child in children)
                {
                    if (child == null) continue;
                    Node tmp_child_node = child.deep_copy();
        
                    clone_curr_node.add_child(tmp_child_node);
                }

            }
            Console.WriteLine("DEEP COPY:\n------------");

            foreach (Node node in cloned_tree)
            {
                Console.WriteLine(node.get_name());
            }
            return cloned_tree;
        }

        public int get_depth()
        {
            return m_depth;
        }

        public void set_depth(int depth_in = 0, bool flag_if_calc = true )
        {
            if (!flag_if_calc)
            {
                m_depth = depth_in;
                return;
            }
            int max_depth = 0;
            foreach (Node node in this)
            {
                if ((node.get_if_leaf()) && (max_depth < node.get_depth()))
                    max_depth = node.get_depth();
            }
            m_depth = max_depth ; // according number of edges

        }

        public void init_last_child_idx()
        {
            foreach ( Node node in this )
                node.init_last_child_idx();
        }

        public void init_is_copied()
        {
            foreach (Node node in this)
                node.init_is_copied();
        }
    }
}

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

        public Tree(Tree tree)
        {
            Console.WriteLine("Tree c'tor");
            m_root = tree.get_root();
            m_depth = m_root.get_depth();
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
                if (search_flag && (n.get_node_name().Equals(name_to_search)))
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
            Node cloned_root = m_root.deep_copy();
            Tree cloned_tree = new Tree(m_num_of_children, cloned_root);
            string name;

            // Deep copy and add to the tree all nodes axcept the root (which is already in it)
#if v
            var this_enum = this.GetEnumerator();
            var cloned_enum = cloned_tree.GetEnumerator();
            while (this_enum.MoveNext() && cloned_enum.MoveNext())
            {
                var node = (Node)this_enum.Current;
                var cloned_node = (Node)cloned_enum.Current;

                name = node.get_node_name();
                //if ( name == m_root.get_node_name() ) continue;

                Node[] children = node.get_children();
                foreach (Node child in children)
                {
                    if (child == null) continue;
                  //  if (child == null) return null;
                    // If node != root
                    Node tmp_child_node = child.deep_copy();
                    cloned_node.set_child(tmp_child_node);
                }
            }
#endif

            foreach (Node node in this)
                {
                    name = node.get_node_name();
                    //if ( name ot.get_== m_ronode_name() ) continue;

                    Node[] children = node.get_children();
                    Node clone_curr_node = Tree.preOrder(cloned_root,name,true);
                    foreach (Node child in children)
                    {
                        if (child == null) continue;
                        // If node != root
                        Node tmp_child_node = child.deep_copy();
                        clone_curr_node.set_child(tmp_child_node);
                    }

                }
            Console.WriteLine("DEEP COPY:\n------------");

            foreach (Node node in cloned_tree)
            {
                Console.WriteLine(node.get_node_name());
            }
            return cloned_tree;
        }
    }
}

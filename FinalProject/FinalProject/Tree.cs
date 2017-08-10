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
        // Data members:
        protected int m_depth;
        protected int m_num_of_children;
        protected Node m_root;
        public const int INVALID_VALUE = -1;
        protected List<Node> m_leaves_array;
        protected bool m_status_number_leaves;
        protected bool m_status_number_gates;
        public int m_number_of_int_nodes;
        public Globals globals;
        private int m_number_of_leaves;

        public Tree()
        {
            m_root = null;
            m_depth = 0;
            m_num_of_children = 0;
            m_leaves_array = new List<Node>();
            m_status_number_leaves = false;
            m_status_number_gates = false;
            globals = Globals.get_instance();
            m_number_of_int_nodes = 0;
            m_number_of_leaves = 0;
        }

        public Tree(Node root_node)
        {
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = root_node.get_num_of_children();
            m_leaves_array = new List<Node>();
            m_status_number_leaves = false;
            m_status_number_gates = false;
            globals = Globals.get_instance();
            m_number_of_int_nodes = 0;
            m_number_of_leaves = 0;
        }

        public Tree(Tree tree)
        {
            m_root = tree.get_root();
            m_depth = tree.get_depth();
            m_num_of_children = m_root.get_num_of_children();
            m_leaves_array = new List<Node>();
            m_status_number_leaves = false;
            m_status_number_gates = false;
            globals = Globals.get_instance();
            m_number_of_int_nodes = 0;
            m_number_of_leaves = 0;
        }

        public Node get_root(copy_flags flag = copy_flags.SHALLOW)
        {
            if (flag == copy_flags.SHALLOW) return m_root;

            return m_root.deep_copy();
        }

        public int get_number_of_int_nodes()
        {
            return m_number_of_int_nodes;
        }

        public int get_number_of_leaves()
        {
            return m_number_of_leaves;
        }

        public int get_num_of_children()
        {
            return m_num_of_children;
        }

        public void set_initial_tree(int i_alphabeth_size, Node root_node)
        {
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
            m_depth = max_depth - this.get_root().get_depth(); // according number of edges

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

        public void set_leaves_array()
        {
            foreach (Node node in this)
            {
                if (node.get_if_leaf())
                    m_leaves_array.Add(node);
            }
        }


        /* Tree::update_leaves_array()
         * This method updates m_leaves_array. The method enters the nodes of a specific depth (parents of the current 
         * m_leaves_array), only if their m_reachable is NA (hasn;t been assigned yet). The main target of the method 
         * is the reachability method.
         * [INPUT]:
         * void
         * [OUTPUT]:
         * void               
         **********************************************************************************************************/
        public void update_leaves_array()
        {
            Node curr_parent = new Node();
            Node prev_parent = new Node();
            List<Node> leave_array_cpy = new List<Node>();

            // Iterate over all nodes in m_leave_array. Foreach node and add it's parent to m_leaves_array if it's an reachable_type.NA 
            // reachble and not a zero leaf  
            foreach (Node node in m_leaves_array)
            {
                if (node.get_parent() == null) continue;
                curr_parent = node.get_parent();
                if (prev_parent != curr_parent)
                {
                    leave_array_cpy.Add(curr_parent);
                    prev_parent = curr_parent;
                }
            }
            // Note: in case that all nodes are reachable, m_leaves_array will be empty. 
            m_leaves_array = leave_array_cpy;
        } // End of "update_leaves_array"

        public Node get_leaves_array_cell(int idx = 0)
        {
            return m_leaves_array[idx];
        }

        public List<Node> get_leaves_array()
        {
            return m_leaves_array;
        }

        public void number_gates_preorder()
        {
            if (m_status_number_gates)
            {
                throw new System.ArgumentException("ERROR: Gates are already numbered");
            }
            m_status_number_gates = true;
            int gate_counter = 0;
            
            foreach ( Node node in this )
            {
                if (node.get_if_leaf()) continue;
                node.set_gate_idx(gate_counter);
                gate_counter++;
            }
            m_number_of_int_nodes = gate_counter;
        }

        /* FormulaTree::number_leaves()
         * The method number the leaves in the formula tree (using enumerator implementation)
         * IN: void
         * OUT: void
         * ********************************************************************************** */
        public void number_leaves()
        {
            if (m_status_number_leaves)
            {
                throw new System.ArgumentException("ERROR: Leaves are already numbered");
            }
            m_status_number_leaves = true;
            int leaf_count = 0;
            foreach (Node node in this)
            {

                if (node.get_if_leaf())
                {
                    node.set_leaf_idx(leaf_count);
                    leaf_count++;
                }
            }
            m_number_of_leaves = leaf_count;
            return;
        }
    }
}

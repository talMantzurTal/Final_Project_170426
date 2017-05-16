using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Node
    {
        protected string m_name;
        protected node_type m_type;
        protected int m_depth;
        protected int m_num_of_children;
        protected Node m_parent;
        protected Node[] m_children;
        protected bool m_if_leaf;
        protected int m_leaf_idx;
        protected int m_last_child_idx;
        public const int INVALID_VALUE = -1;

        public Node()
        {
            Console.WriteLine("Node c'tor");
            m_type = node_type.NA;
            m_depth = 0;
            m_num_of_children = 0;
            m_parent = null;
            m_name = "new node";
            m_children = null;
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 

        }

        public Node(string name, node_type type, int depth, int number_of_children, Node parent)
        {
            Console.WriteLine("Node c'tor");
            m_type = type;
            m_depth = depth;
            m_num_of_children = number_of_children;
            m_parent = parent;
            m_name = name;
            m_children = null;
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
        }

        public Node(Node node)
        {
            m_type = node.get_type();
            m_depth = node.get_depth();
            m_num_of_children = node.get_num_of_children();
          //  m_parent = node.get_parent();
            m_parent = null; //vered change
            m_name = node.get_node_name();
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
            m_children = null;
            // m_children = node.get_children();
        }


        public string get_node_name()
        {
            return m_name;
        }

        /*public Node get_child(string child_name)
        {
            foreach ( Node node in mp_children )
                if ((node != null) && (node.m_name == child_name)) return node;              
            return null;
        } */

        public void set_parent(Node parent)
        {
            this.m_parent = parent;
        }

        public void add_child(Node new_child, int child_to_add_idx = -1, copy_flags flag = copy_flags.SHALLOW)
        {
            // Find the first empty child which hasn't been assigned 
            int child_idx = 0;
            if (child_to_add_idx == INVALID_VALUE)
            {
                while ((child_idx < m_num_of_children) && (m_children[child_idx] != null))
                {
                    child_idx++;
                }
            }
            else child_idx = child_to_add_idx;

            // Change m_if_leaf to false.
            m_if_leaf = false;

            // If flag = shallow copy, the new child is the pointer that accepted 
            // "new_child", including it's subtree if exist.
            // -----------------------------------------------------------------------
            if (flag == copy_flags.SHALLOW)
            {
                this.m_children[child_idx] = new_child;
                new_child.set_parent(this);
                // Set child's depth
                m_children[child_idx].m_depth = this.m_depth + 1;
                return;
            }
            // If flag = deep copy, create a new tmp_node and add this node as a child
            // without it's subtree
            // -----------------------------------------------------------------------
            // Create a tmp_node to add as a child
            string name = new_child.get_node_name();
            node_type type = new_child.get_type();
            int depth = new_child.get_depth();
            int num_of_children = new_child.get_num_of_children();
            Node parent = new_child.get_parent();
            Node tmp_node = new Node(name, type, depth, num_of_children, parent);


            // Set tmp_node.if_leaf = true
            tmp_node.m_if_leaf = true;

            if (child_idx == m_num_of_children)
            {
                throw new System.ArgumentException("Can't add another child, there are already {0} children to this node", child_idx.ToString());
            }
            // Add tmp_node as a child
            m_children[child_idx] = tmp_node;
            m_children[child_idx].m_depth = this.m_depth + 1;
            return;
        }



        public bool set_child(Node child_to_add, copy_flags flag = copy_flags.SHALLOW)
        {

            int i = 0;
            if (m_children == null) {
                m_children = new Node[m_num_of_children];
                for (int child_idx = 0; child_idx < m_num_of_children; child_idx++)
                    m_children[child_idx] = null;
            }

            // Change m_if_leaf to false.
            m_if_leaf = false;

            // Find the first empty child which hasn't been assigned 
            while ((i < m_num_of_children) && (m_children[i] != null)) i++;
            if (i == m_num_of_children) return false;
            else
            {
                Type t = this.GetType();
                if (t.Equals(typeof(LogicNode)))
                    m_children[i] = new LogicNode(child_to_add);
                      
                else m_children[i] = new PartyNode(child_to_add);
                
                m_children[i].m_parent = this; /////// vered change
            }

            // Set children depth
            m_children[i].m_depth = this.m_depth + 1;
            if (m_children[i].m_if_leaf) return true;
            Tree tmp_tree_for_depth = new Tree(m_num_of_children,m_children[i]);
            foreach (Node node in tmp_tree_for_depth)
            {
                node.m_depth = node.get_parent().get_depth() + 1;
            }
            return true;
        }

        public int get_last_child_idx()
        {
            return m_last_child_idx;
        }

        public void inc_last_child_idx()
        {
            m_last_child_idx++;
        }

        public Node get_child(int last_child_idx)
        {
            return m_children[last_child_idx];
        }

        public void set_leaf_idx(int leaf_idx)
        {
            m_leaf_idx = leaf_idx;
        }

        public Node get_parent(copy_flags flag = copy_flags.SHALLOW)
        {
            return m_parent;
        }

        public int get_depth()
        {
            return m_depth;
        }


        public node_type get_type()
        {
            return m_type;
        }

        public int get_num_of_children()
        {
            return m_num_of_children;
        }

        public Node[] get_children(copy_flags flag = copy_flags.SHALLOW)
        {
            if (flag == copy_flags.SHALLOW) return m_children;

            // Deep copy
            Node[] children_to_return = new Node[m_num_of_children];
            int child_idx = 0;


            foreach (Node node in m_children)
            {
                if ((node != null) && (!node.m_if_leaf))
                    children_to_return[child_idx] = node.deep_copy();
            }
            return children_to_return;
        }

        /* Deep copy a node without pointers to it's parent and children */
        public virtual Node deep_copy()
        {
            string name = m_name;
            node_type type = m_type;
            int depth = m_depth;
            int number_of_children = m_num_of_children;
            bool if_leaf = m_if_leaf;
            int leaf_idx = m_leaf_idx;

            Node tmp_node = new Node(name, type, depth, number_of_children, /* parent = */null);
            tmp_node.set_leaf_idx(leaf_idx);
            tmp_node.set_if_leaf(if_leaf);

            return tmp_node;
        }

        public virtual int calculate_value(int[] input_vector)
        {
            return 0;
        }

        public bool get_if_leaf()
        {
            return m_if_leaf;
        }

        public int get_leaf_idx()
        {
            return m_leaf_idx;
        }

        public void set_if_leaf(bool if_leaf)
        {
            m_if_leaf = if_leaf;
        }

        



    }

}

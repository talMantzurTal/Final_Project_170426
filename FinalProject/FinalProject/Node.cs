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
        protected int m_my_depth;
        protected int m_number_of_children;
        protected Node m_parent;
        protected List<Node> m_children;
        protected bool m_if_leaf;
        protected int m_leaf_idx;
        protected int m_last_child_idx;
        public const int INVALID_VALUE = -1;

        public Node()
        {
            Console.WriteLine("Node c'tor");
            m_type = node_type.NA;
            m_my_depth = 0;
            m_number_of_children = 0;
            m_parent = null;
            m_name = "new node";
           // m_children = new List<Node>();
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
        
        }

        public Node(string name, node_type type, int depth, int number_of_children, Node parent)
        {
            Console.WriteLine("Node c'tor");
            m_type = type;
            m_my_depth = depth;
            m_number_of_children = number_of_children;
            m_parent = parent;
            m_name = name;
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
            //m_children = new List<Node>();
        }

        public Node(Node node)
        {
            m_type = node.get_type();
            m_my_depth = node.get_depth();
            m_number_of_children = node.get_num_of_children();
            m_parent = node.get_parent();
            m_name = node.get_node_name();
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
            //m_children = new List<Node>();
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
        
        public void add_child(Node new_child)
        {
            // Create a tmp_node to add as a child
            string name = new_child.get_node_name();
            node_type type = new_child.get_type();
            int depth = new_child.get_depth();
            int num_of_children = new_child.get_num_of_children();
            Node parent = new_child.get_parent();
            Node tmp_node = new Node(name, type, depth, num_of_children, parent);

            // Add tmp_node as a child
            int child_idx = 0;
            while ((child_idx < m_number_of_children) && (m_children[child_idx] != null) )
            {
                child_idx++;
            }
            if (child_idx == m_number_of_children)
            {
                throw new System.ArgumentException("Can't add another child, there are already {0} children to this node", child_idx.ToString());
            }
            m_children[child_idx] = tmp_node;
            return;
        }

        

        public bool set_child(Node child_to_add)
        {
            int i = 0;
            if (m_children[0] == null) m_if_leaf = false;
            while ( (i < m_number_of_children) && (m_children[i] != null) ) i++;
            if ( i == m_number_of_children) return false;
            else m_children[i] = child_to_add;
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

        public Node get_parent()
        {
            return m_parent;
        }

        public int get_depth()
        {
            return m_my_depth;
        }


        public node_type get_type()
        {
            return m_type;
        }

        public int get_num_of_children()
        {
            return m_number_of_children;
        }

        public List<Node> get_children()
        {
            List<Node> children_to_return = new List<Node>();
            string name;
            node_type type;
            int depth;
            int number_of_children;
            Node parent;
            Node[] children;
            bool if_leaf;
            int leaf_idx;
            int last_child_idx;

            foreach (Node node in m_children)
            {
                if (node != null)
                {
                    name = node.get_node_name();
                    type = node.get_type();
                    depth = node.get_depth();
                    number_of_children = node.get_num_of_children();
                    parent = node.get_parent();
                    if_leaf = node.get_if_leaf();
                    leaf_idx = node.get_leaf_idx();

                    Node tmp_node = new Node(name, type, depth, number_of_children, parent);
                    tmp_node.set_leaf_idx(leaf_idx);
                    tmp_node.set_if_leaf(if_leaf);

                    children_to_return.ToList<Node>().Add(tmp_node);
                }
            }
            return children_to_return;
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

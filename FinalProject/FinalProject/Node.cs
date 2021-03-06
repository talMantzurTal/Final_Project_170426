﻿using System;
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
        protected bool m_is_copied;
        protected int m_gate_idx;
        public Globals globals;

        public Node()
        {
            m_type = node_type.NA;
            m_depth = 0;
            m_num_of_children = 0;
            m_parent = null;
            m_name = "new node";
            m_children = null;
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visited at child[0], we increment m_last_child_idx 
            m_is_copied = false;
            m_gate_idx = INVALID_VALUE;
            globals = Globals.get_instance();
        }

        public Node(string name, node_type type, int depth, int number_of_children, Node parent)
        {
            m_type = type;
            m_depth = depth;
            m_num_of_children = number_of_children;
            m_parent = parent;
            m_name = name;
            m_children = null;
            m_if_leaf = true;
            m_leaf_idx = INVALID_VALUE;
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
            m_is_copied = false;
            m_gate_idx = INVALID_VALUE;
            globals = Globals.get_instance();
        }

        public Node(Node node)
        {
            m_type = node.get_type();
            m_depth = node.get_depth();
            m_num_of_children = node.get_num_of_children();
            m_parent = null; //vered change
            m_name = node.get_name();
            m_if_leaf = true;
            m_leaf_idx = node.get_leaf_idx();
            m_last_child_idx = INVALID_VALUE; // If we visted at child[0], we increment m_last_child_idx 
            m_children = null;
            m_is_copied = false;
            m_gate_idx = INVALID_VALUE;
            globals = Globals.get_instance();
        }


        // GETTERS
        public string get_name()
        {
            return m_name;
        }

        public int get_last_child_idx()
        {
            return m_last_child_idx;
        }

        public Node get_child(int last_child_idx)
        {
            return m_children[last_child_idx];
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

        public bool get_if_leaf()
        {
            return m_if_leaf;
        }

        public int get_leaf_idx()
        {
            return m_leaf_idx;
        }

        public bool get_is_copied()
        {
            return m_is_copied;
        }

        public int get_gate_idx()
        {
            return m_gate_idx;
        }

        /* PartyNode::limit_num_of_errors()
         * This method goes over all binary vectors which define errors (1 = error , 0= no error)
         * and for each vector checks: if the number of errors (cells that contains 1)
         * are bigger than error limit -> dispose this vector.
         * 
         * [INPUT]:
         * optional_binary_vectors = list of all binary vector with size n (2^n vectors)
         * 
         * [OUTPUT]:
         * limit_num_of_errors = list of legal error vectors - don't contain more than ERROR_FRACTION errors.
         * ********************************************************************************************************************/
        public List<int> get_path_from_root()
        {
            List<int> path = new List<int>();
            Node curr_node = this;
            while (curr_node.get_parent() != null)
            {
                path.Add(curr_node.my_idx_as_a_child());
                curr_node = (Node)curr_node.get_parent();
            }
            path.Reverse();
            return path;
        } // End of "get_path_from_root"

        // SETTERS
        public void set_parent(Node parent)
        {
            this.m_parent = parent;
        }

        public void set_leaf_idx(int leaf_idx)
        {
            m_leaf_idx = leaf_idx;
        }

        public void set_if_leaf(bool if_leaf)
        {
            m_if_leaf = if_leaf;
        }

        public virtual void set_node(Node reference_node)
        {
            m_name = reference_node.get_name();
            m_type = reference_node.get_type();
            // m_depth = reference_node.get_depth();
            m_num_of_children = reference_node.get_num_of_children();
            // m_if_leaf = reference_node.get_if_leaf();
            m_leaf_idx = reference_node.get_leaf_idx();
            m_last_child_idx = reference_node.get_last_child_idx();
            m_is_copied = false;
        }

        public void set_depth(int depth)
        {
            m_depth = depth;
        }

        public void set_is_copied(bool is_copied)
        {
            m_is_copied = is_copied;
        }

        public void set_gate_idx(int gate_idx)
        {
            m_gate_idx = gate_idx;
        }
        
        public void set_name(string name)
        {
            m_name = name;
        }

        // METHODS
        // --------
#if add_child
        /* Node::add_child()
         * The method add a child to a specific node int the first position that empty in node.m_children or in the
         * index that was sent to the method. The defauld addition is shallow, but it can be added in a deep way after 
         * creating new child.
         * [INPUT]:
         * new_child =        a Node type child, that we want to add to the current node
         * child_to_add_idx = The index we desired to assign the child to in the m_children array. 
         *                    If it isn't set, the method looks to the first empty cell to assign into
         * flag =             The flag indicates of how to add the child: as a pointer (shallow - the default) or to create a new Node (deep)
         * 
         * [OUTPUT]:
         * void
         * ********************************************************************************************************************************** */
        public void add_child(Node new_child, int child_to_add_idx = INVALID_VALUE, copy_flags flag = copy_flags.SHALLOW)
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

            // Change m_if_leaf to false, because this method add a child to it and therefor it became an internal node
            m_if_leaf = false;

            // SHALLOW COPY if required (default)
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

            // DEEP COPY if required
            // If flag = deep copy, create a new tmp_node and add this node as a child
            // without it's subtree
            // -----------------------------------------------------------------------
            // Create a tmp_node to add as a child
            string name = new_child.get_name();
            node_type type = new_child.get_type();
            int depth = new_child.get_depth();
            int num_of_children = new_child.get_num_of_children();
            Node parent = new_child.get_parent();
            Node child_to_add = new Node(name, type, depth, num_of_children, parent);


            // Set tmp_node.if_leaf = true
            child_to_add.m_if_leaf = true;

            if (child_idx == m_num_of_children)
            {
                throw new System.ArgumentException("Can't add another child, there are already {0} children to this node", child_idx.ToString());
            }
            // Add tmp_node as a child
            m_children[child_idx] = child_to_add;
            m_children[child_idx].m_depth = this.m_depth + 1;
            return;
        }
#endif
        /* Node::add_child()
         * The method add a child to a specific node int the first position that empty in node.m_children or in the
         * index that was sent to the method. The defauld addition is shallow, but it can be added in a deep way after 
         * creating new child.
         * [INPUT]:
         * new_child =        a Node type child, that we want to add to the current node
         * child_to_add_idx = The index we desired to assign the child to in the m_children array. 
         *                    If it isn't set, the method looks to the first empty cell to assign into
         * flag =             The flag indicates of how to add the child: as a pointer (shallow - the default) or to create a new Node (deep)
         * 
         * [OUTPUT]:
         * void
         * ********************************************************************************************************************************** */
        public bool add_child(Node child_to_add, int child_to_add_idx = INVALID_VALUE, copy_flags flag = copy_flags.SHALLOW)
        {
            // Find the first empty child which hasn't been assigned
            int child_idx = 0;
            if (m_children == null)
            {
                m_children = new Node[m_num_of_children];
                for (child_idx = 0; child_idx < m_num_of_children; child_idx++)
                    m_children[child_idx] = null;
            }

            if (child_to_add_idx == INVALID_VALUE)
            {
                while ((child_idx < m_num_of_children) && (m_children[child_idx] != null))
                {
                    child_idx++;
                }
            }
            else child_idx = child_to_add_idx;
            
            // Change m_if_leaf to false, because this method add a child to it and therefor it became an internal node
            m_if_leaf = false;

            if (child_idx == m_num_of_children) return false;
            else
            {
                // DEEP COPY if required
                // If flag = deep copy, create a new tmp_node and add this node as a child
                // without it's subtree
                // -----------------------------------------------------------------------
                if (flag == copy_flags.DEEP)
                {
                    Type t = this.GetType();
                    if (t.Equals(typeof(LogicNode)))
                        m_children[child_idx] = new LogicNode(child_to_add);

                    else m_children[child_idx] = new PartyNode(child_to_add);
                }
                // SHALLOW COPY if required (default)
                // If flag = shallow copy, the new child is the pointer that accepted 
                // "new_child", including it's subtree if exist.
                // -----------------------------------------------------------------------
                else if (flag == copy_flags.SHALLOW) m_children[child_idx] = child_to_add;

                this.m_children[child_idx].m_parent = this;
            }

            // Set children depth
            m_children[child_idx].m_depth = this.m_depth + 1;
            if (m_children[child_idx].m_if_leaf) return true;
            Tree tmp_tree_for_depth = new Tree(m_children[child_idx]);
            foreach (Node node in tmp_tree_for_depth)
            {
                node.m_depth = node.get_parent().get_depth() + 1;
            }
            return true;
        }

        /* Node::inc_last_child_idx()
         * The method increments the data member m_last_child_idx
         * [INPUT]:
         * void
         * [OUTPUT]:
         * void
         * ******************************************************** */
        public void inc_last_child_idx()
        {
            m_last_child_idx++;
        }

        /* Node::deep_copy()
         * The virtual method doing a deep copy a node without pointers to it's parent and children 
         * [INPUT]:
         * void
         * [OUTPUT]:
         * A deep copy to this (copied_node)
         * ***************************************************************************************** */
        public virtual Node deep_copy()
        {
            string name = m_name;
            node_type type = m_type;
            int depth = m_depth;
            int number_of_children = m_num_of_children;
            bool if_leaf = m_if_leaf;
            int leaf_idx = m_leaf_idx;

            Node copied_node = new Node(name, type, depth, number_of_children, /* parent = */null);
            copied_node.set_leaf_idx(leaf_idx);
            copied_node.set_if_leaf(if_leaf);

            return copied_node;
        } // End of method "deep_copy"
        
        // TODO: implementation
        /* Node::calculate_value()
         * The method calculate the value of a specific node using it's sub formula tree (it's children)
         * [INPUT]: 
         * input_vector = an array of integers represents the input value to it's sub formula tree
         * [OUTPUT]:
         * An integer stores the calculated value for this
         * ******************************************************************************************** */
       /* public virtual int calculate_value(int[] input_vector)
        {
            return 0;
        }*/ // End of method "calculate_value"

        /* Node::my_idx_as_a_child()
         * The method returns the index of the node in the m_children array of this.m_parent()
         * [INPUT]:
         * void
         * [OUTPUT]:
         * Index of the node in the m_children array of this.m_parent()
         * ***************************************************************************************** */
        public int my_idx_as_a_child()
        {
            Node[] parent_children = this.get_parent().get_children();
            int number_of_children = get_parent().get_num_of_children();

            for (int i = 0; i < number_of_children; i++ )
            {
                if (parent_children[i] == this)
                {
                    return i;
                }
            }
            return INVALID_VALUE;
        } // End of method "my_idx_as_a_child"

        public void init_last_child_idx()
        {
            m_last_child_idx = INVALID_VALUE;
        }

        public void init_is_copied()
        {
            m_is_copied = false;
        }


    }

}

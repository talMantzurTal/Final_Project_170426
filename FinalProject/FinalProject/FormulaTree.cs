using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject;

namespace FinalProject
{
    class FormulaTree : Tree
    {

        private int m_num_of_variables;

        public FormulaTree()
            : base()
        {
            Console.WriteLine("FormulaTree defualt c'tor");
            m_num_of_variables = 0;
            m_root = null;
            m_depth = 0;
            m_num_of_children = 0;
        }

        public FormulaTree(int num_of_children, LogicNode root_node) :
            base(num_of_children, (Node)root_node)
        {
            Console.WriteLine("FormulaTree c'tor");
            m_num_of_variables = 0;
            m_root = root_node;
            m_depth = 0;
            m_num_of_children = num_of_children;
        }

        public void add_child(string name, node_type type, LogicNode parent)
        {
            int new_node_depth = parent.get_depth() + 1;
            Node node_to_add = new LogicNode(name, type, new_node_depth, m_num_of_children, parent);
            bool result = parent.set_child((Node)node_to_add);
            // The tree depth is the longest path in the tree
            if (new_node_depth > m_depth) m_depth = new_node_depth;
        }

        public void calculate_formula(string input)
        {
            return;
        }

        public void number_leaves()
        {
            int leaf_count = 0;
            foreach (Node node in this)
            {

                if (node.get_if_leaf())
                {
                    node.set_leaf_idx(leaf_count);
                    leaf_count++;
                }
            }

            return;
        }
#if number_leaves
        public void number_leaves()
        {
            Node curr_node = new Node();
            curr_node = m_root;
            int leaf_idx = 0;
            int curr_last_child_idx = curr_node.get_last_child_idx();
            // Number the leaves while the parent isn't the root or it is the root but the program haven't checked all root's children yet
            while ((curr_node.get_node_name() != m_root.get_node_name()) || ((curr_last_child_idx + 1) < m_num_of_children))
            {
                curr_node.inc_last_child_idx();
                curr_last_child_idx = curr_node.get_last_child_idx();
                Node tmp_head = new Node();
                tmp_head = curr_node.get_child(curr_last_child_idx);
                tmp_head.inc_last_child_idx();
                int tmp_last_child_idx = tmp_head.get_last_child_idx();
                // If a node with depth 1 is a leaf, number it and don't try to get to it's children
                if (tmp_head.get_child(0) == null)
                {
                    tmp_head.set_leaf_idx(leaf_idx);
                    leaf_idx++;
                }
                else
                {
                    while (tmp_last_child_idx < m_num_of_children)
                    {
                        // If the grandchild is null, the child is a leaf
                        while (tmp_head.get_child(tmp_last_child_idx).get_child(0) != null)
                        {
                            tmp_head = tmp_head.get_child(tmp_last_child_idx);
                        }
                        // Reached a leaf ->  number it
                        tmp_head.get_child(tmp_last_child_idx).set_leaf_idx(leaf_idx);
                        leaf_idx++;
                        tmp_head.inc_last_child_idx();
                        tmp_last_child_idx = tmp_head.get_last_child_idx();
                    }
                }
                Console.WriteLine("node ({0})", curr_node.get_node_name());
                curr_node = tmp_head.get_parent();
            }
            m_num_of_variables = leaf_idx;
        }
#endif

        public Tree deep_copy() //vered!!!
        {
            /* perform deep copy of the root and use this copy in order to create a sub FormulaTree */
            Node cloned_root = m_root.deep_copy(); 
            LogicNode cloned_formula_root = new LogicNode(cloned_root);
            FormulaTree cloned_formula_tree = new FormulaTree(m_num_of_children, cloned_formula_root);
            return (this.deep_copy(/*cloned_formula_root,*/cloned_formula_tree));
        }

#if v
            Node curr_node = new Node();
            curr_node = m_root;
            int leaf_idx = 0;
            int curr_last_child_idx = curr_node.get_last_child_idx();
            // Number the leaves while the parent isn't the root or it is the root but the program haven't checked all root's children yet
            while ((curr_node.get_node_name() != m_root.get_node_name()) || ((curr_last_child_idx + 1) < m_num_of_children))
            {
                curr_node.inc_last_child_idx();
                curr_last_child_idx = curr_node.get_last_child_idx();
                Node tmp_head = new Node();
                tmp_head = curr_node.get_child(curr_last_child_idx);
                tmp_head.inc_last_child_idx();
                int tmp_last_child_idx = tmp_head.get_last_child_idx();
                // If a node with depth 1 is a leaf, number it and don't try to get to it's children
                if (tmp_head.get_child(0) == null)
                {
                    tmp_head.set_leaf_idx(leaf_idx);
                    leaf_idx++;
                }
                else
                {
                    while (tmp_last_child_idx < m_num_of_children)
                    {
                        // If the grandchild is null, the child is a leaf
                        while (tmp_head.get_child(tmp_last_child_idx).get_child(0) != null)
                        {
                            tmp_head = tmp_head.get_child(tmp_last_child_idx);
                        }
                        // Reached a leaf ->  number it
                        tmp_head.get_child(tmp_last_child_idx).set_leaf_idx(leaf_idx);
                        leaf_idx++;
                        tmp_head.inc_last_child_idx();
                        tmp_last_child_idx = tmp_head.get_last_child_idx();
                    }
                }
                curr_node = tmp_head.get_parent();
            }
            m_num_of_variables = leaf_idx;
        }


    }
}
#endif
    }
}
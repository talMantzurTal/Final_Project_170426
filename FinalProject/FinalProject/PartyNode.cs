using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;

namespace FinalProject
{
    class PartyNode : Node
    {
        private Node m_sub_formula_tree;
        private PartyNode m_protocol_node_reference; // used for egh transformation
        private bool m_is_zero_padding;
        //public List<int[]> error_vectors_list; TAL
        //private int m_zero_node_counter;
        private reachable_type m_reachable;
        private bool m_protocol_end_flag;


        // C'TOR
        public PartyNode()
            : base()
        {
            m_children = new PartyNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            // TODO - KW function creats the party node with a suitable "root" of sub_formula_tree
            // m_sub_formula_tree = new FormulaTree(m_num_of_children,null);
            m_protocol_node_reference = null;
            m_is_zero_padding = false;
            m_protocol_end_flag = false;
            // TAL error_vectors_list = new List<int[]>();
            //m_zero_node_counter = 0;
        }

        public PartyNode(string name, node_type type, int depth, int number_of_children, PartyNode parent = null, Node f_tree = null) :
            base(name, type, depth, number_of_children, parent)
        {
            m_children = new PartyNode[m_num_of_children];
            m_is_zero_padding = false;
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            // TODO - KW function creats the party node with a suitable "root" of sub_formula_tree
            // m_sub_formula_tree = new FormulaTree(m_num_of_children,null);
            m_sub_formula_tree = f_tree;
            m_protocol_node_reference = null;
            m_is_zero_padding = false;
            m_protocol_end_flag = false;
            // TAL error_vectors_list = new List<int[]>();
            //m_zero_node_counter = 0;
        }

        public PartyNode(Node node)
            : base(node)
        {
            // TAL error_vectors_list = new List<int[]>();
            // m_sub_formula_tree = node;
            Type t = node.GetType();
            if (t == typeof(FinalProject.LogicNode))
            {
                m_sub_formula_tree = node;
                m_num_of_children++;
                Array.Resize(ref m_children, m_num_of_children);
                m_children[m_num_of_children - 1] = null;
            }
            else
            {
                m_children = new PartyNode[m_num_of_children]; //in case that typeof = Node preform only this command
                if (t == typeof(FinalProject.PartyNode)) //vered!!
                {
                    PartyNode tmp_node = (PartyNode)node; //vered!!!!!!!
                    m_sub_formula_tree = tmp_node.get_sub_formula_tree_ptr(); //vered!!!
                    m_protocol_node_reference = null;
                }

            }

            if (node.get_type() == node_type.AND)
            {
                m_type = node_type.ALICE;
            }
            else if (node.get_type() == node_type.OR)
            {
                m_type = node_type.BOB;
            }
            else if ((node.get_type() == node_type.ALICE) || (node.get_type() == node_type.BOB))
            {
                return;
            }
            else
            {
                throw new System.ArgumentException("Invalid type for Node, suppose to be AND or OR", node.get_type().ToString());
            }
            m_is_zero_padding = false;
            m_protocol_end_flag = false;
            //m_zero_node_counter = 0;
        }

        // GETTERS
        public Node get_sub_formula_tree_ptr()
        {
            return (m_sub_formula_tree);
        }

        public PartyNode get_protocol_node_reference()
        {
            return m_protocol_node_reference;
        }

        public bool get_is_zero_padding()
        {
            return m_is_zero_padding;
        }

       /* TAL public List<int[]> get_error_vectors_list()
        {
            return error_vectors_list;
        }*/

        public reachable_type get_reachable()
        {
            return m_reachable;
        }

       /* public int get_zero_node_counter()
        {
            return m_zero_node_counter;
        }*/


        // SETTERS
        /* Node::set_sub_formula_tree_ptr()
         * The method sets a pointer to the matching sub-formula of this
         * [INPUT]:
         * f_tree = a pointer to the root of a sub-formula tree
         * [OUTPUT]:
         * void
         * ******************************************************** */
        public void set_sub_formula_tree_ptr(Node f_tree)
        {
            m_sub_formula_tree = f_tree;
        }

        public void set_is_zero_padding(bool flag = true)
        {
            m_is_zero_padding = flag;
        }

        public void set_reachable(reachable_type reachable_in)
        {
            m_reachable = reachable_in;
        }

       /* public void set_zero_node_counter(int zero_node_counter)
        {
            m_zero_node_counter = zero_node_counter;
        }

        public void inc_zero_node_counter()
        {
            m_zero_node_counter++;
        }

        public void dec_zero_node_counter()
        {
            m_zero_node_counter -= 2;
        }*/

        public override void set_node(Node reference_node)
        {
            PartyNode p_reference_node = (PartyNode)reference_node;
            base.set_node(reference_node);
            m_sub_formula_tree = p_reference_node.get_sub_formula_tree_ptr(); 
            m_protocol_node_reference = p_reference_node.get_protocol_node_reference();
            m_is_zero_padding = p_reference_node.get_is_zero_padding();
            //error_vectors_list = p_reference_node.get_error_vectors_list();
           // m_zero_node_counter = p_reference_node.get_zero_node_counter();
            m_reachable = p_reference_node.get_reachable();
        }

        // METHODS 

        /* PartyNode::deep_copy()
         * The method deep copying the PartyNode this and it's overrides the method in Node
         * [INPUT]:
         * void
         * [OUTPUT]:
         * A deep copy to this
         * ******************************************************** */
        public override Node deep_copy() 
        {
            PartyNode party_node_cpy = new PartyNode(base.deep_copy());
            party_node_cpy.m_sub_formula_tree = this.m_sub_formula_tree;
            party_node_cpy.m_protocol_node_reference = this; // TAL
            return (party_node_cpy);
        }


        /* PartyNode::is_reachable()
         * This method check if a specified node is reachable.
         * For each given error vector:
         * start from root and traverse kw_tree according to current error_vector and the given egh_path
         * in case that the path ends with a leaf - the node is reachable, stop checking. else, 
         * check next error vector in the given list.
         * In case that the method done iterating all error vectors- the node isn't reachable.
         * The tour over the tree is performed according to reachability algorithm:
         * In case that no error occured: move to the i't child according to EGH given path.
         * In case of error: As long as the error symbol - i, isn't the reverse symbol, continue
         * to the i't child. In case the symbol is the reverse symbol - go back to grandparent.
         * Note that if the current node has depth less than 2 (doesn't have grandparent), stay at 
         * current node.
         * In addition, the EGH path vector is larger than KW tree's depth. In order to simulate the
         * protocol correctly, when arrive to a leaf node, start transmitting zeros (each leaf has zero
         * counter which defines the location in the simulated protocol (which has the same length as EGH protocol).
         * 
         * [INPUT]:
         * error_vectors_per_node = List of error vectors which suitable to a specified node. each one of 
         * those vectors defines a path from root to node including some errors. (contain all egal options).
         * 
         * egh_path_to_node  = list of child indices which define a path from root to node in EGH tree.
         * 
         * [OUTPUT]:
         * None
         * ********************************************************************************************************************/
        public void is_reachable(ProtocolTree kw_tree, List<int[]> error_vectors_per_node, int[] egh_path_to_node)
        {
            this.set_reachable(reachable_type.UNREACHABLE);
            int iterate_idx = 0;
            PartyNode curr_node = (PartyNode)kw_tree.get_root();
            PartyNode tmp_egh_node = new PartyNode();
            PartyNode tmp_node = new PartyNode(); 

            // If this doesn't have children but isn't a leaf in the KW tree --> set as UNREACHABLE
            if ( ( (this.get_children() == null) || (this.get_child(0) == null)) && (!this.get_if_leaf() ) )
            {
                this.set_reachable(reachable_type.UNREACHABLE);
                return;
            }
            foreach ( int[] error_vector in error_vectors_per_node)
            {
                iterate_idx = 0;
                curr_node = (PartyNode)kw_tree.get_root();
                while ( iterate_idx < error_vector.Length )
                {
                    if ( curr_node.get_if_leaf() )
                    {
                        if ((error_vector[iterate_idx] == (curr_node.get_num_of_children() - 1)) || (egh_path_to_node[iterate_idx] == (curr_node.get_num_of_children() - 1)))
                        {
                            if (curr_node.get_depth() >= 2)
                                curr_node = (PartyNode)curr_node.get_parent().get_parent();
                        }
                        else
                        {
                            break;  // Protocol starts to send zeroes --> protocol ends with a leaf and the node is REACHABLE
                        }
                        /*if (error_vector[iterate_idx] == globals.NO_ERROR)
                            curr_node.inc_zero_node_counter();
                        else if ((error_vector[iterate_idx] < (curr_node.get_num_of_children() - 1)) && (error_vector[iterate_idx] >= 0))
                            curr_node.inc_zero_node_counter();
                        else throw new System.ArgumentException("Can't have an error equals to {0}", error_vector[iterate_idx].ToString());*/
                    }
                    else // curr_node isn't a leaf
                    {
                        if (error_vector[iterate_idx] == (curr_node.get_num_of_children() - 1))
                        {
                            // Check if the depth of curr_node >= 2. In that case, back to the grandparent
                            // else (if depth of curr_node < 2), stay on curr_node
                            if (curr_node.get_depth() >= 2)
                                curr_node = (PartyNode)curr_node.get_parent().get_parent();
                        }
                        // If there was no error -> go by the original intention: follow the egh_path
                        else if (error_vector[iterate_idx] == globals.NO_ERROR)
                        {
                            // If in the egh_tree the next step is child #2 => reverse to grandparent only if the depth >= 2
                            if ( egh_path_to_node[iterate_idx] == (curr_node.get_num_of_children() - 1) )
                            {
                                if (curr_node.get_depth() >= 2)
                                    curr_node = (PartyNode)curr_node.get_parent().get_parent();
                            }
                            else curr_node = (PartyNode)curr_node.get_child(egh_path_to_node[iterate_idx]);
                        }
                        else if ((error_vector[iterate_idx] < (curr_node.get_num_of_children() - 1)) && (error_vector[iterate_idx] >= 0))
                            curr_node = (PartyNode)curr_node.get_child(error_vector[iterate_idx]);
                        else throw new System.ArgumentException("Can't have an error equals to {0}", error_vector[iterate_idx].ToString());
                    }
                    if ( curr_node.get_if_leaf() )
                    {
                        tmp_egh_node = this;
                        //tmp_egh_node.m_reachable = reachable_type.REACHABLE;
                        // update that all path from root to this is reachble
                        while (tmp_egh_node.get_parent() != null)
                        {
                            tmp_egh_node.set_reachable(reachable_type.REACHABLE);
                            tmp_egh_node = (PartyNode)tmp_egh_node.get_parent();
                        }
                        tmp_egh_node.set_reachable(reachable_type.REACHABLE);
                        return;
                        }
                    iterate_idx++;
                }
            }
#if design
            /* this.reachble = false;
             * 
             * iterte all error vectors:
             * foreach (error_vector):
             *      start from root and traverse kw_tree according to the error_vector and to the egh_path:
             *      
             *      if ( curr_node.is_leaf() )
             *      {
             *          error_vector[i] = m_num_children - 1 => curr_node.cpunter -= 2;
             *          error_vector[i] = globals.NO_ERROR => curr_node.cpunter += 1;
             *          0 <= error_vector[i] < m_num_children - 1  => curr_node.cpunter += 1;
             *      }
             *      else
             *          error_vector[i] = m_num_children - 1 => return to the grand parent. if depth < 2 => don't move.
             *          error_vector[i] = globals.NO_ERROR => go the m_children[ egh_path_to_node[i] ].
             *          0 <= error_vector[i] < m_num_children - 1 , m_children[ error_vector[i] ].
             *          else, throw
             *          
             *      if ( curr_node.is_leaf ) 
             *      {
             *          this.reachble = true;
             *          // update that all path from root to this is reachble
             *          while ( tmp_node.get_parent != null )
             *              tmp_node.reable = true;
             *              tmp_node = tmp_node.get_parent;
             *          return;
             *      }
             */
#endif
        } // End of "is_reachable"
    }
}

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
        public List<int[]> error_vectors_list;
        private int m_zero_node_counter;
        private reachable_type m_reachable;


        // C'TOR
        public PartyNode()
            : base()
        {
            Console.WriteLine("PartyNode defult c'tor");
            m_children = new PartyNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            // TODO - KW function creats the party node with a suitable "root" of sub_formula_tree
            // m_sub_formula_tree = new FormulaTree(m_num_of_children,null);
            m_protocol_node_reference = null;
            m_is_zero_padding = false;
            error_vectors_list = new List<int[]>();
            m_zero_node_counter = 0;
        }

        public PartyNode(string name, node_type type, int depth, int number_of_children, PartyNode parent = null, Node f_tree = null) :
            base(name, type, depth, number_of_children, parent)
        {
            Console.WriteLine("PartyNode c'tor");
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
            error_vectors_list = new List<int[]>();
            m_zero_node_counter = 0;
        }

        public PartyNode(Node node)
            : base(node)
        {
            error_vectors_list = new List<int[]>();
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
            m_zero_node_counter = 0;
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

        public List<int[]> get_error_vectors_list()
        {
            return error_vectors_list;
        }

        public reachable_type get_reachable()
        {
            return m_reachable;
        }

        public int get_zero_node_counter()
        {
            return m_zero_node_counter;
        }


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

        public void set_zero_node_counter(int zero_node_counter)
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


#if change
        public List<int[]> generate_error_vectors(Node node)
        {
            PartyNode parent = (PartyNode)this.get_parent();
            PartyNode cur_node = (PartyNode)node;
            int[] generic_error_array = new int[node.get_depth()];

            int my_idx_as_a_child = INVALID_VALUE;
            int ERROR = m_num_of_children - 1; // The index of the error child. Used to generate a generic error vector.
            int node_depth = node.get_depth();
            int num_of_errors = 0;

            // iterate over all nodes in path from node to root
            for (int cur_node_depth = node_depth; cur_node_depth>0; cur_node_depth--)
            {
                my_idx_as_a_child = cur_node.my_idx_as_a_child();

                // Store a generic value for the error vactor: error/no error
                if (my_idx_as_a_child != ERROR)
                    generic_error_array[cur_node_depth] = Globals.NO_ERROR;
                else
                {
                    generic_error_array[cur_node_depth] = ERROR;
                    num_of_errors++;
                }
            }

            // Generate vector
            
            int[] error_vector_to_generate = new int[num_of_errors];
            //List<int[]> error_vectors_list = new List<int[]>();
            int num_of_error_vectors = (int)Math.Pow(m_num_of_children + 1, node.get_depth());
            int error_vector_length = node.get_depth();
            int[][] error_vectors_list = new int[num_of_error_vectors][];
            for (int i = 0; i < num_of_error_vectors; i++)
                error_vectors_list[i] = new int[error_vector_length];
            generate_alphabeth_vectors(error_vector_to_generate, 0, num_of_errors, error_vectors_list);

            return null;
        }

#endif
        /* PartyNode::generate_alphabeth_vectors()
         * The recursive method generates all vectors with a d-ary alphabeth in size of a specific size. 
         * The vectors store in a data member of type List<int[]> 
         * 
         * [INPUT]:
         * error_vector     = The generated vector in each point
         * last_cell_in     = The last cell the function filled in the error_vector
         * vector_size      = depth of a PartyNode in the EGH tree
         * 
         * [OUTPUT]:
         * void
         * ******************************************************************************************************************* */
        public void generate_alphabeth_vectors(int[] error_vector, int last_cell_in, int vector_size, int alphabeth_size = 2)
        {
            // Stop condition
            if ( last_cell_in == (vector_size) )
            {
                int[] tmp_vector = (int[])error_vector.Clone();
                error_vectors_list.Add(tmp_vector);
                return;
            }
            
            for (int i = 0; i < alphabeth_size; i++)
            {
                error_vector[last_cell_in] = i;
                generate_alphabeth_vectors(error_vector, last_cell_in+1, vector_size);
                
            }
        } // End of "generate_alphabeth_vectors"

        public List<int> get_real_egh_path()
        {
            List<int> path = new List<int>();
            PartyNode curr_node = this;
            while( curr_node.get_parent() != null )
            {
                path.Add(curr_node.my_idx_as_a_child());
                curr_node = (PartyNode)curr_node.get_parent();
            }
            path.Reverse();
            return path;
        } // End of "get_real_egh_path"
  
        public List<int[]> limit_num_of_errors(List<int[]> optional_binary_vectors)
        {
            int vector_sum = 0;
            int N = optional_binary_vectors[0].Length;
            int max_num_of_errors = (int)(N * Globals.error_fraction);
            bool flag_illegal_error = false;

            List<int[]> binary_vectors_to_return = new List<int[]>();

            foreach(int[] error_vector in optional_binary_vectors)
            {
                vector_sum = 0;
                for (int cell_idx = 0; cell_idx < error_vector.Length; cell_idx++)
                {
                    vector_sum += error_vector[cell_idx];
                    if ( vector_sum > max_num_of_errors )
                    {
                        // Removes illegal vectors
                        flag_illegal_error = true;
                        break;
                    }
                    if (error_vector[cell_idx] == 0)
                        // (cells == 0) --> indicates that there is no error
                        error_vector[cell_idx] = Globals.NO_ERROR;
                }
                if (flag_illegal_error == false)
                    binary_vectors_to_return.Add(error_vector);

            }
            return binary_vectors_to_return;
        } // End of "limit_num_of_errors"

        public List<int[]> generate_legel_vectors(List<int[]> legal_error_vectors, List<int> real_egh_path)
        {
            List<int[]> error_vectors_per_node = new List<int[]>();
            int error_vector_length = legal_error_vectors[0].Length;

            foreach (int[] error_vector in legal_error_vectors)
            {
                int[] tmp_legal_error_vector = new int[error_vector_length];
                for (int cell_idx = 0; cell_idx < error_vector_length; cell_idx++)
                {
                    if (error_vector[cell_idx] == (int)Globals.NO_ERROR)
                    {
                        tmp_legal_error_vector[cell_idx] = Globals.NO_ERROR;
                    }
                    else // ERROR
                    {
                        tmp_legal_error_vector[cell_idx] = real_egh_path[cell_idx];
                    }

                }
                error_vectors_per_node.Add(tmp_legal_error_vector);
            }

            return error_vectors_per_node;
        } // End of "generate_leagel_vectors"

        public void is_reachable(ProtocolTree kw_tree, List<int[]> error_vectors_per_node, List<int> egh_path_to_node)
        {
            this.set_reachable(reachable_type.UNREACHABLE);
            int iterate_idx = 0;
            PartyNode curr_node = (PartyNode)kw_tree.get_root();

            foreach ( int[] error_vector in error_vectors_per_node)
            {
                iterate_idx = 0;
                curr_node = (PartyNode)kw_tree.get_root();
                while ( iterate_idx < error_vector.Length )
                {
                    if ( curr_node.get_if_leaf() )
                    {
                        if ( error_vector[iterate_idx] == (curr_node.get_num_of_children() - 1) )
                            curr_node.dec_zero_node_counter();
                        else if ( error_vector[iterate_idx] == Globals.NO_ERROR )
                            curr_node.inc_zero_node_counter();
                        else if ( ( error_vector[iterate_idx] < (curr_node.get_num_of_children() - 1)) && ( error_vector[iterate_idx] >= 0 ) )
                            curr_node.inc_zero_node_counter();
                        else throw new System.ArgumentException("Can't have an error equals to {0}", error_vector[iterate_idx].ToString());
                    }
                    else // curr_node isn't a leaf
                    {
                        if (error_vector[iterate_idx] == (curr_node.get_num_of_children() - 1))
                        {
                            // Check if the depth of curr_node >= 2. In that case, back to the grandparent
                            // else (if depth of curr_node < 2), stay on curr_node
                            if (curr_node.get_depth() >= 2)
                                curr_node.get_parent().get_parent();
                        }
                        else if (error_vector[iterate_idx] == Globals.NO_ERROR)
                            // If there was no error -> go by the original intention: follow the egh_path
                            curr_node = (PartyNode)curr_node.get_child( egh_path_to_node[iterate_idx] );
                        else if ((error_vector[iterate_idx] < (curr_node.get_num_of_children() - 1)) && (error_vector[iterate_idx] >= 0))
                            curr_node = (PartyNode)curr_node.get_child( error_vector[iterate_idx] );
                        else throw new System.ArgumentException("Can't have an error equals to {0}", error_vector[iterate_idx].ToString());
                    }
                    if ( curr_node.get_if_leaf() )
                    {
                        this.m_reachable = reachable_type.REACHABLE;
                        // update that all path from root to this is reachble
                        while (curr_node.get_parent() != null)
                        {
                            curr_node.set_reachable(reachable_type.REACHABLE);
                            curr_node = (PartyNode)curr_node.get_parent();
                        }
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
             *          error_vector[i] = Globals.NO_ERROR => curr_node.cpunter += 1;
             *          0 <= error_vector[i] < m_num_children - 1  => curr_node.cpunter += 1;
             *      }
             *      else
             *          error_vector[i] = m_num_children - 1 => return to the grand parent. if depth < 2 => don't move.
             *          error_vector[i] = Globals.NO_ERROR => go the m_children[ egh_path_to_node[i] ].
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

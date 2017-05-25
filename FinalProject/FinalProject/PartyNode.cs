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
        private string m_input;
        private PartyNode m_protocol_node_reference; // used for egh transformation
        private bool m_is_zero_padding;
        public List<int[]> error_vectors_list;
        public static int idx_list = 0;

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
            m_input = null;
            m_protocol_node_reference = null;
            m_is_zero_padding = false;
            error_vectors_list = new List<int[]>();
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
            m_input = null;
            m_sub_formula_tree = f_tree;
            m_protocol_node_reference = null;
            m_is_zero_padding = false;
            error_vectors_list = new List<int[]>();
        }

        public PartyNode(Node node)
            : base(node)
        {
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
                   error_vectors_list = new List<int[]>();
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
        public void generate_alphabeth_vectors(int[] error_vector, int last_cell_in, int vector_size)
        {
            int alphabeth_size = m_num_of_children + 1;
            
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


    }
}

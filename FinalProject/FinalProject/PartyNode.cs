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

        public override Node deep_copy() //vered!!
        {
            //  Node tmp_node_cpy = base.deep_copy();
            PartyNode party_node_cpy = new PartyNode(base.deep_copy());
            party_node_cpy.m_sub_formula_tree = this.m_sub_formula_tree;
            party_node_cpy.m_protocol_node_reference = this; // TAL
            return (party_node_cpy);
        }
    

        public void set_sub_formula_tree_ptr(Node f_tree)
        {
            m_sub_formula_tree = f_tree;

        }

        public Node get_sub_formula_tree_ptr()
        {
            return (m_sub_formula_tree);

        }

        public PartyNode get_protocol_node_reference()
        {
            return m_protocol_node_reference;
        }

        public void set_is_zero_padding(bool flag = true)
        {
            m_is_zero_padding = flag;
        }

        public bool get_is_zero_padding()
        {
            return m_is_zero_padding;
        }

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

            return null;
        }




    }
}

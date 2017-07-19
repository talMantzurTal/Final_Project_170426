using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject;

namespace FinalProject
{
    class LogicNode : Node
    {
        // DATA MEMBERS
        protected Boolean m_value;

        // C'TORS
        public LogicNode()
            : base()
        {
            m_children = new LogicNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            //m_literal_idx = 0;
            m_value = false;
        }

        public LogicNode(string name, node_type type, int depth, int number_of_children, LogicNode parent) :
            base(name, type, depth, number_of_children, parent)
        {
            m_children = new LogicNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            //m_literal_idx = 0;
            m_value = false;
        }

        public LogicNode(Node node)
            : base(node)
        {

            m_children = new LogicNode[m_num_of_children]; //in case that typeof = Node preform only this command
            Type t = node.GetType();
            if (t == typeof(FinalProject.LogicNode))
            {
                m_value = false;
            }
            else
            {
                if (t == typeof(FinalProject.PartyNode)) 
                {
                    if (node.get_type() == node_type.ALICE)
                    {
                        m_type = node_type.AND;
                    }
                    else if (node.get_type() == node_type.BOB)
                    {
                        m_type = node_type.OR;
                    }
                    else if ((node.get_type() == node_type.AND) || (node.get_type() == node_type.OR))
                    {
                        return;
                    }
                    else
                    {
                        throw new System.ArgumentException("Invalid type for Node, suppose to be ALICE or BOB", node.get_type().ToString());
                    }
                }
            }

        } // End of "LogicNode(Node node)"

        // GETTERS
        public Boolean get_value()
        {
            return m_value;
        }

        // SETTERS
        public void set_value(Boolean bit_value)
        {
            m_value = bit_value;
        }

        // METHODS

        /* LogicNode::deep_copy()
         * The method deep copying the LogicNode this and it's overrides the method in Node.
         * [INPUT]:
         * void
         * [OUTPUT]:
         * A deep copy to this
         * ********************************************************************************** */
        public override Node deep_copy()
        {
            Node tmp_node_cpy = this.deep_copy();
            LogicNode logic_node_cpy = new LogicNode(tmp_node_cpy);
            logic_node_cpy.m_value = this.m_value;
            return (logic_node_cpy);
        } // End of method "deep_copy"

        /* LogicNode::get_child()
         * The method gets a child name, search it in the m_children array and returns it.
         * [INPUT]:
         * child_name = A string. A child name to search.
         * [OUTPUT]:
         * A pointer to the child node if the child exists in m_children, null otherwise.
         * ******************************************************************************** */
        public LogicNode get_child(string child_name)
        {
            for (int i = 0; i < m_num_of_children; i++)
                if ((m_children[i] != null) && (m_children[i].get_name() == child_name)) return (LogicNode)m_children[i];
            return null;
        } // End of method "get_child"

        /* LogicNode::calculate_value()
         * The method gets an input vector and calculate the value of the boolean function the formula tree describes.
         * [INPUT]:
         * input_vector = An array of int. The array stores the inputs to the boolean formula the formula_tree describes.
         *                The vector has been used for the calculation of the formula's output.
         * [OUTPUT]:
         * An int value stores the output value of the formula.
         * ************************************************************************************************************ */
        /*public int calculate_value(int[] input_vector)
        {
            // A recursive function
            // Stop condition - if the function gets to a literal, returns it's value according to the input vector
            if (m_if_leaf) return input_vector[m_leaf_idx];

            // else - if the node isn't a leaf (a gate) then calculate it's children values and execute or\and between it's values
            int left_child_value = m_children[0].calculate_value(input_vector);
            int right_child_value = m_children[1].calculate_value(input_vector);
            if (m_type == node_type.AND)
            {
                m_value = left_child_value & right_child_value;
            }
            else if (m_type == node_type.OR)
            {
                m_value = left_child_value | right_child_value;
            }
            else
            {
                throw new System.ArgumentException("Invalid type for Node, suppose to be AND or OR", m_type.ToString());
            }

            return m_value;
        } */// End of method "calculate_value"







    }
}

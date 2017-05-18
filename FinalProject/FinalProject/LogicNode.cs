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

        // protected int m_literal_idx;
        protected int m_value;

        public LogicNode()
            : base()
        {
            Console.WriteLine("LogicNode defult c'tor");
            m_children = new LogicNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            //m_literal_idx = 0;
            m_value = INVALID_VALUE;
        }
        public LogicNode(string name, node_type type, int depth, int number_of_children, LogicNode parent) :
            base(name, type, depth, number_of_children, parent)
        {
            Console.WriteLine("LogicNode c'tor");
            m_children = new LogicNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            //m_literal_idx = 0;
            m_value = INVALID_VALUE;
        }

        public LogicNode(Node node)
            : base(node)
        {
            m_value = INVALID_VALUE;
            m_children = new LogicNode[m_num_of_children];
        }

        public override Node deep_copy() //vered!!!
        {
            Node tmp_node_cpy = this.deep_copy();
            LogicNode logic_node_cpy = new LogicNode(tmp_node_cpy);
            logic_node_cpy.m_value = this.m_value;
            return (logic_node_cpy);
        }

        public LogicNode get_child(string child_name)
        {
            for (int i = 0; i < m_num_of_children; i++)
                if ((m_children[i] != null) && (m_children[i].get_name() == child_name)) return (LogicNode)m_children[i];
            return null;
        }

        public override int calculate_value(int[] input_vector)
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
        }

        // Indexers
        /* public LogicNode this [int idx]
         {
             get
             {
                 return; 
              }
         }*/





    }
}

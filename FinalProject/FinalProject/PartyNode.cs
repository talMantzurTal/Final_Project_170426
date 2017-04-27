using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class PartyNode : Node
    {
        private Node m_sub_formula_tree;
        private string m_input;

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
        }
        public PartyNode(string name, node_type type, int depth, int number_of_children, PartyNode parent, Node f_tree) :
            base(name, type, depth, number_of_children, parent)
        {
            Console.WriteLine("PartyNode c'tor");
            m_children = new PartyNode[m_num_of_children];
            for (int i = 0; i < m_num_of_children; i++)
            {
                m_children[i] = null;
            }
            // TODO - KW function creats the party node with a suitable "root" of sub_formula_tree
            // m_sub_formula_tree = new FormulaTree(m_num_of_children,null);
            m_input = null;
            m_sub_formula_tree = f_tree;
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
                m_children = new PartyNode[m_num_of_children];
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
        }

        public void set_sub_formula_tree_ptr(Node f_tree)
        {
            m_sub_formula_tree = f_tree;

        }
    }
}

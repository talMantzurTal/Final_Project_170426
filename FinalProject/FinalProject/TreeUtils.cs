#define XCV
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// Parsing to text file
//#define FILE_PATH @"C:\Users\vered\Documents\GitHub\Final_Project\tree_input.txt"


namespace FinalProject
{

    class TreeUtils
    {
#if XCV
        const string FILE_NAME_IN = @"C:\Users\mantz\Documents\GitHub\Final_Project_170202\Final_Project_170426\tree_input.txt";
        const string FILE_NAME_OUT = @"C:\Users\mantz\Documents\GitHub\Final_Project_170202\Final_Project_170426\tree_output.txt";
#endif

        public static FormulaTree read_tree_from_file()
        {
            FileStream fs;
            string line;
            int depth_counter = 0;
            bool flag_found_char = false;
            FormulaTree formula_tree = new FormulaTree();
            // Read the text file
            if (File.Exists(FILE_NAME_IN))
            {
                fs = new FileStream(FILE_NAME_IN, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fs);

                int i_alphabeth_size = (reader.Read()) - 48; // Read single char in the file
                // Create a new n-ary tree
                reader.ReadLine(); // Ignore the rest of the first line
                line = reader.ReadLine();
                string s_node_name = line[0].ToString();
                string s_node_type = line[1].ToString();
                node_type type;
                if (s_node_type == "0")
                {
                    type = node_type.AND;
                }
                else
                {
                    type = node_type.OR;
                }
                Console.WriteLine("tree c'tor({0},{1},{2})", i_alphabeth_size, s_node_name, type);

                // Initialize the root of the formula tree
                Node tmp_node = new LogicNode(s_node_name, type, 0, i_alphabeth_size, null);
                formula_tree.set_initial_tree(i_alphabeth_size, tmp_node);
                LogicNode logic_tmp_node = new LogicNode();
                LogicNode logic_parent_node = new LogicNode();
                while ((line = reader.ReadLine()) != null)
                {
                    logic_tmp_node = (LogicNode)formula_tree.get_root();

                    // Start adding a child only if it's not the root
                    for (int i = 2; i < line.Length; i = i + 2)
                    {
                        s_node_name = line[i].ToString();
                        s_node_type = line[i + 1].ToString();
                        logic_parent_node = logic_tmp_node;
                        if (s_node_type == "0")
                        {
                            type = node_type.AND;
                        }
                        else
                        {
                            type = node_type.OR;
                        }
                        logic_tmp_node = (LogicNode)logic_tmp_node.get_child(s_node_name.ToString());
                        if (logic_tmp_node == null)
                            formula_tree.add_child(s_node_name, type, logic_parent_node);
                        Console.WriteLine("Node.create+add(name={0},type={1})", line[i], line[i + 1]);
                    }
                }
                fs.Close();
                formula_tree.number_leaves();
            } // If file exists
            else throw new System.ArgumentException("read_tree_from_file: Text file {0} doesn't exist\n", FILE_NAME_IN);
            return formula_tree;
        } // End of function "read_tree_from_file"

        public static void write_tree_to_file(Tree tree_2_print)
        {
            PartyNode p_node = new PartyNode();

            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(FILE_NAME_OUT))
            {

                string node_name;
                string s_node_type = "";
                foreach (Node node in tree_2_print)
                {
                    node_name = node.get_name();
                    if ((node.get_type() == node_type.ALICE) || (node.get_type() == node_type.AND))
                    {
                        s_node_type = "0";
                    }
                    else if ((node.get_type() == node_type.BOB) || (node.get_type() == node_type.OR))
                    {
                        s_node_type = "1";
                    }

                    /*
                    p_node = (PartyNode)node;
                    if (p_node.get_reachable() == reachable_type.REACHABLE)
                    {*/
                        string line = node_name + s_node_type;
                        fs.WriteLine(line);
                    //}
                }
            }
        } // End of function "write_tree_to_file"

    }

    
}

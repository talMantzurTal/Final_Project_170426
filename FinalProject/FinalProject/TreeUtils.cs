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
        const string FILE_NAME_IN_TMP = @"C:\Users\mantz\Documents\GitHub\Final_Project_170202\Final_Project_170426\tree_input_tmp.txt";
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
               // while ( (line[0].ToString() != "0") && (line[0].ToString() != "1")
               //     s_node_name += 
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

                // Initialize the root of the formula tree
                Node tmp_node = new LogicNode(s_node_name, type, 0, i_alphabeth_size, null);
                formula_tree.set_initial_tree(i_alphabeth_size, tmp_node);
                LogicNode logic_tmp_node = new LogicNode();
                LogicNode logic_parent_node = new LogicNode();
                while ((line = reader.ReadLine()) != null)
                {
                    logic_tmp_node = (LogicNode)formula_tree.get_root();

                    // Start adding a child only if it's not the root
                    int i = 2;
                    while (i < line.Length)
                    {
                        s_node_name = "";
                        while ((line[i] != '0') && (line[i] != '1'))
                        {
                            s_node_name += line[i].ToString();
                            i++;
                        }
                        s_node_type = line[i].ToString();
                        i++;
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
                    }
#if t
                    for (/*int*/ i = 2; i < line.Length; i = i + 2)
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
                        // Console.WriteLine("Node.create+add(name={0},type={1})", line[i], line[i + 1]);
                    }
#endif
                }
                fs.Close();
                formula_tree.number_leaves();
                formula_tree.number_gates_preorder();
            } // If file exists
            else throw new System.ArgumentException("read_tree_from_file: Text file {0} doesn't exist\n", FILE_NAME_IN);
            return formula_tree;
        } // End of function "read_tree_from_file"

        public static void write_tree_to_file(Tree tree_2_print, string file_name = FILE_NAME_OUT, bool flag_complete_tree = false)
        {
            Node tmp_node = new Node();
           

            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(file_name))
            {

                string node_name;
                string s_node_type = "", s_root_type = "";
                List<int> path_from_root;
                string line = "";
                string root_name = tree_2_print.get_root().get_name();
                if ((tree_2_print.get_root().get_type() == node_type.ALICE) || (tree_2_print.get_root().get_type() == node_type.AND))
                {
                    s_root_type = "0";
                }
                else if ((tree_2_print.get_root().get_type() == node_type.BOB) || (tree_2_print.get_root().get_type() == node_type.OR))
                {
                    s_root_type = "1";
                }
                string root_prefix = root_name + s_root_type;

                if (flag_complete_tree)
                {
                    line = tree_2_print.get_root().get_num_of_children().ToString();
                    fs.WriteLine(line);
                }
                foreach (Node node in tree_2_print)
                {
                    path_from_root = node.get_path_from_root();
                    line = root_prefix;
                    tmp_node = tree_2_print.get_root();
                    // Print all path from root to node
                    for (int i = 0; i < path_from_root.Count; i++)
                    {
                        tmp_node = tmp_node.get_child(path_from_root[i]);
                        node_name = tmp_node.get_name();
                        if ((tmp_node.get_type() == node_type.ALICE) || (tmp_node.get_type() == node_type.AND))
                        {
                            s_node_type = "0";
                        }
                        else if ((tmp_node.get_type() == node_type.BOB) || (tmp_node.get_type() == node_type.OR))
                        {
                            s_node_type = "1";
                        }
                        line += node_name + s_node_type;
                        
                    }
                    fs.WriteLine(line);/////////////////
                }

                fs.Close();
            }
        } // End of function "write_tree_to_file"

        public static void set_tree_names(int tree_depth)
        {
            // Create zero tree with depth == tree_depth
            PartyNode p_node = new PartyNode("0", node_type.NA, 0, 2);
            ProtocolTree zero_tree = new ProtocolTree(p_node);
            zero_tree = (ProtocolTree)zero_tree.create_zero_sub_tree(tree_depth);

            string last_name = "a", curr_name = "";
            foreach (Node node in zero_tree)
            {
                node.set_name(last_name);
                curr_name = TreeUtils.find_next_name(last_name);
                last_name = curr_name;
            }
            TreeUtils.write_tree_to_file(zero_tree, FILE_NAME_IN_TMP, true);
        }
#if t
        public static void create_new_complete_tree_file(int tree_depth, int alphabeth_size = 2)
        {
            // Create zero tree with depth == tree_depth
            PartyNode p_node = new PartyNode("0",node_type.NA,0,2);
            ProtocolTree zero_tree = new ProtocolTree(p_node);
            zero_tree = (ProtocolTree)zero_tree.create_zero_sub_tree(tree_depth);

            // Print the rows of the file using preorder 
            string line_to_print = "2";
            int curr_depth = 0;
            string last_name_in_path = ((char)96).ToString();
            string last_name_in_tree = "a";
            string name = "";
            int last_type_in_path = 0, type = 0;


            char[] az = Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (Char)i).ToArray();

            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(FILE_NAME_IN_TMP))
            {
                fs.WriteLine(line_to_print);
                line_to_print = "";
                // Print a row for each node. number of nodes in complete binary tree = 2^(depth+1) -1
                int num_of_rows = (int)Math.Pow(2, tree_depth + 1) + 1;
                //for (int idx_num_of_rows = 0; idx_num_of_rows < num_of_rows; idx_num_of_rows++)
                foreach (Node node in zero_tree)
                {
                    // In each row, print the path from root to current node (maximum tree_depth + 1 nodes)
                    for (int number_of_nodes = 0; number_of_nodes < node.get_depth() + 1; number_of_nodes++)
                    {
                        if ( number_of_nodes == 0 )
                        {
                            line_to_print = "a1";
                            last_name_in_path = "a";
                            last_type_in_path = 1;
                            name = "a";
                            continue;
                        }
                        string next_name_in_path = (TreeUtils.find_next_name(last_name_in_path));
                        if ( (number_of_nodes == node.get_depth()) || ((!node.get_if_leaf()) && ( last_name_in_tree != next_name_in_path)))
                        // Update node's name using last_name_in_tree
                        {
                            name = TreeUtils.find_next_name(last_name_in_tree);
                            last_name_in_path = name;
                        }
                        else
                        // Update node's name using last_name_in_path
                        {
                            name = TreeUtils.find_next_name(last_name_in_path);
                            last_name_in_path = name;
                        }
                        line_to_print += name;
                        type = 1 - last_type_in_path;
                        line_to_print += type.ToString();
                        
                        last_type_in_path = type; 
                    }

                    last_name_in_tree = name;
                    fs.WriteLine(line_to_print);
                    if (curr_depth < tree_depth) curr_depth++;
                    else curr_depth = 1;
                    

                }
            }

        }
#endif
        public static string find_next_name(string last_name_in)
        {
            string name = "";
            int tmp_char_ascii = 0, idx_z = 0;

            if (last_name_in[last_name_in.Length - 1] != 'z')
            {
                StringBuilder sb = new StringBuilder(last_name_in);
                tmp_char_ascii = (int)sb[last_name_in.Length - 1] + 1;
                sb[last_name_in.Length - 1] = (char)tmp_char_ascii;
                name = sb.ToString();
            }
            else // last_name[last_name.Length - 1] == 'z'
            {
                idx_z = last_name_in.Length - 1;
                while ((last_name_in[idx_z] == 'z') && (idx_z > 0))
                {
                    StringBuilder sb = new StringBuilder(last_name_in);
                    sb[idx_z] = 'a';
                    if (last_name_in[idx_z - 1] != 'z')
                    {
                        tmp_char_ascii = (int)sb[last_name_in.Length - 2] + 1;
                        sb[idx_z - 1] = (char)tmp_char_ascii;
                    }
                    last_name_in = sb.ToString();
                    idx_z--;
                }
                name = last_name_in;
                if ((last_name_in[idx_z] == 'z') && (idx_z == 0))
                {
                    name = "";
                    for (int i = 0; i < last_name_in.Length + 1; i++)
                    {
                        name += 'a';
                    }
                }
            }
            return name;
        }
    }

    


        }

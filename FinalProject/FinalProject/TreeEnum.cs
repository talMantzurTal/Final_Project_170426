using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class TreeEnum : IEnumerator
    {
        // Enumerator
        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        // Node current_node;
        Stack<Node> s;
        List<Node> preorder_queue;

        int position = -1;
        public TreeEnum(Tree tree)
        {
            s = new Stack<Node>();
            s.Push(tree.get_root());
            preorder_queue = new List<Node>();
        }


        public bool MoveNext()
        {

            while (s.Count > 0)
            {
                Node n = s.Pop();
                preorder_queue.Add(n);

                // Do Action
                Console.Write(n.get_name());
                if (n.get_children() == null)
                {
                    position++;
                    return (position < preorder_queue.Count());
                }
                foreach (var child in n.get_children().ToArray().Reverse())
                {
                    if (child != null)
                    {
                        s.Push(child);
                    }
                }
            }
            position++;
            return (position < preorder_queue.Count());

        }

        public void Reset()
        {
            position = -1;
            s.Clear();
            preorder_queue.Clear();
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public object Current
        {
            get
            {
                try
                {
                    return preorder_queue[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

}

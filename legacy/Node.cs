using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskA
{
    public class Node
    {
        private string data;
        public Node Left, Right;

        public Node(string item)
        {
            data = item;
            Left = null;
            Right = null;
        }

        public string Data
        {
            set { data = value; }
            get { return data; }
        }
    }
}

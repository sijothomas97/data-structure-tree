using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskA
{
    public class BinTree
    {
        private Node root;

        public BinTree()
        {
            root = null;
        }

        private void insertItem(string item, ref BinTree tree)
        {
            if (tree == null)
            {
                Console.WriteLine("Making new tree :  " + item);
                tree = new Node(item);
            }
            else if (item.Length <= root.Data.Length)
            {
                Console.WriteLine("Going left :  " + item);
                insertItem(item, ref tree.Left);
            }
            else if (item.Length > root.Data.Length)
            {
                Console.WriteLine("Going right :  " + item);
                insertItem(item, tree.Right);
            }
        }

        public void InsertItem(string item, ref BinTree tree)
        {
            insertItem(item, ref tree);
        }

        private void inOrder(Node tree)
        {
            if (tree != null)
            {
                inOrder(tree.Left);
                Console.WriteLine(tree.Data + ",");
                inOrder(tree.Right);
            }
        }

        public void InOrder()
        {
            inOrder(root);
        }

        private void preOrder(Node tree)
        {
            if (tree != null)
            {
                Console.WriteLine(tree.Data + ",");
                preOrder(tree.Left);
                preOrder(tree.Right);
            }
        }

        public void PreOrder()
        {
            preOrder(root);
        }

        private void postOrder(Node tree)
        {
            if (tree != null)
            {
                postOrder(tree.Left);
                postOrder(tree.Right);
                Console.WriteLine(tree.Data + ",");
            }
        }

        public void PostOrder()
        {
            postOrder(root);
        }

        private Boolean contains(string item, Node tree)
        {
            if (tree != null)
            {
                if (tree.Data.Length < item.Length)
                {
                    return contains(item, tree.Right);
                }
                else if (tree.Data.Length > item.Length)
                {
                    return contains(item, tree.Left);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        public Boolean Contains(string item)
        {
            return contains(item, root);
        }

        private void flatten(Node tree, ref List<string> result)
        {
            if (tree != null)
            {
                flatten(tree.Left, ref result);
                result.Add(tree.Data.ToString());
                flatten(tree.Right, ref result);
            }
        }

        private string longest(Node tree)
        {
            var flattened = new List<string>();
            flatten(tree, ref flattened);
            return flattened.OrderByDescending(s => s.Length).First();
        }

        public string Longest()
        {
            return longest(root);
        }

        private Node ancestor(Node root, Node first, Node second)
        {
            while (root != null)
            {
                Console.WriteLine(root.Data.Length + ">" + first.Data.Length + "&&" + root.Data.Length + ">" + second.Data.Length);
                Console.WriteLine(root.Data.Length + "<" + first.Data.Length + "&&" + root.Data.Length + "<" + second.Data.Length);
                
                if ((root.Data.Length > first.Data.Length) && (root.Data.Length > second.Data.Length))
                {
                    Console.WriteLine("Traversing left");
                    root = root.Left;
                }
                else if ((root.Data.Length < first.Data.Length) && (root.Data.Length < second.Data.Length))
                {
                    Console.WriteLine("Traversing right");
                    root = root.Right;
                }
                else
                {
                    Console.WriteLine("Returning ancestor");
                    //i.e; either iterative node is equal to e1 or e2 or in between e1 and e2
                    return root;
                }
            }
            //control will never come here
            return null;


            //// iterate through the tree starting from root  
            //Node current = root;
            //while (current == null)
            //{
            //    Console.WriteLine(current.Data);
            //    // either iterate on left(p) or right(q) from the current node  
            //    if (p.Data.Length < current.Data.Length)
            //        current = current.Left;
            //    else
            //        current = current.Right;
            //    Console.WriteLine(current.Data);
            //    // lowest common ancestor found  
            //    if ((p.Data.Length <= current.Data.Length && q.Data.Length >= current.Data.Length) ||
            //       (q.Data.Length <= current.Data.Length && p.Data.Length >= current.Data.Length))
            //        return current;
            //}

            //// lowest common ancestor not found  
            //return null;
        }

        private Node getNode(string item, Node tree)
        {
            if (tree != null)
            {
                if (tree.Data.Length < item.Length)
                {
                    return getNode(item, tree.Right);
                }
                else if (tree.Data.Length > item.Length)
                {
                    return getNode(item, tree.Left);
                }
                else
                {
                    return tree;
                }
            }
            else
            {
                return null;
            }
        }

        public string Ancestor(string first, string second)
        {
            if (Contains(first) && Contains(second))
            {
                Node firstNode = getNode(first, root);
                Node secondNode = getNode(second, root);

                Node resultNode = ancestor(root, firstNode, secondNode);
                Console.WriteLine(resultNode);
                return "Success";
            }
            else
            {
                return "Names do not exist in the tree!";
            }
        }
    }
}

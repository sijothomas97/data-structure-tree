namespace TaskA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int searchItem;
            BinTree tree = new BinTree();
            tree.InsertItem("Sijo", ref tree);
            tree.InsertItem("Sona", ref tree);
            tree.InsertItem("Paapu", ref tree);
            tree.InsertItem("Minu", ref tree);
            tree.InsertItem("Thomas", ref tree);
            tree.InsertItem("Bro", ref tree);

            Console.WriteLine("In-Order");
            tree.InOrder();
            Console.WriteLine("Pre-Order");
            tree.PreOrder();
            Console.WriteLine("Post-Order");
            tree.PostOrder();
            Console.WriteLine("------------------");
            Console.WriteLine(tree.Longest());
            Console.WriteLine("------------------");
            Console.WriteLine(tree.Ancestor("Bro", "Thomas"));
            //Console.WriteLine("Enter an element to search: ");
            //searchItem = Convert.ToInt32(Console.ReadLine());
            //if (tree.Contains(searchItem))
            //{
            //    Console.WriteLine("Item found from the tree");
            //}
            //else
            //{
            //    Console.WriteLine("Item could not found from the tree");
            //}

            //Console.WriteLine("Enter an element to search: ");
            //searchItem = Convert.ToInt32(Console.ReadLine());
            //if (tree.Contains(searchItem))
            //{
            //    Console.WriteLine("Item found from the tree");
            //}
            //else
            //{
            //    Console.WriteLine("Item could not found from the tree");
            //}
        }
    }
}
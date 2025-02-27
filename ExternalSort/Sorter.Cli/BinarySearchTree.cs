namespace Sorter.Cli
{
    class Node
    {
        public string[] Data { get; set; }
        public Node? Left, Right;

        public Node(string[] data)
        {
            Data = data;
            Left = Right = null;
        }
    }

    class BinarySearchTree
    {
        public int Count { get; private set; }
        public Node? Root;

        public void Insert(string data)
        {
            Root = InsertRec(Root, data.Split(". "));
        }

        private Node InsertRec(Node? root, string[] data)
        {
            if (root == null)
            {
                Count++;
                return new Node(data);
            }

            if (Compare(root, data) < 0)
                root.Left = InsertRec(root.Left, data);
            else
                root.Right = InsertRec(root.Right, data);

            return root;
        }

        private static int Compare(Node root, string[] data)
        {
            var a = data[1];
            var b = root.Data[1];

            var result = a.CompareTo(b);
            return result;
            //return string.Compare(data, root.Data);
        }

        public void InOrderTraversal(Node? root, StreamWriter writer)
        {
            if (root != null)
            {
                InOrderTraversal(root.Left, writer);
                writer.WriteLine(root.Data[0] + ". " + root.Data[1]);
                InOrderTraversal(root.Right, writer);
            }
        }
    }
}

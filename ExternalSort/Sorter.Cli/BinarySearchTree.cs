namespace Sorter.Cli
{
    internal class Node
    {
        public string Data { get; init; }
        public string[] Values { get; init; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(string data, string[] values)
        {
            Data = data;
            Values = values;
        }
    }

    internal class BinarySearchTree
    {
        public int Count { get; private set; }
        public Node? Root;

        public void Insert(string data)
        {
            Root = InsertRec(Root, data, data.Split(". "));
        }

        private Node InsertRec(Node? node, string data, string[] values)
        {
            if (node == null)
            {
                Count++;
                return new Node(data, values);
            }

            if (Compare(node, values) < 0)
                node.Left = InsertRec(node.Left, data, values);
            else
                node.Right = InsertRec(node.Right, data, values);

            return node;
        }

        private static int Compare(Node root, string[] data)
        {
            var a = data[1];
            var b = root.Values[1];

            var result = a.CompareTo(b);

            if (result == 0)
            {
                var numerica = long.Parse(data[0]);
                var numericb = long.Parse(root.Values[0]);

                return numerica.CompareTo(numericb);
            }

            return result;
        }

        public IEnumerable<Node> Ordered { get => Traverse(Root); }

        private static IEnumerable<Node> Traverse(Node? node)
        {
            if (node is not null)
            {
                foreach (var left in Traverse(node.Left))
                {
                    yield return left;
                }

                yield return node;

                foreach (var right in Traverse(node.Right))
                {
                    yield return right;
                }
            }
        }
    }
}

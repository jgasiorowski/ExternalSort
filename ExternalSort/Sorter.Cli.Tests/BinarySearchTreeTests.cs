namespace Sorter.Cli.Tests
{
    [TestClass]
    public class BinarySearchTreeTests
    {
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(0)]
        public void InsertNItems_CountShouldBeN(int items)
        {
            var tree = new BinarySearchTree();

            for (int i = 0; i < items; i++)
            {
                tree.Insert($"123{i}. abc");
            }

            Assert.AreEqual( items, tree.Count );
        }

        [TestMethod]
        public void Iterator_TraverseInOrder()
        {
            var tree = new BinarySearchTree();

            tree.Insert($"4. abe");
            tree.Insert($"1. abc");
            tree.Insert($"3. abe");
            tree.Insert($"8. abc");
            tree.Insert($"5. abc");
            tree.Insert($"2. abd");
            tree.Insert($"6. abc");
            tree.Insert($"7. abe");

            foreach (var item in tree.Ordered)
            {
                Console.WriteLine($"{item.Values[1]} {item.Values[0]}");
            }
        }
    }
}
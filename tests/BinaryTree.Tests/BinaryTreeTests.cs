using BinaryTree.Core;
using Xunit;

namespace BinaryTree.Tests;

public class BinaryTreeTests
{
    /// <summary>
    /// Builds the reference tree used by most tests:
    ///
    ///         8
    ///        / \
    ///       3   10
    ///      / \    \
    ///     1   6    14
    ///        / \   /
    ///       4   7 13
    /// </summary>
    private static BinaryTree<int> BuildSampleTree()
    {
        var tree = new BinaryTree<int>();
        foreach (var v in new[] { 8, 3, 10, 1, 6, 14, 4, 7, 13 })
        {
            tree.Insert(v);
        }
        return tree;
    }

    [Fact]
    public void Insert_ReturnsTrueForNewValue_FalseForDuplicate()
    {
        var tree = new BinaryTree<int>();
        Assert.True(tree.Insert(5));
        Assert.True(tree.Insert(3));
        Assert.False(tree.Insert(5));
        Assert.Equal(2, tree.Count);
    }

    [Fact]
    public void InOrder_ReturnsSortedSequence()
    {
        var tree = BuildSampleTree();
        Assert.Equal(new[] { 1, 3, 4, 6, 7, 8, 10, 13, 14 }, tree.InOrder());
    }

    [Fact]
    public void PreOrder_ReturnsRootLeftRight()
    {
        var tree = BuildSampleTree();
        Assert.Equal(new[] { 8, 3, 1, 6, 4, 7, 10, 14, 13 }, tree.PreOrder());
    }

    [Fact]
    public void PostOrder_ReturnsLeftRightRoot()
    {
        var tree = BuildSampleTree();
        Assert.Equal(new[] { 1, 4, 7, 6, 3, 13, 14, 10, 8 }, tree.PostOrder());
    }

    [Fact]
    public void Traversals_OnEmptyTree_AreEmpty()
    {
        var tree = new BinaryTree<int>();
        Assert.Empty(tree.InOrder());
        Assert.Empty(tree.PreOrder());
        Assert.Empty(tree.PostOrder());
    }

    [Theory]
    [InlineData(8)]
    [InlineData(1)]
    [InlineData(13)]
    public void Contains_ReturnsTrueForStoredValues(int value)
    {
        Assert.True(BuildSampleTree().Contains(value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(99)]
    public void Contains_ReturnsFalseForMissingValues(int value)
    {
        Assert.False(BuildSampleTree().Contains(value));
    }

    [Fact]
    public void MinAndMax_ReturnExtremes()
    {
        var tree = BuildSampleTree();
        Assert.Equal(1, tree.Min());
        Assert.Equal(14, tree.Max());
    }

    [Fact]
    public void MinAndMax_OnEmptyTree_Throw()
    {
        var tree = new BinaryTree<int>();
        Assert.Throws<InvalidOperationException>(() => tree.Min());
        Assert.Throws<InvalidOperationException>(() => tree.Max());
    }

    [Fact]
    public void Height_CountsNodesOnLongestPath()
    {
        Assert.Equal(0, new BinaryTree<int>().Height());

        var single = new BinaryTree<int>();
        single.Insert(1);
        Assert.Equal(1, single.Height());

        Assert.Equal(4, BuildSampleTree().Height()); // 8 -> 10 -> 14 -> 13
    }

    [Theory]
    [InlineData(1, 7, 3)]    // both under 3, opposite subtrees
    [InlineData(4, 7, 6)]    // siblings under 6
    [InlineData(1, 14, 8)]   // opposite sides of the root
    [InlineData(3, 7, 3)]    // ancestor of the other is its own LCA
    [InlineData(13, 13, 13)] // value with itself
    public void LowestCommonAncestor_ReturnsExpectedValue(int first, int second, int expected)
    {
        var tree = BuildSampleTree();
        Assert.Equal(expected, tree.LowestCommonAncestor(first, second));
        // Symmetric
        Assert.Equal(expected, tree.LowestCommonAncestor(second, first));
    }

    [Fact]
    public void FindLowestCommonAncestor_ReturnsActualNode()
    {
        var tree = BuildSampleTree();
        var node = tree.FindLowestCommonAncestor(4, 7);
        Assert.NotNull(node);
        Assert.Equal(6, node!.Value);
        Assert.Equal(4, node.Left!.Value);
        Assert.Equal(7, node.Right!.Value);
    }

    [Fact]
    public void LowestCommonAncestor_MissingValue_Throws()
    {
        var tree = BuildSampleTree();
        Assert.Throws<ArgumentException>(() => tree.LowestCommonAncestor(1, 99));
        Assert.Null(tree.FindLowestCommonAncestor(99, 1));
        Assert.Null(new BinaryTree<int>().FindLowestCommonAncestor(1, 2));
    }

    [Fact]
    public void CustomComparer_OrdersByStringLength_LikeLegacyApp()
    {
        // The legacy TaskA app keyed the BST on string length.
        var byLength = Comparer<string>.Create((a, b) => a.Length.CompareTo(b.Length));
        var tree = new BinaryTree<string>(byLength);

        foreach (var name in new[] { "Sijo", "Sona", "Paapu", "Minu", "Thomas", "Bro" })
        {
            tree.Insert(name);
        }

        // "Sona" and "Minu" compare equal to "Sijo" (length 4) — deduplicated.
        Assert.Equal(4, tree.Count);
        Assert.Equal(new[] { "Bro", "Sijo", "Paapu", "Thomas" }, tree.InOrder());
        Assert.True(tree.Contains("Sona")); // any 4-letter string matches
        Assert.Equal("Bro", tree.Min());
        Assert.Equal("Thomas", tree.Max());

        // Legacy bug fixed: Ancestor now returns the node/value, not "Success".
        Assert.Equal("Paapu", tree.LowestCommonAncestor("Paapu", "Thomas"));
        Assert.Equal("Sijo", tree.LowestCommonAncestor("Bro", "Thomas"));
    }

    [Fact]
    public void ReverseComparer_InvertsOrdering()
    {
        var desc = Comparer<int>.Create((a, b) => b.CompareTo(a));
        var tree = new BinaryTree<int>(desc);
        foreach (var v in new[] { 5, 1, 9, 3 })
        {
            tree.Insert(v);
        }

        Assert.Equal(new[] { 9, 5, 3, 1 }, tree.InOrder());
        Assert.Equal(9, tree.Min()); // "smallest" under the comparer
        Assert.Equal(1, tree.Max());
    }

    [Fact]
    public void Insert_SkewedInput_ProducesLinearHeight()
    {
        var tree = new BinaryTree<int>();
        for (int i = 1; i <= 5; i++)
        {
            tree.Insert(i);
        }

        Assert.Equal(5, tree.Height());
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, tree.InOrder());
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, tree.PreOrder());
        Assert.Equal(new[] { 5, 4, 3, 2, 1 }, tree.PostOrder());
    }
}

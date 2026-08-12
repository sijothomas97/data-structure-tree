namespace BinaryTree.Core;

/// <summary>
/// A generic binary search tree ordered by an <see cref="IComparer{T}"/>.
/// Duplicate values (values that compare equal) are not stored twice.
/// All traversals return lazy sequences; the tree performs no I/O.
/// </summary>
public sealed class BinaryTree<T>
{
    /// <summary>A node of the tree. Exposed read-only so callers can inspect structure.</summary>
    public sealed class Node
    {
        internal Node(T value) => Value = value;

        public T Value { get; }
        public Node? Left { get; internal set; }
        public Node? Right { get; internal set; }
    }

    private readonly IComparer<T> _comparer;

    /// <summary>Creates a tree ordered by <paramref name="comparer"/>, or by
    /// <see cref="Comparer{T}.Default"/> when omitted.</summary>
    public BinaryTree(IComparer<T>? comparer = null)
        => _comparer = comparer ?? Comparer<T>.Default;

    /// <summary>The root node, or null when the tree is empty.</summary>
    public Node? Root { get; private set; }

    /// <summary>Number of stored values.</summary>
    public int Count { get; private set; }

    /// <summary>
    /// Inserts <paramref name="value"/>. Returns true if it was added,
    /// false if an equal value was already present.
    /// </summary>
    public bool Insert(T value)
    {
        if (Root is null)
        {
            Root = new Node(value);
            Count = 1;
            return true;
        }

        var current = Root;
        while (true)
        {
            int cmp = _comparer.Compare(value, current.Value);
            if (cmp == 0)
            {
                return false; // duplicate — not stored
            }

            if (cmp < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new Node(value);
                    Count++;
                    return true;
                }
                current = current.Left;
            }
            else
            {
                if (current.Right is null)
                {
                    current.Right = new Node(value);
                    Count++;
                    return true;
                }
                current = current.Right;
            }
        }
    }

    /// <summary>Returns true if a value equal to <paramref name="value"/> is stored.</summary>
    public bool Contains(T value) => Find(value) is not null;

    /// <summary>In-order (sorted) traversal.</summary>
    public IEnumerable<T> InOrder() => InOrder(Root);

    /// <summary>Pre-order (node, left, right) traversal.</summary>
    public IEnumerable<T> PreOrder() => PreOrder(Root);

    /// <summary>Post-order (left, right, node) traversal.</summary>
    public IEnumerable<T> PostOrder() => PostOrder(Root);

    /// <summary>Smallest stored value. Throws <see cref="InvalidOperationException"/> when empty.</summary>
    public T Min()
    {
        var node = Root ?? throw new InvalidOperationException("The tree is empty.");
        while (node.Left is not null)
        {
            node = node.Left;
        }
        return node.Value;
    }

    /// <summary>Largest stored value. Throws <see cref="InvalidOperationException"/> when empty.</summary>
    public T Max()
    {
        var node = Root ?? throw new InvalidOperationException("The tree is empty.");
        while (node.Right is not null)
        {
            node = node.Right;
        }
        return node.Value;
    }

    /// <summary>
    /// Height of the tree measured in nodes on the longest root-to-leaf path:
    /// 0 for an empty tree, 1 for a single node.
    /// </summary>
    public int Height() => Height(Root);

    /// <summary>
    /// Returns the node holding the lowest common ancestor of two stored values,
    /// or null when either value is absent (or the tree is empty).
    /// A value that is an ancestor of the other is its own LCA.
    /// </summary>
    public Node? FindLowestCommonAncestor(T first, T second)
    {
        if (Find(first) is null || Find(second) is null)
        {
            return null;
        }

        var current = Root;
        while (current is not null)
        {
            int cmpFirst = _comparer.Compare(first, current.Value);
            int cmpSecond = _comparer.Compare(second, current.Value);

            if (cmpFirst < 0 && cmpSecond < 0)
            {
                current = current.Left;
            }
            else if (cmpFirst > 0 && cmpSecond > 0)
            {
                current = current.Right;
            }
            else
            {
                return current; // split point (or equal to one of the values)
            }
        }

        return null; // unreachable: both values verified present
    }

    /// <summary>
    /// Returns the value of the lowest common ancestor of two stored values.
    /// Throws <see cref="ArgumentException"/> when either value is not in the tree.
    /// </summary>
    public T LowestCommonAncestor(T first, T second)
    {
        var node = FindLowestCommonAncestor(first, second)
            ?? throw new ArgumentException("Both values must exist in the tree.");
        return node.Value;
    }

    private Node? Find(T value)
    {
        var current = Root;
        while (current is not null)
        {
            int cmp = _comparer.Compare(value, current.Value);
            if (cmp == 0)
            {
                return current;
            }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    private static IEnumerable<T> InOrder(Node? node)
    {
        if (node is null)
        {
            yield break;
        }
        foreach (var v in InOrder(node.Left)) yield return v;
        yield return node.Value;
        foreach (var v in InOrder(node.Right)) yield return v;
    }

    private static IEnumerable<T> PreOrder(Node? node)
    {
        if (node is null)
        {
            yield break;
        }
        yield return node.Value;
        foreach (var v in PreOrder(node.Left)) yield return v;
        foreach (var v in PreOrder(node.Right)) yield return v;
    }

    private static IEnumerable<T> PostOrder(Node? node)
    {
        if (node is null)
        {
            yield break;
        }
        foreach (var v in PostOrder(node.Left)) yield return v;
        foreach (var v in PostOrder(node.Right)) yield return v;
        yield return node.Value;
    }

    private static int Height(Node? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));
}

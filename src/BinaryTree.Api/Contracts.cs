namespace BinaryTree.Api;

/// <summary>Request body for POST /insert.</summary>
public sealed record InsertRequest(int Value);

/// <summary>Result of an insert: whether the value was added and the new count.</summary>
public sealed record InsertResponse(int Value, bool Added, int Count);

/// <summary>A traversal's visit sequence.</summary>
public sealed record TraversalResponse(string Order, IReadOnlyList<int> Sequence);

/// <summary>Lowest common ancestor of two stored values.</summary>
public sealed record LcaResponse(int First, int Second, int Ancestor);

/// <summary>A node with layout hints: XIndex is the in-order rank, Depth the level (root = 0).</summary>
public sealed record NodeDto(int Id, int Value, int XIndex, int Depth);

/// <summary>A parent-to-child edge (ids are node values, which are unique in a BST).</summary>
public sealed record EdgeDto(int From, int To);

/// <summary>Whole-tree snapshot as a node/edge graph plus summary stats.</summary>
public sealed record TreeGraphResponse(
    IReadOnlyList<NodeDto> Nodes,
    IReadOnlyList<EdgeDto> Edges,
    int Count,
    int Height);

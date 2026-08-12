using BinaryTree.Core;

namespace BinaryTree.Api;

/// <summary>
/// Singleton, thread-safe holder for the application's single shared tree.
/// All access goes through <see cref="With{TResult}"/> under a lock because
/// <see cref="BinaryTree{T}"/> itself is not thread-safe.
/// </summary>
public sealed class TreeStore
{
    private readonly object _gate = new();
    private BinaryTree<int> _tree = new();

    /// <summary>Runs <paramref name="action"/> against the tree under the store's lock.</summary>
    public TResult With<TResult>(Func<BinaryTree<int>, TResult> action)
    {
        lock (_gate)
        {
            return action(_tree);
        }
    }

    /// <summary>Replaces the tree with a fresh empty one.</summary>
    public void Reset()
    {
        lock (_gate)
        {
            _tree = new BinaryTree<int>();
        }
    }
}

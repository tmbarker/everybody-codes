using System.Diagnostics;
using System.Text;
using Utilities.Graph;

namespace Solutions.S01.Q02;

using Node = BinaryTreeNode<Forest.NodeInfo>;
using Pair = (BinaryTreeNode<Forest.NodeInfo> Left, BinaryTreeNode<Forest.NodeInfo> Right);

public class Forest
{
    public readonly record struct NodeInfo(int Rank, string Symbol);
    public enum SwapMode
    {
        Value,
        Node
    }
    
    private Node? _leftRoot;
    private Node? _rightRoot;
    private int _treeSize;
    private readonly Dictionary<int, Pair> _pairMap = new();

    public void AddPair(int id, int lr, int rr, string ls, string rs)
    {
        var ln = new Node(new NodeInfo(lr, ls));
        var rn = new Node(new NodeInfo(rr, rs));

        _treeSize++;
        _pairMap[id] = (ln, rn);

        if (_leftRoot == null) _leftRoot = ln;
        else InsertNode(root: _leftRoot, ln);
        
        if (_rightRoot == null) _rightRoot = rn;
        else InsertNode(root: _rightRoot, rn);
    }

    public void SwapPair(int id, SwapMode mode)
    {
        var (a, b) = _pairMap[id];
        switch (mode)
        {
            case SwapMode.Value:
                (a.Value, b.Value) = (b.Value, a.Value);
                return;
            case SwapMode.Node:
                var aParent = a.Parent;
                var bParent = b.Parent;
                var aIsLeftChild = aParent?.Left == a;
                var bIsLeftChild = bParent?.Left == b;
                var originalLRoot = _leftRoot;
                var originalRRoot = _rightRoot;

                //  Disconnect nodes from parents.
                //
                if (aParent != null)
                {
                    if (aIsLeftChild) aParent.Left = null;
                    else aParent.Right = null;
                }
                if (bParent != null)
                {
                    if (bIsLeftChild) bParent.Left = null;
                    else bParent.Right = null;
                }
                
                //  Set nodes as children of their new parents (swap).
                //
                if (aParent != null)
                {
                    if (aIsLeftChild) aParent.Left = b;
                    else aParent.Right = b;
                }
                else
                {
                    if (a == originalLRoot) _leftRoot = b;
                    else
                    {
                        Debug.Assert(a == originalRRoot);
                        _rightRoot = b;
                    }
                }
                
                if (bParent != null)
                {
                    if (bIsLeftChild) bParent.Left = a;
                    else bParent.Right = a;
                }
                else
                {
                    if (b == originalLRoot) _leftRoot = a;
                    else
                    {
                        Debug.Assert(b == originalRRoot);
                        _rightRoot = a;
                    }
                }
                return;
            default:
                throw new NoSolutionException($"Unhandled swap mode: {mode}");
        }
    }

    private static void InsertNode(Node root, Node node)
    {
        if (node.Value.Rank < root.Value.Rank)
        {
            if (root.Left == null) root.Left = node;
            else InsertNode(root.Left, node);
        }
        else
        {
            if (root.Right == null) root.Right = node;
            else InsertNode(root.Right, node);
        }
    }

    private static string ReadMessage(Node root, int size)
    {
        var message = new StringBuilder();
        var queue = new Queue<Node>([root]);
        var ranks = new PriorityQueue<string, int>();

        while (queue.Count != 0)
        {
            var nodesAtDepth = queue.Count;
            var remainingAtDepth = nodesAtDepth;
            
            while (remainingAtDepth-- > 0)
            {
                var node = queue.Dequeue();
                message.Append(node.Value.Symbol);
                
                if (node.Left != null)  queue.Enqueue(node.Left);
                if (node.Right != null) queue.Enqueue(node.Right);
            }

            ranks.Enqueue(message.ToString(), priority: size - nodesAtDepth);
            message.Clear();
        }

        return ranks.Dequeue();
    }
    
    public string Read()
    {
        return $"{ReadMessage(_leftRoot!, _treeSize)}{ReadMessage(_rightRoot!, _treeSize)}";
    }
}
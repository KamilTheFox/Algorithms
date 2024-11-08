using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BinaryAVLTree<T> : IMyData<T> where T : IComparable<T>
{
    //
    // Большая часть реализуется через циклы, так как в больших деревьях это вызывает переполнение стека вызово.
    //
    public int Count { get; protected set; }

    protected int addCounter = 0;

    private const int BALANCE_THRESHOLD = 5;

    protected bool isBalancingEnabled = true;

    protected NodeTree<T> root;

    protected bool isInverse;

    public void InOrderTraversal(Action<NodeTree<T>> action)
    {
        if (root == null)
            return;
        InOrderTraversal(root, action);
    }

    public void Inverse()
    {
        isInverse =! isInverse;
        if (root == null) return;

        Stack<NodeTree<T>> stack = new Stack<NodeTree<T>>();

        stack.Push(root);

        while (stack.Count > 0)
        {
            NodeTree<T> NodeTree = stack.Pop();
            NodeTree<T> temp = NodeTree.left;
            NodeTree.left = NodeTree.right;
            NodeTree.right = temp;
            if (NodeTree.right != null) stack.Push(NodeTree.right);
            if (NodeTree.left != null) stack.Push(NodeTree.left);
        }
    }

    protected bool IsMinStep(T value, NodeTree<T> cur, bool minLeft)
    {
        if (minLeft)
            return cur.Value.CompareTo(value) < 0;
        else
            return cur.Value.CompareTo(value) > 0;
    }

    public virtual void AddRange(T[] values)
    {
        isBalancingEnabled = false;
        foreach (T value in values)
            Add(value);
        BalanceTree();
        isBalancingEnabled = true;
        addCounter = 0;
    }

    public virtual void Add(T value)
    {
        NodeTree<T> newNodeTree = new NodeTree<T>(value);
        if (root == null)
        {
            root = newNodeTree;
            Count++;
            return;
        }
        NodeTree<T> cur = root;

        while (cur != null)
        {
            if (cur.Value.CompareTo(value) == 0)
                break;
            if(IsMinStep(value, cur, isInverse))
            {
                if (cur.left == null)
                {
                    cur.left = newNodeTree;
                    break;
                }
                cur = cur.left;
                continue;
            }
            if (cur.right == null)
            {
                cur.right = newNodeTree;
                break;
            }
            cur = cur.right;
        }
        Count++;
        if (isBalancingEnabled)
        {
            addCounter++;
            if (addCounter >= BALANCE_THRESHOLD)
            {
                BalanceTree();
                addCounter = 0;
            }
        }
    }

    public virtual void Remove(T value)
    {
        root = RemoveNode(root, value);
        Count--;
    }

    private NodeTree<T> RemoveNode(NodeTree<T> node, T value)
    {
        if (node == null)
            return null;

        int compareResult;

        if (isInverse)
            compareResult = value.CompareTo(node.Value) * -1;
        else
            compareResult = value.CompareTo(node.Value);

        if (compareResult < 0)
        {
            node.left = RemoveNode(node.left, value);
        }
        else if (compareResult > 0)
        {
            node.right = RemoveNode(node.right, value);
        }
        else // Нашли узел для удаления
        {
            if (node.left == null)
                return node.right;
            if (node.right == null)
                return node.left;

            if (isInverse)
            {
                NodeTree<T> maxLeft = FindMax(node.left);
                node.Value = maxLeft.Value;
                node.left = RemoveNode(node.left, maxLeft.Value);
            }
            else
            {
                NodeTree<T> minRight = FindMin(node.right);
                node.Value = minRight.Value;
                node.right = RemoveNode(node.right, minRight.Value);
            }
        }

        return BalanceNode(node);
    }

    private NodeTree<T> FindMax(NodeTree<T> node)
    {
        if (node == null)
            return null;

        while (node.right != null)
            node = node.right;

        return node;
    }

    private NodeTree<T> FindMin(NodeTree<T> node)
    {
        if (node == null)
            return null;

        if(isInverse)
        {
            while (node.right != null)
                node = node.right;
        }

        while (node.left != null)
            node = node.left;

        return node;
    }

    public virtual void BalanceTree()
    {
        root = BalanceNode(root);
    }

    private NodeTree<T> BalanceNode(NodeTree<T> node)
    {
        if (node == null) return null;

        rebalance:
        node.left = BalanceNode(node.left);
        node.right = BalanceNode(node.right);

        int heightLeft = MaxHeightNode(node.left);
        int heightRight = MaxHeightNode(node.right);

        if ((heightLeft - heightRight) > 1)
        {
            int hll = MaxHeightNode(node.left.left);
            int hlr = MaxHeightNode(node.left.right);
            if (hll < hlr)
            {
                node.left = RotateLeft(node.left);
            }
            node = RotateRight(node);
            goto rebalance;
        }
        else if ((heightRight - heightLeft) > 1)
        {
            int hrr = MaxHeightNode(node.right.right);
            int hrl = MaxHeightNode(node.right.left);
            if (hrr < hrl)
            {
                node.right = RotateRight(node.right);
            }
            node = RotateLeft(node);
            goto rebalance;
        }
        return node;
    }

    protected NodeTree<T> RotateLeft(NodeTree<T> target)
    {
        NodeTree<T> A = target.left;
        NodeTree<T> B = target.right.left;
        NodeTree<T> C = target.right.right;
        NodeTree<T> pivot = target.right;

        pivot.left = target;
        pivot.left.left = A;
        pivot.left.right = B;
        pivot.right = C;

        return pivot;
    }
    protected NodeTree<T> RotateRight(NodeTree<T> target)
    {
        NodeTree<T> A = target.right;
        NodeTree<T> B = target.left.right;
        NodeTree<T> C = target.left.left;
        NodeTree<T> pivot = target.left;

        pivot.right = target;
        pivot.right.right = A;
        pivot.right.left = B;
        pivot.left = C;

        return pivot;
    }
    
    private int MaxHeightNode(NodeTree<T> started) 
    {
        if (started == null) return 0;

        Stack<(NodeTree<T> node, int depth)> stack = new Stack<(NodeTree<T>, int)>();
        stack.Push((started, 1));
        int maxHeight = 0;

        while (stack.Count > 0)
        {
            var (currentNode, currentDepth) = stack.Pop();
            maxHeight = Math.Max(maxHeight, currentDepth);

            if (currentNode.left != null)
                stack.Push((currentNode.left, currentDepth + 1));
            if (currentNode.right != null)
                stack.Push((currentNode.right, currentDepth + 1));
        }
        return maxHeight;
    }
    //Хотел улучшить алгоритм минимумом высоты, но не получилось
    private int MinHeightNode(NodeTree<T> started)
    {
        if (started == null) return 0;
        if (started.left == null && started.right == null) return 1; 

        Stack<(NodeTree<T> node, int depth)> stack = new Stack<(NodeTree<T>, int)>();
        stack.Push((started, 1));
        int minLeafHeight = int.MaxValue;

        while (stack.Count > 0)
        {
            var (currentNode, currentDepth) = stack.Pop();

            if (currentNode.left == null && currentNode.right == null)
            {
                minLeafHeight = Math.Min(minLeafHeight, currentDepth);
            }

            if (currentNode.right != null)
                stack.Push((currentNode.right, currentDepth + 1));
            if (currentNode.left != null)
                stack.Push((currentNode.left, currentDepth + 1));
        }

        return minLeafHeight;
    }
    protected void InOrderTraversal(NodeTree<T> started, Action<NodeTree<T>> action)
    {
        Stack<NodeTree<T>> stack = new Stack<NodeTree<T>>();
        NodeTree<T> current = started;

        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.left;
            }
            current = stack.Pop();
            action(current);
            current = current.right;
        }
    }
    #region FindFunction
    public T Find(T value)
    {
        var node = FindNode(root, value);
        if(node == null)
            return default(T);
        return node.Value;
    }

    protected NodeTree<T> FindNode(NodeTree<T> node, T value)
    {
        if (node == null || node.Value.CompareTo(value) == 0)
            return node;

        int compareResult = isInverse ?
            value.CompareTo(node.Value) * -1 :
            value.CompareTo(node.Value);

        if (compareResult < 0)
            return FindNode(node.left, value);

        return FindNode(node.right, value);
    }

    public T FindByIndex(int index)
    {
        List<T> elements = new List<T>();
        InOrderTraversal(root, (node) =>
        {
            elements.Add(node.Value);
        });

        if (index >= 0 && index < elements.Count)
            return elements[index];

        throw new ArgumentOutOfRangeException("Index out of range");
    }
    public List<T> FindAll(Predicate<T> predicate)
    {
        List<T> result = new List<T>();
        FindAllRecursive(root, predicate, result);
        return result;
    }

    private void FindAllRecursive(NodeTree<T> node, Predicate<T> predicate, List<T> result)
    {
        if (node == null) return;

        FindAllRecursive(node.left, predicate, result);

        if (predicate(node.Value))
            result.Add(node.Value);

        FindAllRecursive(node.right, predicate, result);
    }
    public NodeTree<T> FindByHash(int hash)
    {
        return FindNodeByHash(root, hash);
    }

    private NodeTree<T> FindNodeByHash(NodeTree<T> node, int hash)
    {
        if (node == null) return null;

        if (node.Value.GetHashCode() == hash)
            return node;

        NodeTree<T> leftResult = FindNodeByHash(node.left, hash);
        if (leftResult != null) return leftResult;

        return FindNodeByHash(node.right, hash);
    }
    #endregion
    public override string ToString()
    {
        List<NodeTree<T>[]> resultTree = new List<NodeTree<T>[]>();

        Stack<NodeTree<T>[]> stack = new Stack<NodeTree<T>[]>();
        stack.Push(new NodeTree<T>[] { root } );
        while (stack.Count > 0)
        {
            NodeTree<T>[] nodes = stack.Pop();
            resultTree.Add(nodes);
            List<NodeTree<T>> currentLevel = new List<NodeTree<T>>();

            foreach (NodeTree<T> node in nodes)
            {
                if(node == null)
                {
                    currentLevel.Add(null);
                    currentLevel.Add(null);
                    continue;
                }
                currentLevel.Add(node.left);
                currentLevel.Add(node.right);
            }
            if (currentLevel.Any(n => n != null))
            {
                stack.Push(currentLevel.ToArray());
            }
        }
        StringBuilder builder = new StringBuilder();
        int maxVertical = resultTree.Count;

        for (int i = 0; i < resultTree.Count; i++)
        {
            var arr = resultTree[i];
            int spaces = (int)Math.Pow(2, maxVertical - i - 1) - 1;
            int betweenSpaces = (int)Math.Pow(2, maxVertical - i) - 1;

            builder.Append(new string(' ', spaces));
            foreach (NodeTree<T> node in arr)
            {
                if (node == null)
                {
                    builder.Append(new string(' ', betweenSpaces + 1)); 
                }
                else
                {
                    builder.Append(node.Value.ToString().PadRight(betweenSpaces + 1)); 
                }
            }
            builder.AppendLine();

            if (i < resultTree.Count - 1)
            {
                int leftPadding = spaces / 2;
                builder.Append(new string(' ', leftPadding));
                for (int k = 0; k < arr.Length; k++)
                {
                    NodeTree<T> node = arr[k];
                    int lineLength = Math.Max(0, (betweenSpaces - 1) / 4);
                    if (node != null && (node.left != null || node.right != null))
                    {
                        builder.Append('\u2554');
                        builder.Append(new string('\u2550', lineLength));
                        builder.Append('\u2569');
                        builder.Append(new string('\u2550', lineLength));
                        builder.Append('\u2557');
                        int len = betweenSpaces - lineLength * 2 - i;
                        if (len < 0) len = 0;
                        builder.Append(new string(' ', len ));
                    }
                    else
                    {
                        builder.Append(new string(' ', betweenSpaces + 1));
                    }
                }
                builder.AppendLine();
            }
        }
        return builder.ToString();
    }
    public object Clone()
    {
        List<T> ts = new List<T>();
        InOrderTraversal(root, (node) => ts.Add(node.Value));
        var newTree = new BinaryAVLTree<T>();
        newTree.AddRange(ts.ToArray());
        return newTree;
    }

    public IEnumerator GetEnumerator()
    {
        List<T> ts = new List<T>();
        InOrderTraversal(root, (node) => ts.Add(node.Value));
        return ts.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        List<T> ts = new List<T>();
        InOrderTraversal(root, (node) => ts.Add(node.Value));
        return ts.GetEnumerator();
    }

    public void Dispose()
    {
        InOrderTraversal(root, (node) =>
        {
            node.left = null;
            node.right = null;
        });
        root = null;
    }
}

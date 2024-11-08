using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Реализация красночерного дерева по тупому. Эффективность примерно O(N^2)
public class RedBlackTree1<T> : BinaryAVLTree<T> where T : IComparable<T>
{
    private const bool RED = true;
    private const bool BLACK = false;

    private NodeTreeRedBlack<T> Root
    {
        get { return (NodeTreeRedBlack<T>)root; }
        set { root = value; }
    }

    public override void Add(T value)
    {
        NodeTreeRedBlack<T> newNodeTree = new NodeTreeRedBlack<T>(value);
        if (root == null)
        {
            root = newNodeTree;
            Count++;
        }
        NodeTreeRedBlack<T> cur = Root;

        while (cur != null)
        {
            if (cur.Value.CompareTo(value) == 0)
                break;
            if (IsMinStep(value, cur, isInverse))
            {
                if (cur.left == null)
                {
                    cur.left = newNodeTree;
                    break;
                }
                cur = cur.Left;
                continue;
            }
            if (cur.right == null)
            {
                cur.right = newNodeTree;
                break;
            }
            cur = cur.Right;
        }
        Count++;
        BalanceTree();
    }
    //Основная проблема тут. Дерево полностью перестраивается и перекрашивается.
    public override void BalanceTree()
    {
        root = BalanceNode(Root);
        if (Root.Color != BLACK)
        {
            Root.Color = BLACK;
            RecolorNode(Root);
        }
    }
    //Баланс на основе количества красных нод
    private NodeTreeRedBlack<T> BalanceNode(NodeTreeRedBlack<T> node)
    {
        if (node == null) return null;

        rebalance:
        node.left = BalanceNode(node.Left);
        node.right = BalanceNode(node.Right);

        int heightLeft = MaxHeightNode(node.Left).red;
        int heightRight = MaxHeightNode(node.Right).red;

        if ((heightLeft - heightRight) > 1)
        {
            int hll = MaxHeightNode(node.Left.Left).red;
            int hlr = MaxHeightNode(node.Left.Right).red;
            if (hll < hlr)
            {
                node.left = RotateLeft(node.left);
            }
            node = (NodeTreeRedBlack<T>)RotateRight(node);
            goto rebalance;
        }
        else if ((heightRight - heightLeft) > 1)
        {
            int hrr = MaxHeightNode(node.Right.Right).red;
            int hrl = MaxHeightNode(node.Right.Left).red;
            if (hrr < hrl)
            {
                node.right = RotateRight(node.right);
            }
            node = (NodeTreeRedBlack<T>)RotateLeft(node);
            goto rebalance;
        }
        RecolorNode(node);
        return node;
    }
    //Обычный рекурсивный реколор
    private void RecolorNode(NodeTreeRedBlack<T> node)
    {
        if (node.Left != null)
        {
            node.Left.Color = !node.Color;
            RecolorNode(node.Left);
        }
        if (node.Right != null)
        {
            node.Right.Color = !node.Color;
            RecolorNode(node.Right);
        }
    }
    //Максимум высоты в ноде по красным и черным нодам
    private (int black, int red) MaxHeightNode(NodeTreeRedBlack<T> started)
    {
        if (started == null) return (0, 0);

        Stack<(NodeTreeRedBlack<T> node, int depthRed, int depthBack)> stack = new Stack<(NodeTreeRedBlack<T>, int, int)>();
        bool isRed = started.Color == RED;
        stack.Push((started, isRed ? 1 : 0, isRed ? 0 : 1));

        int maxHeightBlack = 0;
        int maxHeightRed = 0;

        while (stack.Count > 0)
        {
            var (currentNode, currentDepthRed, currentDepthBlack) = stack.Pop();

            maxHeightBlack = Math.Max(maxHeightBlack, currentDepthBlack);

            maxHeightRed = Math.Max(maxHeightRed, currentDepthRed);

            if (currentNode.left != null)
            {
                if (currentNode.Left.Color == BLACK)
                    currentDepthBlack++;
                else
                    currentDepthRed++;
                stack.Push((currentNode.Left, currentDepthRed, currentDepthBlack));
            }
            if (currentNode.right != null)
            {
                if (currentNode.Right.Color == BLACK)
                    currentDepthBlack++;
                else
                    currentDepthRed++;
                stack.Push((currentNode.Right, currentDepthRed, currentDepthBlack));
            }
        }
        return (maxHeightRed, maxHeightBlack);
    }

    public void AddRange(T[] values)
    {
        foreach (T value in values)
            Add(value);
    }

    public override void Remove(T value)
    {
        base.Remove(value);
        BalanceTree();
    }

    public override string ToString()
    {
        // Константы для цветов ANSI
        const string RED = "\u001b[31m";
        const string RESET = "\u001b[0m";

        List<NodeTreeRedBlack<T>[]> resultTree = new List<NodeTreeRedBlack<T>[]>();

        Stack<NodeTreeRedBlack<T>[]> stack = new Stack<NodeTreeRedBlack<T>[]>();
        stack.Push(new NodeTreeRedBlack<T>[] { Root });
        while (stack.Count > 0)
        {
            NodeTreeRedBlack<T>[] nodes = stack.Pop();
            resultTree.Add(nodes);
            List<NodeTreeRedBlack<T>> currentLevel = new List<NodeTreeRedBlack<T>>();

            foreach (NodeTreeRedBlack<T> node in nodes)
            {
                if (node == null)
                {
                    currentLevel.Add(null);
                    currentLevel.Add(null);
                    continue;
                }
                currentLevel.Add((NodeTreeRedBlack<T>)node.left);
                currentLevel.Add((NodeTreeRedBlack<T>)node.right);
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
            foreach (NodeTreeRedBlack<T> node in arr)
            {
                if (node == null)
                {
                    builder.Append(new string(' ', betweenSpaces + 1));
                }
                else
                {
                    string nodeValue = node.Value.ToString().PadRight(betweenSpaces + 1);
                    // Добавляем цвет для красных узлов
                    if (node.Color == RedBlackTree1<T>.RED)
                    {
                        builder.Append(RED + nodeValue + RESET);
                    }
                    else
                    {
                        builder.Append(nodeValue);
                    }
                }
            }
            builder.AppendLine();

            if (i < resultTree.Count - 1)
            {
                int leftPadding = spaces / 2;
                builder.Append(new string(' ', leftPadding));
                for (int k = 0; k < arr.Length; k++)
                {
                    NodeTreeRedBlack<T> node = arr[k];
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
                        builder.Append(new string(' ', len));
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
}

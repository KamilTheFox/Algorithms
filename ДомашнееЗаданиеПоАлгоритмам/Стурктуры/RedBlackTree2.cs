using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RedBlackTree2<T> : BinaryAVLTree<T> where T : IComparable<T>
{
    protected new NodeTreeRedBlack<T> root;
    private const bool RED = true;
    private const bool BLACK = false;

    public override void Add(T value)
    {
        var newNode = new NodeTreeRedBlack<T>(value);

        if (root == null)
        {
            root = newNode;
            root.Color = BLACK;
            Count++;
            return;
        }

        InsertNode(newNode);
        Count++;
    }

    private void InsertNode(NodeTreeRedBlack<T> node)
    {
        // Стандартная вставка BST
        NodeTreeRedBlack<T> current = root;
        NodeTreeRedBlack<T> parent = null;

        while (current != null)
        {
            parent = current;
            if (node.Value.CompareTo(current.Value) < 0)
                current = current.Left;
            else
                current = current.Right;
        }

        node.Parent = parent;

        if (node.Value.CompareTo(parent.Value) < 0)
            parent.left = node;
        else
            parent.right = node;

        node.Color = RED;
        InsertFixup(node);
    }

    private void InsertFixup(NodeTreeRedBlack<T> node)
    {
        // Пока не дошли до корня и родитель красный
        while (node != root && node.Parent.Color == RED)
        {
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = node.Parent.Parent.Right;

                // Случай 1: Дядя красный
                if (uncle != null && uncle.Color == RED)
                {
                    node.Parent.Color = BLACK;
                    uncle.Color = BLACK;
                    node.Parent.Parent.Color = RED;
                    node = node.Parent.Parent;
                }
                else
                {
                    // Случай 2: Дядя черный, node - правый ребенок
                    if (node == node.Parent.Right)
                    {
                        node = node.Parent;
                        LeftRotate(node);
                    }

                    // Случай 3: Дядя черный, node - левый ребенок
                    node.Parent.Color = BLACK;
                    node.Parent.Parent.Color = RED;
                    RightRotate(node.Parent.Parent);
                }
            }
            else // Симметричный случай
            {
                var uncle = node.Parent.Parent.Left;

                if (uncle != null && uncle.Color == RED)
                {
                    node.Parent.Color = BLACK;
                    uncle.Color = BLACK;
                    node.Parent.Parent.Color = RED;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        node = node.Parent;
                        RightRotate(node);
                    }

                    node.Parent.Color = BLACK;
                    node.Parent.Parent.Color = RED;
                    LeftRotate(node.Parent.Parent);
                }
            }
        }

        root.Color = BLACK;
    }

    private void LeftRotate(NodeTreeRedBlack<T> x)
    {
        var y = x.Right;

        // Установка левого поддерева y
        x.right = y.left;
        if (y.Left != null)
            y.Left.Parent = x;

        // Связывание родителя x с y
        y.Parent = x.Parent;
        if (x.Parent == null)
            root = y;
        else if (x == x.Parent.Left)
            x.Parent.left = y;
        else
            x.Parent.right = y;

        // Помещаем x слева от y
        y.left = x;
        x.Parent = y;
    }

    private void RightRotate(NodeTreeRedBlack<T> y)
    {
        var x = y.Left;

        // Установка правого поддерева x
        y.left = x.right;
        if (x.Right != null)
            x.Right.Parent = y;

        // Связывание родителя y с x
        x.Parent = y.Parent;
        if (y.Parent == null)
            root = x;
        else if (y == y.Parent.Right)
            y.Parent.right = x;
        else
            y.Parent.left = x;

        // Помещаем y справа от x
        x.right = y;
        y.Parent = x;
    }

    private bool IsBlackHeightValid(NodeTreeRedBlack<T> node, ref int blackHeight)
    {
        if (node == null)
        {
            blackHeight = 1; // Null узлы считаются черными
            return true;
        }

        int leftBlackHeight = 0, rightBlackHeight = 0;

        if (!IsBlackHeightValid(node.Left, ref leftBlackHeight) ||
            !IsBlackHeightValid(node.Right, ref rightBlackHeight))
            return false;

        if (leftBlackHeight != rightBlackHeight)
            return false;

        blackHeight = leftBlackHeight;
        if (node.Color == BLACK)
            blackHeight++;

        return true;
    }

    private bool AreRedNodesValid(NodeTreeRedBlack<T> node)
    {
        if (node == null)
            return true;

        if (node.Color == RED)
        {
            if ((node.Left != null && node.Left.Color == RED) ||
                (node.Right != null && node.Right.Color == RED))
                return false;
        }

        return AreRedNodesValid(node.Left) && AreRedNodesValid(node.Right);
    }

    public bool ValidateTree()
    {
        if (root == null)
            return true;

        // Проверка 1: Корень должен быть черным
        if (root.Color != BLACK)
            return false;

        // Проверка 2: Красные узлы должны иметь черных детей
        if (!AreRedNodesValid(root))
            return false;

        // Проверка 3: Черная высота должна быть одинаковой
        int blackHeight = 0;
        return IsBlackHeightValid(root, ref blackHeight);
    }
    private int GetBlackHeight(NodeTreeRedBlack<T> node)
    {
        if (node == null)
            return 0;

        int leftHeight = GetBlackHeight(node.Left);
        int rightHeight = GetBlackHeight(node.Right);

        if (leftHeight != rightHeight)
            return -1;

        return leftHeight + (node.Color == BLACK ? 1 : 0);
    }

    public void AddRange(T[] values)
    {
        foreach (T value in values)
            Add(value);
    }

    public override string ToString()
    {
        // Константы для цветов ANSI
        const string RED = "\u001b[31m";
        const string RESET = "\u001b[0m";

        List<NodeTreeRedBlack<T>[]> resultTree = new List<NodeTreeRedBlack<T>[]>();

        Stack<NodeTreeRedBlack<T>[]> stack = new Stack<NodeTreeRedBlack<T>[]>();
        stack.Push(new NodeTreeRedBlack<T>[] { root });
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
                    if (node.Color == RedBlackTree2<T>.RED)
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
        //builder.AppendLine($"ValidateRedNodesProperty: {ValidateRedNodesProperty(root)}");
        //builder.AppendLine($"ValidateRedBlackProperties: {ValidateRedBlackProperties()}");
        //builder.AppendLine($"ValidateBlackHeight: {ValidateRedNodesProperty(root)}");
        return builder.ToString();
    }
    private bool ValidateBlackProperties()
    {
        // Проверяем, что корень черный
        if (root.Color != BLACK)
            return false;

        // Проверяем черную высоту
        return GetBlackHeight(root) != -1;
    }
    private bool ValidateRedBlackProperties()
    {
        if (root.Color != BLACK)
            return false;

        // Проверка свойства: красный узел имеет черных детей
        return ValidateRedNodesProperty(root) && ValidateBlackHeight(root) != -1;
    }

    private bool ValidateRedNodesProperty(NodeTreeRedBlack<T> node)
    {
        if (node == null)
            return true;

        if (node.Color == RED)
        {
            if (node.Left?.Color == RED || node.Right?.Color == RED)
                return false;
        }

        return ValidateRedNodesProperty(node.Left) && ValidateRedNodesProperty(node.Right);
    }

    private int ValidateBlackHeight(NodeTreeRedBlack<T> node)
    {
        if (node == null)
            return 0;

        int leftHeight = ValidateBlackHeight(node.Left);
        int rightHeight = ValidateBlackHeight(node.Right);

        if (leftHeight == -1 || rightHeight == -1 || leftHeight != rightHeight)
            return -1;

        return (node.Color == BLACK) ? leftHeight + 1 : leftHeight;
    }
}

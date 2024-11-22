using System.Text;

public class RedBlackTree<T> : BinaryAVLTree<T> where T : IComparable<T>
{
    protected NodeTreeRedBlack<T> Root
    {
        get
        {
            return (NodeTreeRedBlack<T>)root;
        }
        set
        {
            root = value;
        }
    }
    private const bool RED = true;
    private const bool BLACK = false;

    public override void Add(T value)
    {
        var newNode = new NodeTreeRedBlack<T>(value);

        if (Root == null)
        {
            Root = newNode;
            Root.Color = BLACK;
            Count++;
            return;
        }

        AddNode(newNode);
        Count++;
    }

    private void AddNode(NodeTreeRedBlack<T> node)
    {
        // Стандартная вставка BST
        NodeTreeRedBlack<T> current = Root;
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
        AddFixup(node);
    }

    private void AddFixup(NodeTreeRedBlack<T> node)
    {
        // Пока не дошли до корня и родитель красный
        while (node != Root && node.Parent.Color == RED)
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

        Root.Color = BLACK;
    }

    private void LeftRotate(NodeTreeRedBlack<T> x)
    {
        var y = x.Right;

        x.right = y.left;
        if (y.Left != null)
            y.Left.Parent = x;

        y.Parent = x.Parent;
        if (x.Parent == null)
            Root = y;
        else if (x == x.Parent.Left)
            x.Parent.left = y;
        else
            x.Parent.right = y;

        y.left = x;
        x.Parent = y;
    }

    private void RightRotate(NodeTreeRedBlack<T> y)
    {
        var x = y.Left;

        y.left = x.right;
        if (x.Right != null)
            x.Right.Parent = y;

        x.Parent = y.Parent;
        if (y.Parent == null)
            Root = x;
        else if (y == y.Parent.Right)
            y.Parent.right = x;
        else
            y.Parent.left = x;

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
    public override void Remove(T value)
    {
        var nodeToDelete = (NodeTreeRedBlack<T>)FindNode(Root, value);
        if (nodeToDelete == null) return;

        var originalColor = nodeToDelete.Color;
        NodeTreeRedBlack<T> replacementNode;
        NodeTreeRedBlack<T> fixupNode;

        // Случай 1: Узел без детей или только с одним ребенком
        if (nodeToDelete.Left == null)
        {
            fixupNode = nodeToDelete.Right;
            TransplantNode(nodeToDelete, nodeToDelete.Right);
        }
        else if (nodeToDelete.Right == null)
        {
            fixupNode = nodeToDelete.Left;
            TransplantNode(nodeToDelete, nodeToDelete.Left);
        }
        // Случай 2: Узел с двумя детьми
        else
        {
            // Находим преемника (минимальный узел в правом поддереве)
            var successor = FindMin(nodeToDelete.Right);
            originalColor = successor.Color;
            fixupNode = successor.Right;

            if (successor.Parent == nodeToDelete)
            {
                if (fixupNode != null)
                    fixupNode.Parent = successor;
            }
            else
            {
                TransplantNode(successor, successor.Right);
                successor.right = nodeToDelete.Right;
                successor.Right.Parent = successor;
            }

            TransplantNode(nodeToDelete, successor);
            successor.left = nodeToDelete.Left;
            successor.Left.Parent = successor;
            successor.Color = nodeToDelete.Color;
        }

        Count--;

        // Если удалили черный узел, нужно восстановить свойства дерева
        if (originalColor == BLACK && fixupNode != null)
        {
            RemoveFixup(fixupNode);
        }
    }

    private void RemoveFixup(NodeTreeRedBlack<T> node)
    {
        while (node != Root && GetColor(node) == BLACK)
        {
            if (node == node.Parent.Left)
            {
                var sibling = node.Parent.Right;

                // Случай 1: Брат красный
                if (GetColor(sibling) == RED)
                {
                    sibling.Color = BLACK;
                    node.Parent.Color = RED;
                    LeftRotate(node.Parent);
                    sibling = node.Parent.Right;
                }

                // Случай 2: Оба ребенка брата черные
                if (GetColor(sibling.Left) == BLACK && GetColor(sibling.Right) == BLACK)
                {
                    sibling.Color = RED;
                    node = node.Parent;
                }
                else
                {
                    // Случай 3: Правый ребенок брата черный
                    if (GetColor(sibling.Right) == BLACK)
                    {
                        if (sibling.Left != null)
                            sibling.Left.Color = BLACK;
                        sibling.Color = RED;
                        RightRotate(sibling);
                        sibling = node.Parent.Right;
                    }

                    // Случай 4: Правый ребенок брата красный
                    sibling.Color = GetColor(node.Parent);
                    node.Parent.Color = BLACK;
                    if (sibling.Right != null)
                        sibling.Right.Color = BLACK;
                    LeftRotate(node.Parent);
                    node = Root;
                }
            }
            else
            {
                // Симметричный случай
                var sibling = node.Parent.Left;

                if (GetColor(sibling) == RED)
                {
                    sibling.Color = BLACK;
                    node.Parent.Color = RED;
                    RightRotate(node.Parent);
                    sibling = node.Parent.Left;
                }

                if (GetColor(sibling.Right) == BLACK && GetColor(sibling.Left) == BLACK)
                {
                    sibling.Color = RED;
                    node = node.Parent;
                }
                else
                {
                    if (GetColor(sibling.Left) == BLACK)
                    {
                        if (sibling.Right != null)
                            sibling.Right.Color = BLACK;
                        sibling.Color = RED;
                        LeftRotate(sibling);
                        sibling = node.Parent.Left;
                    }

                    sibling.Color = GetColor(node.Parent);
                    node.Parent.Color = BLACK;
                    if (sibling.Left != null)
                        sibling.Left.Color = BLACK;
                    RightRotate(node.Parent);
                    node = Root;
                }
            }
        }
        node.Color = BLACK;
    }

    private void TransplantNode(NodeTreeRedBlack<T> u, NodeTreeRedBlack<T> v)
    {
        if (u.Parent == null)
            Root = v;
        else if (u == u.Parent.Left)
            u.Parent.left = v;
        else
            u.Parent.right = v;

        if (v != null)
            v.Parent = u.Parent;
    }

    private NodeTreeRedBlack<T> FindMin(NodeTreeRedBlack<T> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    private bool GetColor(NodeTreeRedBlack<T> node)
    {
        return node == null ? BLACK : node.Color;
    }

    public void AddRange(T[] values)
    {
        foreach (T value in values)
            Add(value);
    }
    public static string GetVisualizeStringTree(NodeTreeRedBlack<T> root)
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
                    if (node.Color == RedBlackTree<T>.RED)
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
    public override string ToString()
    {
        return GetVisualizeStringTree(Root);
    }


}

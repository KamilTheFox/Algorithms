using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NodeTree<T> where T : IComparable<T>
{
    public T Value { get; set; }

    public NodeTree<T> left;
    public NodeTree<T> right;

    public NodeTree(T value)
    {
        Value = value;
    }
}

public class NodeTreeRedBlack<T> : NodeTree<T> where T : IComparable<T>
{
    public bool Color { get; set; }

    public NodeTreeRedBlack<T> Parent { get; set; }

    public NodeTreeRedBlack<T> Right => (NodeTreeRedBlack<T>)right;
    public NodeTreeRedBlack<T> Left => (NodeTreeRedBlack<T>)left;
    public NodeTreeRedBlack(T value) : base(value)
    {
        Color = true;
    }
}


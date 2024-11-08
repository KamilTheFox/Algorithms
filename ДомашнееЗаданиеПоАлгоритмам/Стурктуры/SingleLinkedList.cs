using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SingleLinkedList<T> : IMyData<T> ,IEnumerable<T> where T : IComparable<T>
{
    //Служебный класс для удобства перечисления
    private class EnumeratorLinkedList : IEnumerator<T>
    {
        private SingleLinkedList<T>? list;
        public EnumeratorLinkedList(SingleLinkedList<T> ts)
        {
            list = ts;
        }
        private int iterator;

        private Node CurrentNode;
        public T Current => CurrentNode.Value;

        object IEnumerator.Current => CurrentNode.Value;

        public void Dispose()
        {
            CurrentNode = null;
            iterator = 0;
        }

        public bool MoveNext()
        {
            if (list.Count == iterator)
                return false;
            if (CurrentNode == null)
            {
                CurrentNode = list.head;
                return true;
            }
            if(CurrentNode.nextNode == null)
                return false;
            CurrentNode = CurrentNode.nextNode;
            iterator++;
            return true;
        }

        public void Reset()
        {
            Dispose();
        }
    }
    
    private Node? head;

    private Node? tail;

    public int Count { get; private set; } = 0;

    private class Node
    {
        public Node(T obj)
        {
            value = obj;
        }
        public T Value => value;
        private T value = default(T);
        public Node? nextNode;
    }

    public void Add(T obj)
    {
        if (InitializeHeadTail(obj))
            return;
        Node newNode = new Node(obj);
        tail.nextNode = newNode;
        tail = newNode;
        Count++;
    }

    public void AddRange(T[] objs)
    {
        foreach(var obj in objs)
            Add(obj);
    }
    //Поиск нод по содержимому
    private (Node?,Node?) FindDataNode(T searchObj)
    {
        Node? parentNode = null;
        Node? currentNode = head;
        while(currentNode != null)
        {
            if (EqualityComparer<T>.Default.Equals(currentNode.Value, searchObj))
                return (parentNode, currentNode);
            parentNode = currentNode;
            currentNode = currentNode.nextNode;
        }
        return (null, null);
    }
    //Поиск нод по индексу
    private (Node?, Node?) FindDataNodeAt(int index)
    {
        Node? parentNode = null;
        Node? currentNode = head;
        int iter = 0;
        if (currentNode != null)
            while (currentNode != null)
            {
                if (iter == index)
                    return (parentNode, currentNode);
                iter++;
                parentNode = currentNode;
                currentNode = currentNode.nextNode;
            }
        return (null, null);
    }

    public void Remove(T obj)
    {
        RemoveElement(FindDataNode(obj));
    }
    private void RemoveElement((Node, Node) nods)
    {
        Node parent = nods.Item1;
        Node removed = nods.Item2;
        if (removed == null)
            return;
        if (parent == null)
        {
            head = removed.nextNode;
        }
        else
        {
            if (removed == tail)
            {
                parent.nextNode = null;
                tail = parent;
                Count--;
                return;
            }
            parent.nextNode = removed.nextNode;
        }
        Count--;
    }
    public void RemoveAt(int index)
    {
        RemoveElement(FindDataNodeAt(index));
    }
    private bool InitializeHeadTail(T obj)
    {
        if (head == null)
        {
            Count = 1;
            head = new Node(obj);
            tail = head;
            return true;
        }
        return false;
    }
    //Изменить направление списка
    public void Reverse()
    {
        if (head == null || head.nextNode == null)
            return; 
        Node previous = null;
        Node current = head;
        tail = current;
        while (current != null)
        {
            Node next = current.nextNode;
            current.nextNode = previous;
            previous = current;
            current = next;
        }
        head = previous;
    }
    //Получение перечисления
    public IEnumerator<T> GetEnumerator()
    {
        return new EnumeratorLinkedList(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new EnumeratorLinkedList(this);
    }

    public object Clone()
    {
        SingleLinkedList<T> list = new SingleLinkedList<T>();
        list.AddRange(this.ToArray());
        return list;
    }
    //Дублирование в интерфейс
    public void Inverse()
    {
        Reverse();
    }
    public override string ToString()
    {
        return String.Join(", ", this.ToArray());
    }

    public T Find(T value)
    {
        var node = FindNode(head, value);
        if (node == null)
            return default(T);
        return node.Value;
    }

    private Node FindNode(Node node, T value)
    {
        if (node == null || node.Value.CompareTo(value) == 0)
            return node;
        return FindNode(node.nextNode, value);
    }

    public void Dispose()
    {
        Node current = head;
        while (current != null)
        {
            Node next = current.nextNode;
            current.nextNode = null;
            current = next;
        }
        head = null;
        tail = null;
    }
}

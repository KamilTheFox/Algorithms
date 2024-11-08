using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Словарик хранящий ключ - значение. Реализация 2х системных интерфейсов для итерации по foreach и реализация словаря IDictionary
public class DictionaryTree<Key,Value> : IEnumerable<DictionaryTree<Key, Value>.MyKeyValuePairs>, IDictionary<Key,Value>
{
    //Хранение данных Ключ - значение
    public struct MyKeyValuePairs : IComparable<MyKeyValuePairs>
    {
        public Key Key { get; set; }
        public Value Value { get; set; }
        public int CompareTo(MyKeyValuePairs other)
        {
            return this.Key.GetHashCode() - other.Key.GetHashCode();
        }
        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
        //Переопределение типов из Key для поиска и переопределение KeyValuePair в MyKeyValuePairs и наоборот.
        //(ЧТо бы избавиться от конфликтов от моей и системной реализации)
        public static implicit operator MyKeyValuePairs(Key Key)
        {
            return new MyKeyValuePairs() { Key = Key };
        }
        public static implicit operator MyKeyValuePairs(KeyValuePair<Key, Value> keyValuePair)
        {
            return new MyKeyValuePairs() { Key = keyValuePair.Key, Value = keyValuePair.Value};
        }
        public static implicit operator KeyValuePair<Key, Value>(MyKeyValuePairs keyValuePair)
        {
            return new KeyValuePair<Key, Value>(keyValuePair.Key, keyValuePair.Value);
        }
    }

    //Интерфейс который хранит в себе любую реализацию данных. Бинарное AVL Tree,
    //Красно-черное Tree, односвязный список или двусвязный список, без разницы
    IMyData<MyKeyValuePairs> dataBase;

    public ICollection<Key> Keys => dataBase.ToArray().Select(x => x.Key).ToArray();

    public ICollection<Value> Values => dataBase.ToArray().Select(x => x.Value).ToArray();

    public int Count => dataBase.Count();

    public bool IsReadOnly => false;

    //Получение значения. Удобрый доступ по параметру указателя
    public Value this[Key key]
    {
        get
        {
            return GetValue(key);
        }
        set
        {
            Remove(key);
            Add(key, value);
        }
    }
    //Не зависимо от индекса словарь будет работать одинаково.
    public DictionaryTree(int index = 0)
    {
        switch (index)
        {
            case 0:
            dataBase = new RedBlackTree1<MyKeyValuePairs>();
                break;
            case 1:
                dataBase = new RedBlackTree2<MyKeyValuePairs>();
                break;
            case 2:
            dataBase = new BinaryAVLTree<MyKeyValuePairs>();
                break;
            case 3:
            dataBase = new SingleLinkedList<MyKeyValuePairs>();
                break;
        }
    }
    //Получение значения
    public Value GetValue(Key key)
    {
        MyKeyValuePairs dataDictionary = dataBase.Find(key);
        if(dataDictionary.Value == null)
            return default(Value);
        return dataDictionary.Value;
    }
    public bool Remove(Key key)
    {
        if (ContainsKey(key))
        {
            dataBase.Remove(key);
            return true;
        }
        return false;
    }
    //Выводит представление о содержимом в виде выбранного хранения
    public override string ToString()
    {
        return dataBase.ToString();
    }

    public IEnumerator<MyKeyValuePairs> GetEnumerator()
    {
        return dataBase.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dataBase.GetEnumerator();
    }

    public bool ContainsKey(Key key)
    {
        return dataBase.Find(key).Value != null;
    }

    public bool TryGetValue(Key key, [MaybeNullWhen(false)] out Value value)
    {
        MyKeyValuePairs dataDictionary = dataBase.Find(key);
        if (dataDictionary.Value == null)
        {
            value = default(Value);
            return false;
        }
        value = dataDictionary.Value;
        return true;
    }

    public void Add(Key key, Value value)
    {
        MyKeyValuePairs data = new MyKeyValuePairs();
        data.Key = key;
        data.Value = value;
        dataBase.Add(data);
    }

    public void Add(KeyValuePair<Key, Value> item)
    {
        dataBase.Add(item);
    }

    public void Clear()
    {
        dataBase.Dispose();
    }

    public bool Contains(KeyValuePair<Key, Value> item)
    {
        if(TryGetValue(item.Key, out Value obj))
        {
            if (obj.Equals(item.Value))
                return true;
        }
        return false;
    }

    public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
    {
        var arrayKeyValue = ((IEnumerable<KeyValuePair<Key, Value>>)GetEnumerator()).ToArray();
        for(int i = arrayIndex; i < arrayIndex + arrayKeyValue.Length; i++)
        {
            array[i] = arrayKeyValue[i];
        }
    }

    IEnumerator<KeyValuePair<Key, Value>> IEnumerable<KeyValuePair<Key, Value>>.GetEnumerator()
    {
        return dataBase.ToArray().Select(x => (KeyValuePair<Key, Value>)x).GetEnumerator();
    }

    public bool Remove(KeyValuePair<Key, Value> item)
    {
        return Remove(item.Key);
    }
}
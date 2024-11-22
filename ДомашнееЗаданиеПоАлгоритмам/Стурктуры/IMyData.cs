//Интерфейс для взаимозамеяемости
public interface IMyData<T> : ICloneable, IEnumerable<T>, IDisposable
{
    void Add(T value);
    void AddRange(params T[] array);

    void Remove(T value);

    void Inverse();

    string ToString();

    T Find(T search);
}

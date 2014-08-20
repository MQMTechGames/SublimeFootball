using System.Collections.Generic;

[System.Serializable]
public class SimpleList<T>
{
    List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
    }

    public bool IsTypeOfAt(int idx, System.Type type)
    {
        System.Object item = _items[idx];

        return item.GetType() == type;
    }

    public T GetAt(int idx)
    {
        return _items[idx];
    }

    public I GetAtAs<I>(int idx) where I : class
    {
        return _items[idx] as I;
    }

    public T GetAtAndRemove(int idx)
    {
        T item = _items[idx];
        _items.RemoveAt(idx);

        return item;
    }

    public int Count()
    {
        return _items.Count;
    }

    public void Clear()
    {
        _items.Clear();
    }
}

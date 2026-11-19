using System.Collections.Generic;

public class ResourcePool<T> where T : class
{
    private readonly Queue<T> _items = new Queue<T>();

    public int Count => _items.Count;

    public bool TryTake(out T item)
    {
        if (_items.Count == 0)
        {
            item = default;
            return false;
        }

        item = _items.Dequeue();
        return true;
    }

    public void Store(T item)
    {
        if (item == null)
        {
            throw new System.ArgumentNullException(nameof(item));
        }

        _items.Enqueue(item);
    }
}
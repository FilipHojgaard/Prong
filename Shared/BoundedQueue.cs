using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Prong.Shared;

public class BoundedQueue<T>
{
    private Queue<T> _queue = new Queue<T>();
    private int _maxSize = 3;

    public BoundedQueue(int MaxSize)
    {
        _maxSize = MaxSize;
    }

    public void Enqueue(T item)
    {
        if (_queue.Count >= _maxSize)
        {
            _queue.Dequeue();
        }
        _queue.Enqueue(item);
    }

    public T Dequeue()
    {
        return _queue.Dequeue();
    }
    public int Count()
    {
        return _queue.Count;
    }

    public bool Contains(T x)
    {
        if (x == null)
        {
            return false;
        }
        return _queue.Contains(x);
    }
}

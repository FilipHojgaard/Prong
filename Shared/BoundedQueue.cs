using System.Collections.Generic;

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
        if (_queue.Count > _maxSize)
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
}

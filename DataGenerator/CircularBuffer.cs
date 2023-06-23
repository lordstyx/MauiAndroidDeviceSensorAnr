namespace MauiApp1.DataGenerator;

/// <summary>
/// Plucked this off of the internet somewhere, forgot where 😅.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CircularBuffer<T> : IEnumerable<T>
{
    private readonly int _size;
    private readonly object _locker;

    private int _count;
    private int _head;
    private int _rear;
    private readonly T[] _values;

    public CircularBuffer(int max)
    {
        _size = max;
        _locker = new object();
        _count = 0;
        _head = 0;
        _rear = 0;
        _values = new T[_size];
    }

    private static int Increment(int index, int size)
    {
        return (index + 1) % size;
    }

    private int Decrement(int index, int size)
    {
        var result = index - size;
        return result >= 0
            ? result
            : _size + result;
    }

    private void UnsafeEnsureQueueNotEmpty()
    {
        if (_count == 0)
            throw new Exception("Empty queue");
    }

    public int Size { get { return _size; } }
    public object SyncRoot { get { return _locker; } }

    #region Count

    public int Count { get { return UnsafeCount; } }
    public int SafeCount { get { lock (_locker) { return UnsafeCount; } } }
    public int UnsafeCount { get { return _count; } }

    #endregion

    #region Enqueue

    public void Enqueue(T obj)
    {
        UnsafeEnqueue(obj);
    }

    public void SafeEnqueue(T obj)
    {
        lock (_locker) { UnsafeEnqueue(obj); }
    }

    public void UnsafeEnqueue(T obj)
    {
        _values[_rear] = obj;

        if (Count == Size)
            _head = Increment(_head, Size);
        _rear = Increment(_rear, Size);
        _count = Math.Min(_count + 1, Size);
    }

    #endregion

    #region Dequeue

    public T Dequeue()
    {
        return UnsafeDequeue();
    }

    public T SafeDequeue()
    {
        lock (_locker) { return UnsafeDequeue(); }
    }

    public T UnsafeDequeue()
    {
        UnsafeEnsureQueueNotEmpty();

        T res = _values[_head];
        _values[_head] = default;
        _head = Increment(_head, Size);
        _count--;

        return res;
    }

    #endregion

    #region Peek

    public T Peek(int offset = 0)
    {
        return UnsafePeek(offset);
    }

    public T SafePeek(int offset = 0)
    {
        lock (_locker) { return UnsafePeek(offset); }
    }

    public T UnsafePeek(int offset = 0)
    {
        UnsafeEnsureQueueNotEmpty();

        return _values[Decrement(_head, offset)];
    }

    #endregion


    #region GetEnumerator

    public IEnumerator<T> GetEnumerator()
    {
        return UnsafeGetEnumerator();
    }

    public IEnumerator<T> SafeGetEnumerator()
    {
        lock (_locker)
        {
            List<T> res = new List<T>(_count);
            var enumerator = UnsafeGetEnumerator();
            while (enumerator.MoveNext())
                res.Add(enumerator.Current);
            return res.GetEnumerator();
        }
    }

    public IEnumerator<T> UnsafeGetEnumerator()
    {
        int index = _head;
        for (int i = 0; i < _count; i++)
        {
            yield return _values[index];
            index = Increment(index, _size);
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}

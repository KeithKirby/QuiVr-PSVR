using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class ResArray<T> where T : new()
    {
        public ResArray(int capacity = 4)
        {
            _values = new T[capacity];
            Capacity = capacity;
        }

        public int Capacity { get; private set; }
        public int Count { get; private set; }
        T[] _values = new T[0];

        public T this[int i]
        {
            get { return _values[i]; }
            set { _values[i] = value; }
        }

        public void Resize(int size)
        {
            Capacity = size;
            Array.Resize(ref _values, Capacity);
        }

        public void Insert(int position, T value)
        {
            if (++Count >= Capacity)
                Resize(Count + ((Count + 1) >> 1));
            for (int i = Count - 1; i > position; --i)
                _values[i] = _values[i - 1];
            _values[position] = value;
        }

        public void Add(T value)
        {
            if (Count == Capacity)
                Resize(Count + ((Count + 1) >> 1));
            _values[Count++] = value;
        }

        public T AddCreate()
        {
            if (Count == Capacity)
                Resize(Count + ((Count + 1) >> 1));
            if (null == _values[Count])
                _values[Count] = new T();
            return _values[Count++];
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}

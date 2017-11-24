using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo11
{
    class Program
    {
        static void Main()
        {
            var pole = new MyThreadSafeList<int>();
            var tasks = new Task[1000];

            for (int i = 0; i < 1000; i++)
            {
                tasks[i] = Task.Factory.StartNew(index =>
                {
                    Console.WriteLine("Index {0}: pole.Count={1}", index, pole.Count);
                    pole.Add((int) index);
                    Console.WriteLine("{0} added. pole.Count={1}", index, pole.Count);

                }, i);
            }

            Task.WaitAll(tasks);

            bool ok = true;
            for (int i = 0; i < 1000; i++)
            {
                if (!pole.Contains(i))
                {
                    Console.WriteLine("{0} not found");
                    ok = false;
                }
            }

            if(ok)
                Console.WriteLine("Everything OK");
            
            Console.ReadLine();

        }
    }


    public class MyThreadSafeList<T> : IList<T>
    {
        //lepsi by bylo pouzit reader writer lock
        private readonly IList<T> _internalList;

        private readonly Semaphore _semaphore = new Semaphore(1, 1);


        public MyThreadSafeList()
        {
            _internalList = new List<T>();
        }


        public void Add(T item)
        {
            _semaphore.WaitOne();
            _internalList.Add(item);
            _semaphore.Release();
        }

        public void Clear()
        {
            _semaphore.WaitOne();

            try
            {
                _internalList.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public bool Contains(T item)
        {
            _semaphore.WaitOne();
            try
            {
                return _internalList.Contains(item);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _semaphore.WaitOne();
            try
            {
                _internalList.CopyTo(array, arrayIndex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public bool Remove(T item)
        {
            _semaphore.WaitOne();
            try
            {
                return _internalList.Remove(item);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public int Count
        {
            get
            {
                _semaphore.WaitOne();
                try
                {
                    return _internalList.Count;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public bool IsReadOnly => _internalList.IsReadOnly;

        public int IndexOf(T item)
        {
            _semaphore.WaitOne();
            try
            {
                return _internalList.IndexOf(item);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Insert(int index, T item)
        {
            _semaphore.WaitOne();
            try
            {
                _internalList.Insert(index, item);
            }
            finally
            {
                _semaphore.Release();
            }           
        }

        public void RemoveAt(int index)
        {
            _semaphore.WaitOne();
            try
            {
                _internalList.RemoveAt(index);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public T this[int index]
        {
            get
            {
                _semaphore.WaitOne();
                try
                {
                    return _internalList[index];
                }
                finally
                {
                    _semaphore.Release();
                }

            }
            set
            {
                _semaphore.WaitOne();
                try
                {
                    _internalList[index] = value;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}

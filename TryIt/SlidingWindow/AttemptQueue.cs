using System;
using System.Collections.Concurrent;
using System.Linq;

namespace TryIt
{
    public class AttemptQueue<T>
    {

        public AttemptQueue(int length)
        {
            _attempt = new ConcurrentQueue<T>();
            _lockObject = new object();
            _length = length;
        }

        private readonly ConcurrentQueue<T> _attempt = null;
        private readonly object _lockObject;

        private readonly int _length;
        public void Enqueue(T obj)
        {
            _attempt.Enqueue(obj);
            lock (_lockObject)
            {
                while (_attempt.Count > _length && _attempt.TryDequeue(out T overflow));
            }
        }

        public void RemoveAll(Func<T, bool> condition)
        {
            var result = _attempt.Count<T>(condition);

            lock (_lockObject)
            {
                while (_attempt.TryDequeue(out T overflow) && --result > 0) ;
            }
        }

        public int Count()
        {
            return _attempt.Count;
        }

        public T Max()
        {
            lock (_lockObject)
            {
                if (_attempt.Count > 0)
                {
                    return _attempt.Max<T>();
                }
            }

            return default(T);
        }
    }

}

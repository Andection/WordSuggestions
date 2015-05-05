using System;
using System.Collections.Generic;
using System.Linq;

namespace SuggestionApp
{
    public class LimitPriorityQueue<T>
    {
        private readonly LinkedList<T> _list = new LinkedList<T>();

        private readonly int _limit;
        private readonly IComparer<T> _comparer;

        public LimitPriorityQueue(int limit, IComparer<T> comparer)
        {
            if (limit <= 0)
                throw new ArgumentException("limit must be greater 0");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _limit = limit;
            _comparer = comparer;
        }

        public void Add(T elem)
        {
            var current = _list.First;
            while (current != null)
            {
                if (_comparer.Compare(current.Value, elem) > 0)
                {
                    _list.AddBefore(current, elem);
                    Truncate();
                    return;
                }
                current = current.Next;
            }

            if (_list.Count < _limit)
            {
                _list.AddLast(elem);
            }
        }

        private void Truncate()
        {
            if (_list.Count > _limit)
            {
                _list.RemoveLast();
            }
        }

        public IEnumerable<T> GetItems()
        {
            return _list.ToArray();
        }
    }
}
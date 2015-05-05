using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class LimitPriorityQueueTests
    {
        private int _limit;
        private LimitPriorityQueue<int> _queue;

        [SetUp]
        public void SetUp()
        {
            _limit = 10;
            _queue = new LimitPriorityQueue<int>(_limit, new IntComparer());
        }

        [Test]
        public void should_add_elem_when_queue_is_empty()
        {
            const int expected = 5;
            _queue.Add(expected);
            var res = _queue.GetItems();
            res.Should().HaveCount(1);
            res.Should().Contain(expected);
        }

        [Test]
        public void should_add_with_order()
        {
            var random = new Random();
            for (var i = 0; i < _limit*2; i++)
            {
                _queue.Add(random.Next(0, 10000));
            }
            var res = _queue.GetItems();

            res.Should().HaveCount(_limit);

            res.Should().BeInAscendingOrder();
        }

        [Test]
        public void should_limit_element_in_queue()
        {
            var random = new Random();
            for (var i = 0; i < _limit*30; i++)
            {
                _queue.Add(random.Next(0, 10000));
            }
            var res = _queue.GetItems();

            res.Should().HaveCount(_limit);
        }

        private class IntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
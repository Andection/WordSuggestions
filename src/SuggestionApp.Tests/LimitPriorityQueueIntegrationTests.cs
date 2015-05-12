using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Service;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class LimitPriorityQueueIntegrationTests
    {
        private int _limit;
        private LimitPriorityQueue<WordInfo> _queue;

        [SetUp]
        public void SetUp()
        {
            _limit = 10;
            _queue = new LimitPriorityQueue<WordInfo>(_limit, new WordComparer());
        }

        [Test]
        public void should_sort_right()
        {
            var expectedOrder = new[]
            {
                new WordInfo()
                {
                    Frequency = 10,
                    Word = "ab"
                },
                new WordInfo()
                {
                    Frequency = 5,
                    Word = "sdf"
                },
                new WordInfo()
                {
                    Frequency = 3,
                    Word = "AA"
                },
                new WordInfo()
                {
                    Frequency = 3,
                    Word = "AB"
                }
            };

            foreach (var wordInfo in expectedOrder.Reverse())
            {
                _queue.Add(wordInfo);
            }

            _queue.GetItems().Should().BeEquivalentTo(expectedOrder);
        }

    }
}
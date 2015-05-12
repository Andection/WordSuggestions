using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Service;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class WordComparerTests
    {
        private WordComparer _comparer;

        private readonly object[][] _comparastionTestCases = new[]
            {

                new object[]
                    {
                        new WordInfo(){Frequency = 10,Word = "swer"},
                        new WordInfo(){Frequency = 10,Word = "swer"},
                        0
                    },
                new object[]
                    {
                        new WordInfo(){Frequency = 10,Word = "swer"},
                        new WordInfo(){Frequency = 12,Word = "swer"},
                        -1
                    },
                new object[]           
                    {
                        new WordInfo(){Frequency = 10,Word = "swer"},
                        new WordInfo(){Frequency = 7,Word = "swer"},
                        1
                    },
                new object[]
                    {
                        new WordInfo(){Frequency = 10,Word = "s"},
                        new WordInfo(){Frequency = 10,Word = "swer"},
                        -1
                    }
            };

        [SetUp]
        public void SetUp()
        {
            _comparer = new WordComparer();
        }

        [Test,Sequential]
        [TestCaseSource("_comparastionTestCases")]
        public void should_compare(WordInfo word1, WordInfo word2, int result)
        {
            if (result == 0)
            {
                _comparer.Compare(word1, word2).Should().Be(0);
            }
            else if (result < 0)
            {
                _comparer.Compare(word1, word2).Should().BeLessThan(0);
            }
            else
            {
                _comparer.Compare(word1, word2).Should().BeGreaterThan(0);
            }
        }
    }
}
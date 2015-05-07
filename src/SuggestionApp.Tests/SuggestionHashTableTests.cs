using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Service;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class SuggestionHashTableTests
    {
        private SuggestionHashTable _hashTable;

        [SetUp]
        public void SetUp()
        {
            _hashTable = new SuggestionHashTable();
        }

        [Test]
        public void should_add_word()
        {
            var expectedWord = new WordInfo
                {
                    Frequency = 1, 
                    Word = "word"
                };

            _hashTable.Add(expectedWord);

            _hashTable.Find(expectedWord.Word).Should().Contain(expectedWord);
        }
    }
}
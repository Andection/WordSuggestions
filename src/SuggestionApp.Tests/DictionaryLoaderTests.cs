using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SuggestionApp.Tests
{
    /// <summary>
    /// Summary description for DictionaryLoaderTests
    /// </summary>
    [TestFixture]
    public class DictionaryLoaderTests
    {
        [Test]
        public void DictionaryLoader_should_load_words_from_stream()
        {
            /*
               kare 10
               kanojo 10
               karetachi 1
               korosu 7
               ksakura 3
             */

            var expectedWords = new List<WordInfo>()
                {
                    new WordInfo {Frequency = 10, Word = "kare"},
                    new WordInfo {Frequency = 10, Word = "kanojo"},
                    new WordInfo {Frequency = 1, Word = "karetachi"},
                    new WordInfo {Frequency = 7, Word = "korosu"},
                    new WordInfo {Frequency = 3, Word = "ksakura"}
                };
            using (var stream = new StreamReader("input mini.txt"))
            {
                var dictionary = DictionaryLoader.Load(stream);
                var actualWords = dictionary.Find("k");
                actualWords.Should().HaveCount(expectedWords.Count);
                actualWords.Should().Contain(actual => expectedWords.Any(expected => expected.Frequency == actual.Frequency && expected.Word == actual.Word));
            }
        }
    }
}

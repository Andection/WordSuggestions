using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Service;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class SuggestionServiceTests
    {
        private SuggestionService _suggestionService;

        private readonly object[][] _suggestionTestCases =
        {

            new object[]
            {
                "k",
                new[] {"kare","kanojo",  "korosu", "karetachi"}
            },

            new object[]
            {
                "ka",
                new[] {"kare", "kanojo", "karetachi"}
            },

            new object[]
            {
                "kar",
                new[] {"kare", "karetachi"}
            }
        };

        [SetUp]
        public void SetUp()
        {
            var words = new List<WordInfo>()
            {
                new WordInfo {Frequency = 10, Word = "kare"},
                new WordInfo {Frequency = 10, Word = "kanojo"},
                new WordInfo {Frequency = 1, Word = "karetachi"},
                new WordInfo {Frequency = 7, Word = "korosu"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"},
                new WordInfo {Frequency = 3, Word = "sekura"}
            };
            var dictionary = new SuggestionHashTable();
            foreach (var word in words)
            {
                dictionary.Add(word);
            }
            _suggestionService = new SuggestionService(dictionary);
        }

        [Test, Sequential]
        [TestCaseSource("_suggestionTestCases")]
        public void Suggest_should_suggest_right_element(string token, string[] expectedResult)
        {
            var actualResult = _suggestionService.Suggest(token);
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test, Sequential]
        [TestCaseSource("_suggestionTestCases")]
        public void Suggest_should_suggest_in_right_order(string token, string[] expectedResult)
        {
            var actualResult = _suggestionService.Suggest(token);
            actualResult.Should().ContainInOrder(expectedResult);
        }

        [Test, Sequential]
        public void Suggest_should_return_10_or_less_words()
        {
            const string token = "se";
            var actualResult = _suggestionService.Suggest(token);
            actualResult.Count().Should().BeLessOrEqualTo(10);
        }
    }
}
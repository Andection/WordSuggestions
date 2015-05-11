using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using WordSuggestion.Service;

namespace SuggestionApp.Tests
{
    [TestFixture]
    public class SpeedTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test, Sequential]
        public void should_faster_then_10_seconds([Values("input.txt")] string fileName)
        {
            var allStopwatch = new Stopwatch();
            using (var stream = new StreamReader(fileName))
            {
                allStopwatch.Start();
                var stopWatchLoader = new Stopwatch();
                stopWatchLoader.Start();
                var dictionary = DictionaryLoader.Load(stream);
                var suggestionService = new SuggestionService(dictionary);
                stopWatchLoader.Stop();

                var stopWatchSuggestion = new Stopwatch();
                stopWatchSuggestion.Start();
                var suggestCount = Convert.ToInt32(stream.ReadLine());
                using (var outputStream = new StreamWriter("output.txt"))
                {
                    for (var i = 0; i < suggestCount; i++)
                    {
                        var token = stream.ReadLine();

                        var result = suggestionService.Suggest(token);
                        foreach (var suggestion in result)
                        {
                            outputStream.WriteLine(suggestion);
                        }
                        outputStream.WriteLine();
                    }
                }
                stopWatchSuggestion.Stop();
                allStopwatch.Stop();
                Console.WriteLine(string.Format("load seconds:{0}", stopWatchLoader.Elapsed.TotalSeconds));
                Console.WriteLine(string.Format("suggestion seconds:{0}", stopWatchSuggestion.Elapsed.TotalSeconds));
                Console.WriteLine(string.Format("total seconds:{0}", allStopwatch.Elapsed.TotalSeconds));
                allStopwatch.Elapsed.TotalSeconds.Should().BeLessOrEqualTo(10);
            }
        }
    }
}
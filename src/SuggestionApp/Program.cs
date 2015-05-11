using System;
using System.Collections.Generic;
using System.IO;
using WordSuggestion.Service;

namespace SuggestionApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var stream = new StreamReader(Console.OpenStandardInput()))
            {
                var dictionary = DictionaryLoader.Load(stream);
                var suggestionService = new SuggestionService(dictionary);

                var suggestCount = Convert.ToInt32(stream.ReadLine());
                using (var streamWriter = new StreamWriter(Console.OpenStandardOutput()))
                {
                    for (var i = 0; i < suggestCount; i++)
                    {
                        var token = stream.ReadLine();
                        var result = suggestionService.Suggest(token);
                        WriteSuggestions(result, streamWriter);
                    }
                }
            }
        }

        private static void WriteSuggestions(IEnumerable<string> result, TextWriter streamWriter)
        {
            foreach (var suggestion in result)
            {
                streamWriter.WriteLine(suggestion);
            }
            streamWriter.WriteLine();
        }
    }
}
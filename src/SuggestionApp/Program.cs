using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SuggestionApp
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (var stream = new StreamReader(Console.OpenStandardInput()))
            {
                var dictionary = DictionaryLoader.Load(stream);
                SuggestionManager.Init(dictionary);

                var suggestCount = Convert.ToInt32(stream.ReadLine());
                using (var streamWriter = new StreamWriter(Console.OpenStandardOutput()))
                {
                    for (var i = 0; i < suggestCount; i++)
                    {
                        var token = stream.ReadLine();
                        var result = SuggestionManager.Suggest(token);
                        WriteSuggestions(result, streamWriter);
                    }
                }
            }
        }

        private static void WriteSuggestions(IEnumerable<string> result, StreamWriter streamWriter)
        {
            foreach (var suggestion in result)
            {
                streamWriter.WriteLine(suggestion);
            }
            streamWriter.WriteLine();
        }
    }
}

using System;
using System.IO;

namespace SuggestionApp
{
    public static class DictionaryLoader
    {
        public static SuggestionHashTable Load(StreamReader stream)
        {
            var wordCount = Convert.ToInt32(stream.ReadLine());
            var words = new SuggestionHashTable();
            for (var i = 0; i < wordCount; i++)
            {
                var word = Map(stream.ReadLine());
                words.Add(word);
            }
            return words;
        }

        private static WordInfo Map(string wordInfoText)
        {
            var strings = wordInfoText.Split(' ');
            return new WordInfo
                {
                    Word = strings[0],
                    Frequency = Convert.ToInt32(strings[1])
                };
        }
    }
}
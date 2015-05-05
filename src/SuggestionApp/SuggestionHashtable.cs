using System.Collections.Generic;
using System.Linq;

namespace SuggestionApp
{
    public class SuggestionHashTable
    {
        private readonly Dictionary<string, WordStorage> _storageList = new Dictionary<string, WordStorage>();

        public SuggestionHashTable()
        {
        }

        public void Add(WordInfo word)
        {
            WordStorage wordStorage;
            var key = GetKey(word.Word);
            if (!_storageList.TryGetValue(key, out wordStorage))
            {
                wordStorage = new WordStorage(key);
                _storageList.Add(key, wordStorage);
            }

            wordStorage.AddWord(word);
        }

        private static string GetKey(string word)
        {
            return word.First().ToString();
        }

        public IEnumerable<WordInfo> Find(string token)
        {
            var key = GetKey(token);
            WordStorage storage;
            return !_storageList.TryGetValue(key, out storage) 
                                ? Enumerable.Empty<WordInfo>() 
                                : storage.Words(token);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace WordSuggestion.Service
{
    public class WordStorage
    {
        private readonly string _key;
        private readonly LimitPriorityQueue<WordInfo> _words = new LimitPriorityQueue<WordInfo>(10, new WordComparer());
        private readonly Dictionary<string, WordStorage> _storageList = new Dictionary<string, WordStorage>();

        public WordStorage(string key)
        {
            _key = key;
        }

        public IEnumerable<WordInfo> Words(string token)
        {
            if (token == _key)
            {
                return _words.GetItems();
            }
            var newKey = GetNewKey(token);
            WordStorage storage;
            return _storageList.TryGetValue(newKey, out storage) ? storage.Words(token) : Enumerable.Empty<WordInfo>();
        }

        public void AddWord(WordInfo word)
        {
            _words.Add(word);

            AddToStorage(word);
        }

        private void AddToStorage(WordInfo word)
        {
            if (word.Word == _key)
                return;

            var newKey = GetNewKey(word.Word);

            WordStorage wordStorage;
            if (!_storageList.TryGetValue(newKey, out wordStorage))
            {
                wordStorage = new WordStorage(newKey);
                _storageList.Add(newKey, wordStorage);
            }

            wordStorage.AddWord(word);
        }

        private string GetNewKey(string word)
        {
            var keyLength = _key.Length + 1;
            var newKey = word;
            if (newKey.Length > keyLength)
            {
                newKey = newKey.Remove(keyLength);
            }
            return newKey;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace SuggestionApp
{
    public static class SuggestionManager
    {
        private static SuggestionHashTable _hashTable;

        public static void Init(SuggestionHashTable hashTable)
        {
            _hashTable = hashTable;
        }

        public static IEnumerable<string> Suggest(string token)
        {
            return _hashTable.Find(token).Select(s => s.Word).ToArray();
        }
    }
}
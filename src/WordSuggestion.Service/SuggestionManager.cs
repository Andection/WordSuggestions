using System;
using System.Collections.Generic;
using System.Linq;

namespace WordSuggestion.Service
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
            if (token == null)
                throw new ArgumentNullException("token");

            var normalizedToken = token.ToLower();

            return _hashTable.Find(normalizedToken).Select(s => s.Word).ToArray();
        }
    }
}
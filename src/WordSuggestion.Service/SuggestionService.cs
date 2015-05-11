using System;
using System.Collections.Generic;
using System.Linq;

namespace WordSuggestion.Service
{
    public class SuggestionService
    {
        private static SuggestionHashTable _hashTable;

        public SuggestionService(SuggestionHashTable hashTable)
        {
            _hashTable = hashTable;
        }

        public IEnumerable<string> Suggest(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            var normalizedToken = token.ToLower();

            return _hashTable.Find(normalizedToken).Select(s => s.Word).ToArray();
        }
    }
}
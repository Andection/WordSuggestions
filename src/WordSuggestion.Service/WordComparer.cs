using System.Collections.Generic;

namespace WordSuggestion.Service
{
    public class WordComparer : IComparer<WordInfo>
    {
        public int Compare(WordInfo x, WordInfo y)
        {
            var frequencyComparison =x.Frequency.CompareTo(y.Frequency);
            if (frequencyComparison != 0)
            {
                return frequencyComparison;
            }
            return System.String.Compare(x.Word, y.Word, System.StringComparison.Ordinal);
        }
    }
}
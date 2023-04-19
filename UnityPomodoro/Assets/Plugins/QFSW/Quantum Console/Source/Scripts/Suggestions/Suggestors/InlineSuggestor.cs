using System.Collections.Generic;

namespace QFSW.QC.Suggestors
{
    /// <summary>
    /// Produces the available suggestions for the suggestion system.
    /// </summary>
    public class InlineSuggestor : IQcSuggestor
    {
        public IEnumerable<IQcSuggestion> GetSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            foreach (Tags.InlineSuggestionsTag t in context.GetTags<Tags.InlineSuggestionsTag>())
            {
                foreach (string s in t.Suggestions)
                {
                    yield return new RawSuggestion(s, singleLiteral: true);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC.Suggestors.Tags
{
    /// <summary>
    /// Object that holds all the inlined suggestions provided 
    /// to <see cref="SuggestionsAttribute"/>.
    /// </summary>
    public struct InlineSuggestionsTag : IQcSuggestorTag
    {
        public readonly IEnumerable<string> Suggestions;

        public InlineSuggestionsTag(IEnumerable<string> suggestions)
        {
            Suggestions = suggestions;
        }
    }

    /// <summary>
    /// Provides suggestions for the parameter.
    /// </summary>
    public sealed class SuggestionsAttribute : SuggestorTagAttribute
    {
        private readonly IQcSuggestorTag[] _tags;

        /// <param name="suggestions">String-convertible suggestions.</param>
        public SuggestionsAttribute(params object[] suggestions)
        {
            InlineSuggestionsTag tag = new InlineSuggestionsTag(
                suggestions.Select(o => o.ToString()));
            _tags = new IQcSuggestorTag[] { tag };
        }

        public override IQcSuggestorTag[] GetSuggestorTags() => _tags;
    }
}

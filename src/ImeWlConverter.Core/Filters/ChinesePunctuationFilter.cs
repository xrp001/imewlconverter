using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed partial class ChinesePunctuationFilter : IWordFilter
{
    [GeneratedRegex(@"[\u3002\uff1b\uff0c\uff1a\u201c\u201d\uff08\uff09\u3001\uff1f\u300a\u300b]")]
    private static partial Regex ChinesePunctuationRegex();

    public bool ShouldKeep(WordEntry entry) =>
        !ChinesePunctuationRegex().IsMatch(entry.Word);
}

public sealed partial class ChinesePunctuationRemoveTransform : IWordTransform
{
    [GeneratedRegex(@"[\u3002\uff1b\uff0c\uff1a\u201c\u201d\uff08\uff09\u3001\uff1f\u300a\u300b]")]
    private static partial Regex ChinesePunctuationRegex();

    public WordEntry? Transform(WordEntry entry)
    {
        var result = ChinesePunctuationRegex().Replace(entry.Word, "");
        return string.IsNullOrEmpty(result) ? null : entry with { Word = result };
    }
}

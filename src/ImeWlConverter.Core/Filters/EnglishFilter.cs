using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed partial class EnglishFilter : IWordFilter
{
    [GeneratedRegex("[a-zA-Z]")]
    private static partial Regex EnglishRegex();

    public bool ShouldKeep(WordEntry entry) =>
        !EnglishRegex().IsMatch(entry.Word);
}

public sealed partial class EnglishRemoveTransform : IWordTransform
{
    [GeneratedRegex("[a-zA-Z]")]
    private static partial Regex EnglishRegex();

    public WordEntry? Transform(WordEntry entry)
    {
        var result = EnglishRegex().Replace(entry.Word, "");
        return string.IsNullOrEmpty(result) ? null : entry with { Word = result };
    }
}

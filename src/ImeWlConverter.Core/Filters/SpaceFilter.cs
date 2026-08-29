using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class SpaceFilter : IWordFilter
{
    public bool ShouldKeep(WordEntry entry) =>
        !entry.Word.Contains(' ');
}

public sealed partial class SpaceRemoveTransform : IWordTransform
{
    [GeneratedRegex(@"\s")]
    private static partial Regex WhitespaceRegex();

    public WordEntry? Transform(WordEntry entry)
    {
        var result = WhitespaceRegex().Replace(entry.Word, "");
        return string.IsNullOrEmpty(result) ? null : entry with { Word = result };
    }
}

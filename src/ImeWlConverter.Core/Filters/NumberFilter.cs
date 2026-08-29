using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed partial class NumberFilter : IWordFilter
{
    [GeneratedRegex(@"\d")]
    private static partial Regex NumberRegex();

    public bool ShouldKeep(WordEntry entry) =>
        !NumberRegex().IsMatch(entry.Word);
}

public sealed partial class NumberRemoveTransform : IWordTransform
{
    [GeneratedRegex(@"\d")]
    private static partial Regex NumberRegex();

    public WordEntry? Transform(WordEntry entry)
    {
        var result = NumberRegex().Replace(entry.Word, "");
        return string.IsNullOrEmpty(result) ? null : entry with { Word = result };
    }
}

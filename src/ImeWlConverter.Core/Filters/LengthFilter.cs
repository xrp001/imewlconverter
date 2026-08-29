using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class LengthFilter : IWordFilter
{
    public int MinLength { get; init; } = 1;
    public int MaxLength { get; init; } = 9999;

    public bool ShouldKeep(WordEntry entry) =>
        entry.Word.Length >= MinLength && entry.Word.Length <= MaxLength;
}

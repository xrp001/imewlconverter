using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class RankFilter : IWordFilter
{
    public int MinRank { get; init; } = 1;
    public int MaxRank { get; init; } = 999999;

    public bool ShouldKeep(WordEntry entry) =>
        entry.Rank >= MinRank && entry.Rank <= MaxRank;
}

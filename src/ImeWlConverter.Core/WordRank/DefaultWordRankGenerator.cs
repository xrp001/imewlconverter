using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.WordRank;

/// <summary>
/// Assigns a fixed rank value to entries that have no rank (or all entries if ForceOverride).
/// </summary>
public sealed class DefaultWordRankGenerator : IWordRankGenerator
{
    public int DefaultRank { get; init; } = 1;
    public bool ForceOverride { get; init; }

    public int GenerateRank(WordEntry entry) => DefaultRank;

    public Task<IReadOnlyList<WordEntry>> GenerateRanksAsync(
        IReadOnlyList<WordEntry> entries, CancellationToken ct = default)
    {
        var result = new List<WordEntry>(entries.Count);
        foreach (var entry in entries)
        {
            if (entry.Rank == 0 || ForceOverride)
                result.Add(entry with { Rank = GenerateRank(entry) });
            else
                result.Add(entry);
        }
        return Task.FromResult<IReadOnlyList<WordEntry>>(result);
    }
}

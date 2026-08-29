using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Abstractions.Contracts;

/// <summary>
/// Generates word frequency rank for entries that lack rank information.
/// </summary>
public interface IWordRankGenerator
{
    /// <summary>Generate rank for a single word entry.</summary>
    int GenerateRank(WordEntry entry);

    /// <summary>Generate ranks for a batch of entries (useful for LLM-based generators).</summary>
    Task<IReadOnlyList<WordEntry>> GenerateRanksAsync(
        IReadOnlyList<WordEntry> entries,
        CancellationToken ct = default);
}

using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Abstractions.Contracts;

/// <summary>
/// Determines whether a word entry should be kept in the output.
/// </summary>
public interface IWordFilter
{
    /// <summary>Returns true if the entry should be kept.</summary>
    bool ShouldKeep(WordEntry entry);
}

/// <summary>
/// Transforms a word entry (e.g., replacing content).
/// </summary>
public interface IWordTransform
{
    /// <summary>Transforms the entry, returning a new entry or null to remove it.</summary>
    WordEntry? Transform(WordEntry entry);
}

/// <summary>
/// Batch filter that processes an entire collection (e.g., deduplication).
/// </summary>
public interface IBatchFilter
{
    /// <summary>Filter a batch of entries.</summary>
    IReadOnlyList<WordEntry> Filter(IReadOnlyList<WordEntry> entries);
}

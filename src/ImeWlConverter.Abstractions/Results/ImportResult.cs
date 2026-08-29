using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Abstractions.Results;

/// <summary>Result of an import operation.</summary>
public sealed record ImportResult
{
    /// <summary>The imported word entries.</summary>
    public required IReadOnlyList<WordEntry> Entries { get; init; }

    /// <summary>Number of lines/entries that failed to parse.</summary>
    public int ErrorCount { get; init; }

    /// <summary>Error messages for failed entries.</summary>
    public IReadOnlyList<string> Errors { get; init; } = [];
}

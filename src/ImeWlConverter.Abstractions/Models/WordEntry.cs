using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Abstractions.Models;

/// <summary>
/// Represents a single word entry in an IME dictionary.
/// Immutable value type for safe pipeline processing.
/// </summary>
public sealed record WordEntry
{
    /// <summary>The word or phrase text.</summary>
    public required string Word { get; init; }

    /// <summary>The encoding/code associated with this word.</summary>
    public WordCode? Code { get; init; }

    /// <summary>The frequency rank (higher = more common).</summary>
    public int Rank { get; init; }

    /// <summary>The type of code associated with this entry.</summary>
    public CodeType CodeType { get; init; } = CodeType.Pinyin;

    /// <summary>Whether this entry is an English word.</summary>
    public bool IsEnglish { get; init; }
}

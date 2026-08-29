namespace ImeWlConverter.Abstractions.Enums;

/// <summary>
/// Sorting strategy for word entries.
/// </summary>
public enum SortType
{
    /// <summary>Default order (as imported).</summary>
    Default = 0,

    /// <summary>Sort by word text.</summary>
    ByWord,

    /// <summary>Sort by encoding/code.</summary>
    ByCode,

    /// <summary>Sort by frequency rank.</summary>
    ByRank
}

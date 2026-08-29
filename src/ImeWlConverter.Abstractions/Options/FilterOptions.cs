namespace ImeWlConverter.Abstractions.Options;

/// <summary>Options for word filtering.</summary>
public sealed class FilterOptions
{
    /// <summary>Minimum word length to keep (0 = no minimum).</summary>
    public int MinLength { get; init; }

    /// <summary>Maximum word length to keep (0 = no maximum).</summary>
    public int MaxLength { get; init; }

    /// <summary>Minimum rank to keep (0 = no minimum).</summary>
    public int MinRank { get; init; }

    /// <summary>Maximum rank to keep (0 = no maximum).</summary>
    public int MaxRank { get; init; }

    /// <summary>Whether to remove entries containing English characters.</summary>
    public bool RemoveEnglish { get; init; }

    /// <summary>Whether to remove entries containing numbers.</summary>
    public bool RemoveNumbers { get; init; }

    /// <summary>Whether to remove entries containing spaces.</summary>
    public bool RemoveSpaces { get; init; }

    /// <summary>Whether to remove entries containing punctuation.</summary>
    public bool RemovePunctuation { get; init; }
}

using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Abstractions.Options;

/// <summary>Options for format export operations.</summary>
public sealed class ExportOptions
{
    /// <summary>The target code type for the output.</summary>
    public CodeType TargetCodeType { get; init; } = CodeType.Pinyin;

    /// <summary>Text encoding name for text-based formats.</summary>
    public string? EncodingName { get; init; }

    /// <summary>Sort type for the output.</summary>
    public SortType SortType { get; init; } = SortType.Default;

    /// <summary>Whether to sort descending.</summary>
    public bool SortDescending { get; init; }
}

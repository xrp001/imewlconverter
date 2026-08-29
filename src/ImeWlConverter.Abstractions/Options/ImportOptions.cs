using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Abstractions.Options;

/// <summary>Options for format import operations.</summary>
public sealed class ImportOptions
{
    /// <summary>The expected code type of the source.</summary>
    public CodeType SourceCodeType { get; init; } = CodeType.Pinyin;

    /// <summary>Text encoding name for text-based formats (e.g., "UTF-8", "GBK").</summary>
    public string? EncodingName { get; init; }
}

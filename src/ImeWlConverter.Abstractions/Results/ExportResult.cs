namespace ImeWlConverter.Abstractions.Results;

/// <summary>Result of an export operation.</summary>
public sealed record ExportResult
{
    /// <summary>Number of entries successfully exported.</summary>
    public int EntryCount { get; init; }

    /// <summary>Number of entries that failed to export.</summary>
    public int ErrorCount { get; init; }
}

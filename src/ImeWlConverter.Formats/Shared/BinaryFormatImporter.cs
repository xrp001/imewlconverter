namespace ImeWlConverter.Formats.Shared;

using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>
/// Base class for binary format importers (e.g., .scel, .bdict).
/// </summary>
public abstract class BinaryFormatImporter : IFormatImporter
{
    public abstract FormatMetadata Metadata { get; }

    /// <summary>Parse word entries from a binary stream.</summary>
    protected abstract IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct);

    public Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, CancellationToken ct = default)
    {
        var entries = ParseBinary(input, ct);
        return Task.FromResult(new ImportResult
        {
            Entries = entries,
            ErrorCount = 0
        });
    }
}

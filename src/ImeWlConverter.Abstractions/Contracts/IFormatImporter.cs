using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

namespace ImeWlConverter.Abstractions.Contracts;

/// <summary>
/// Contract for importing word entries from an IME dictionary format.
/// Each implementation handles one specific format.
/// </summary>
public interface IFormatImporter
{
    /// <summary>Metadata describing this format.</summary>
    FormatMetadata Metadata { get; }

    /// <summary>Import word entries from a stream.</summary>
    Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, CancellationToken ct = default);
}

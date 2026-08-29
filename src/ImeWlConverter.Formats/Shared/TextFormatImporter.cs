namespace ImeWlConverter.Formats.Shared;

using System.Runtime.CompilerServices;
using System.Text;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>
/// Base class for text-based format importers.
/// Handles file reading, encoding detection, and line-by-line parsing.
/// </summary>
public abstract class TextFormatImporter : IFormatImporter
{
    /// <summary>The text encoding for this format.</summary>
    protected abstract Encoding FileEncoding { get; }

    public abstract FormatMetadata Metadata { get; }

    /// <summary>Parse a single line into zero or more word entries.</summary>
    protected abstract IEnumerable<WordEntry> ParseLine(string line);

    /// <summary>Whether a line should be processed (override to skip comments/headers).</summary>
    protected virtual bool IsContentLine(string line) => !string.IsNullOrWhiteSpace(line);

    public async Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, CancellationToken ct = default)
    {
        var entries = new List<WordEntry>();
        var errors = new List<string>();

        using var reader = new StreamReader(input, FileEncoding);
        string? line;
        var lineNumber = 0;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            ct.ThrowIfCancellationRequested();
            lineNumber++;

            if (!IsContentLine(line))
                continue;

            try
            {
                foreach (var entry in ParseLine(line))
                    entries.Add(entry);
            }
            catch (Exception ex)
            {
                errors.Add($"Line {lineNumber}: {ex.Message}");
            }
        }

        return new ImportResult
        {
            Entries = entries,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }

    /// <summary>Stream word entries one at a time for large files.</summary>
    public async IAsyncEnumerable<WordEntry> ImportStreamingAsync(
        Stream input,
        ImportOptions? options = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var reader = new StreamReader(input, FileEncoding);
        string? line;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            ct.ThrowIfCancellationRequested();
            if (!IsContentLine(line))
                continue;

            WordEntry[]? entries = null;
            try
            {
                entries = ParseLine(line).ToArray();
            }
            catch
            {
                // Skip parse errors in streaming mode
            }

            if (entries != null)
            {
                foreach (var entry in entries)
                    yield return entry;
            }
        }
    }
}

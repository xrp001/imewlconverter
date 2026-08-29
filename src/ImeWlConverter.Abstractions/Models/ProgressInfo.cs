namespace ImeWlConverter.Abstractions.Models;

/// <summary>
/// Progress information for long-running operations.
/// </summary>
/// <param name="Current">Current progress value.</param>
/// <param name="Total">Total expected value.</param>
/// <param name="Message">Optional progress message.</param>
public sealed record ProgressInfo(int Current, int Total, string? Message = null)
{
    /// <summary>Progress percentage (0-100).</summary>
    public double Percentage => Total > 0 ? (double)Current / Total * 100 : 0;
}

namespace ImeWlConverter.Abstractions.Contracts;

/// <summary>
/// Converts between Simplified and Traditional Chinese characters.
/// </summary>
public interface IChineseConverter
{
    /// <summary>Convert simplified Chinese to traditional.</summary>
    string ToTraditional(string simplified);

    /// <summary>Convert traditional Chinese to simplified.</summary>
    string ToSimplified(string traditional);
}

using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Abstractions.Contracts;

/// <summary>
/// Generates encoding/codes for Chinese words (pinyin, wubi, etc.).
/// </summary>
public interface ICodeGenerator
{
    /// <summary>The code type this generator produces.</summary>
    CodeType SupportedType { get; }

    /// <summary>Whether each character maps to exactly one code.</summary>
    bool Is1Char1Code { get; }

    /// <summary>Generate the code for a word string.</summary>
    WordCode GenerateCode(string word);
}

using System.Globalization;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class FirstCJKFilter : IWordFilter
{
    public bool ShouldKeep(WordEntry entry)
    {
        if (string.IsNullOrEmpty(entry.Word))
            return false;

        var si = new StringInfo(entry.Word);
        if (si.LengthInTextElements == 0)
            return false;

        var firstElement = si.SubstringByTextElements(0, 1);

        // Handle surrogate pairs (characters beyond BMP, like CJK Extension B-F)
        if (firstElement.Length == 2 && char.IsSurrogatePair(firstElement, 0))
        {
            var codePoint = char.ConvertToUtf32(firstElement, 0);
            return codePoint >= 0x20000 && codePoint <= 0x2FFFF;
        }

        var c = firstElement[0];
        return c >= 0x2E80 && c <= 0x9FFF;
    }
}

using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class NoAlphabetCodeFilter : IWordFilter
{
    public bool ShouldKeep(WordEntry entry)
    {
        if (entry.Code is null)
            return true;

        foreach (var segment in entry.Code.Segments)
            foreach (var code in segment)
                foreach (var c in code)
                    if (c < 'a' || c > 'z')
                        return false;

        return true;
    }
}

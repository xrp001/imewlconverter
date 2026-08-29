using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class EmojiReplacer : IWordTransform
{
    private readonly Dictionary<string, string> _mapping;

    // 映射表通过构造函数注入
    public EmojiReplacer(Dictionary<string, string> mapping)
    {
        _mapping = mapping;
    }

    public WordEntry? Transform(WordEntry entry)
    {
        return _mapping.TryGetValue(entry.Word, out var emoji)
            ? entry with { Word = emoji }
            : entry;
    }
}

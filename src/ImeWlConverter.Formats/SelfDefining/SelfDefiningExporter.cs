namespace ImeWlConverter.Formats.SelfDefining;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Self-defining format exporter with configurable field order and separators.</summary>
[FormatPlugin("self", "自定义", 2000)]
public sealed partial class SelfDefiningExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => new UTF8Encoding(false);

    protected override string LineEnding => "\n";

    /// <summary>Field order spec: '1'=pinyin, '2'=word, '3'=rank. Default "213" = word,pinyin,rank.</summary>
    public string OrderSpec { get; set; } = "213";

    /// <summary>Separator between pinyin syllables. Default space.</summary>
    public char PinyinSeparator { get; set; } = ' ';

    /// <summary>Separator between fields. Default comma.</summary>
    public char FieldSeparator { get; set; } = ',';

    /// <summary>Whether to include pinyin in the output.</summary>
    public bool ShowPinyin { get; set; } = true;

    /// <summary>Whether to include word in the output.</summary>
    public bool ShowWord { get; set; } = true;

    /// <summary>Whether to include rank in the output.</summary>
    public bool ShowRank { get; set; } = true;

    protected override string? FormatEntry(WordEntry entry)
    {
        var parts = new List<string>();

        foreach (var c in OrderSpec)
        {
            switch (c)
            {
                case '1': // pinyin
                    if (ShowPinyin)
                    {
                        var code = entry.Code?.GetPrimaryCode(PinyinSeparator.ToString()) ?? "";
                        parts.Add(code);
                    }

                    break;
                case '2': // word
                    if (ShowWord)
                        parts.Add(entry.Word);
                    break;
                case '3': // rank
                    if (ShowRank)
                        parts.Add(entry.Rank.ToString());
                    break;
            }
        }

        return parts.Count > 0 ? string.Join(FieldSeparator.ToString(), parts) : null;
    }
}

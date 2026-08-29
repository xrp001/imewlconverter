namespace ImeWlConverter.Formats.SelfDefining;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Self-defining format importer with configurable field order and separators.</summary>
[FormatPlugin("self", "自定义", 2000)]
public sealed partial class SelfDefiningImporter : BinaryFormatImporter
{
    /// <summary>Field order spec: '1'=pinyin, '2'=word, '3'=rank. Default "213" = word,pinyin,rank.</summary>
    public string OrderSpec { get; set; } = "213";

    /// <summary>Separator between pinyin syllables. Default single-quote.</summary>
    public char PinyinSeparator { get; set; } = '\'';

    /// <summary>Separator between fields. Default comma.</summary>
    public char FieldSeparator { get; set; } = ',';

    /// <summary>Whether pinyin is present in the data.</summary>
    public bool ShowPinyin { get; set; } = true;

    /// <summary>Whether word is present in the data.</summary>
    public bool ShowWord { get; set; } = true;

    /// <summary>Whether rank is present in the data.</summary>
    public bool ShowRank { get; set; } = true;

    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        using var reader = new StreamReader(input, Encoding.UTF8);
        var entries = new List<WordEntry>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(FieldSeparator);

            string? pinyin = null;
            string? word = null;
            int rank = 0;

            // Map parts to fields based on OrderSpec
            // OrderSpec characters indicate what field is at each position
            // Only visible fields are present in the data
            var visibleFields = BuildVisibleFieldOrder();
            for (var i = 0; i < visibleFields.Count && i < parts.Length; i++)
            {
                switch (visibleFields[i])
                {
                    case '1': // pinyin
                        pinyin = parts[i].Trim();
                        break;
                    case '2': // word
                        word = parts[i].Trim();
                        break;
                    case '3': // rank
                        if (int.TryParse(parts[i].Trim(), out var r))
                            rank = r;
                        break;
                }
            }

            if (string.IsNullOrEmpty(word)) continue;

            var code = !string.IsNullOrEmpty(pinyin)
                ? WordCode.FromSingle(pinyin.Split(PinyinSeparator))
                : null;

            entries.Add(new WordEntry
            {
                Word = word,
                Rank = rank,
                CodeType = CodeType.Pinyin,
                Code = code
            });
        }

        return entries;
    }

    private List<char> BuildVisibleFieldOrder()
    {
        var result = new List<char>();
        foreach (var c in OrderSpec)
        {
            switch (c)
            {
                case '1' when ShowPinyin:
                case '2' when ShowWord:
                case '3' when ShowRank:
                    result.Add(c);
                    break;
            }
        }

        return result;
    }
}

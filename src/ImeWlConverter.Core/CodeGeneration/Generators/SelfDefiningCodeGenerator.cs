using System.Diagnostics;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Core.Helpers;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// SelfDefiningCodeGenerator 自定义编码生成器，根据用户提供的外部码表和组词规则生成编码。
/// </summary>
public sealed class SelfDefiningCodeGenerator : ICodeGenerator
{
    /// <summary>
    /// 外部的编码表。Key 为汉字，Value 为该字的所有编码。
    /// </summary>
    public IDictionary<char, IList<string>> MappingDictionary { get; set; } = new Dictionary<char, IList<string>>();

    /// <summary>
    /// 对于多个字的编码的设定。
    /// 形如：
    /// code_e2=p11+p12+p21+p22
    /// code_e3=p11+p21+p31+p32
    /// code_a4=p11+p21+p31+n11
    /// </summary>
    public string MutiWordCodeFormat { get; set; } = "";

    public CodeType SupportedType => CodeType.UserDefine;

    /// <summary>
    /// 如果是拼音格式，那么就是一字一码，如果不是，那么就是一词一码。
    /// </summary>
    public bool Is1Char1Code { get; set; }

    public WordCode GenerateCode(string word)
    {
        if (string.IsNullOrEmpty(word))
            return new WordCode { Segments = [] };

        try
        {
            if (Is1Char1Code)
            {
                // 一字一码模式：每个字独立取编码
                var segments = new List<IReadOnlyList<string>>();
                foreach (var c in word)
                {
                    if (MappingDictionary.TryGetValue(c, out var codes))
                        segments.Add(codes.ToArray());
                    else
                        segments.Add([]);
                }

                return new WordCode { Segments = segments };
            }

            // 一词一码模式：按组词规则生成
            var result = GetWordCode(word);
            if (string.IsNullOrEmpty(result))
                return new WordCode { Segments = [] };

            return new WordCode
            {
                Segments = [new[] { result }]
            };
        }
        catch
        {
            return new WordCode { Segments = [] };
        }
    }

    private string? GetDefaultCodeOfChar(char c)
    {
        if (MappingDictionary.TryGetValue(c, out var codes) && codes.Count > 0)
            return codes[0];
        return null;
    }

    private string GetWordCode(string word)
    {
        var format = ParseFormat();

        var key = "e" + word.Length;
        if (format.TryGetValue(key, out var f))
            return GetStringCode(word, f);

        // 字符串很长，找最大的 a{n} 格式
        for (var i = word.Length; i > 0; i--)
        {
            key = "a" + i;
            if (format.TryGetValue(key, out f))
                return GetStringCode(word, f);
        }

        return "";
    }

    private Dictionary<string, string> ParseFormat()
    {
        var format = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(MutiWordCodeFormat))
            return format;

        var arr = MutiWordCodeFormat.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in arr)
        {
            var kv = line.Split('=');
            if (kv.Length < 2) continue;

            // code_e2=p11+p12+p21+p22 → key="e2", value="p11+p12+p21+p22"
            var keyPart = kv[0];
            var value = kv[1];

            if (keyPart.StartsWith("code_"))
                keyPart = keyPart[5..];

            format.TryAdd(keyPart, value);
        }

        return format;
    }

    private string GetStringCode(string word, string formatStr)
    {
        var result = "";
        var flist = formatStr.Split('+');

        foreach (var s in flist)
        {
            if (s.Length < 3) continue;

            var pn = s[0]; // p=左取(正序), n=右取(倒序)
            var pindex = s[1] - '0';
            char c;

            if (pn == 'p')
                c = word[pindex - 1];
            else if (pn == 'n')
                c = word[word.Length - pindex];
            else
                continue;

            var pcode = GetDefaultCodeOfChar(c);
            if (pcode == null) continue;

            var cindex = s[2] - '0';
            if (pcode.Length >= cindex)
                result += pcode[cindex - 1];
            else
                Debug.WriteLine($"{word} 编码生成错误");
        }

        return result;
    }
}

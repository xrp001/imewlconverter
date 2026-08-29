using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Core.Helpers;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// PinyinCodeGenerator 拼音编码生成器，使用多音字词组注音和贪婪匹配算法。
/// </summary>
public sealed class PinyinCodeGenerator : ICodeGenerator
{
    private static Dictionary<string, List<string>>? mutiPinYinWord;

    public CodeType SupportedType => CodeType.Pinyin;

    public bool Is1Char1Code => true;

    public WordCode GenerateCode(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return new WordCode { Segments = Array.Empty<IReadOnlyList<string>>() };
        }

        var pinyinList = IsInWordPinYin(word)
            ? GenerateMutiWordPinYin(word)
            : null;

        var segments = new List<IReadOnlyList<string>>(word.Length);
        for (var i = 0; i < word.Length; i++)
        {
            string py;
            if (pinyinList != null && pinyinList[i] != null)
            {
                py = pinyinList[i]!;
            }
            else
            {
                try
                {
                    py = PinyinHelper.GetDefaultPinyin(word[i]);
                }
                catch
                {
                    py = "";
                }
            }

            segments.Add(new[] { py });
        }

        return new WordCode { Segments = segments };
    }

    private static void InitMutiPinYinWord()
    {
        if (mutiPinYinWord == null)
        {
            var wlList = new Dictionary<string, List<string>>();
            var lines = DictionaryHelper.GetResourceContent("WordPinyin.txt")
                .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Split(' ');
                if (line.Length < 2) continue;

                var py = line[0];
                var wordText = line[1];

                var pinyin = new List<string>(
                    py.Split(new[] { '\'' }, StringSplitOptions.RemoveEmptyEntries)
                );
                wlList.TryAdd(wordText, pinyin);
            }

            mutiPinYinWord = wlList;
        }
    }

    private static bool IsInWordPinYin(string word)
    {
        InitMutiPinYinWord();
        foreach (var key in mutiPinYinWord!.Keys)
        {
            if (word.Contains(key))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 贪婪匹配算法生成多音字词组拼音，优先匹配较长的词组，避免重复标注。
    /// </summary>
    private static List<string?> GenerateMutiWordPinYin(string word)
    {
        InitMutiPinYinWord();
        var pinyin = new string?[word.Length];
        var matched = new bool[word.Length];

        var sortedKeys = mutiPinYinWord!.Keys.OrderByDescending(k => k.Length).ToList();

        foreach (var key in sortedKeys)
        {
            var index = 0;
            while ((index = word.IndexOf(key, index, StringComparison.Ordinal)) != -1)
            {
                var canMatch = true;
                for (var i = 0; i < key.Length; i++)
                {
                    if (matched[index + i])
                    {
                        canMatch = false;
                        break;
                    }
                }

                if (canMatch)
                {
                    var pinyinValues = mutiPinYinWord[key];
                    for (var i = 0; i < pinyinValues.Count; i++)
                    {
                        pinyin[index + i] = pinyinValues[i];
                        matched[index + i] = true;
                    }
                }

                index++;
            }
        }

        return new List<string?>(pinyin);
    }
}

using System.Diagnostics;
using System.Globalization;

namespace ImeWlConverter.Core.Helpers;

public static class PinyinHelper
{
    /// <summary>
    /// 获得一个字的默认拼音(不包含音调)
    /// </summary>
    public static string GetDefaultPinyin(char c)
    {
        try
        {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                return c.ToString().ToLower();
            }

            if (c >= '0' && c <= '9')
            {
                return c.ToString();
            }

            var pys = PinYinDict[c];
            if (pys != null && pys.Count > 0) return pys[0];
            throw new Exception($"找不到字:'{c}'的拼音");
        }
        catch
        {
            throw new Exception($"找不到字:'{c}'的拼音");
        }
    }

    public static IList<string> GetDefaultPinyin(string word)
    {
        var result = new List<string>();
        var si = new StringInfo(word);
        for (int i = 0; i < si.LengthInTextElements; i++)
        {
            var textElement = si.SubstringByTextElements(i, 1);
            if (textElement.Length == 1)
            {
                result.Add(GetDefaultPinyin(textElement[0]));
            }
            else
            {
                Debug.WriteLine($"Skipping character beyond BMP: {textElement}");
            }
        }
        return result;
    }

    /// <summary>
    /// 获得单个字的拼音,不包括声调
    /// </summary>
    public static IList<string> GetPinYinOfChar(char str)
    {
        return PinYinDict[str];
    }

    /// <summary>
    /// 判断一个字是否多音字
    /// </summary>
    public static bool IsMultiPinyinWord(char c)
    {
        return GetPinYinOfChar(c).Count > 1;
    }

    /// <summary>
    /// 获得单个字的拼音,包括声调
    /// </summary>
    public static List<string> GetPinYinWithToneOfChar(char str)
    {
        return PinYinWithToneDict[str];
    }

    /// <summary>
    /// 如果给出一个字和一个没有音调的拼音，返回正确的带音调的拼音
    /// </summary>
    public static string AddToneToPinyin(char str, string py)
    {
        if (!PinYinWithToneDict.ContainsKey(str))
        {
            Debug.WriteLine("找不到" + str + "的拼音,使用其默认拼音对应的音调1");
            return py + "1";
        }

        var list = PinYinWithToneDict[str];
        foreach (var allpinyin in list)
            foreach (var pinyin in allpinyin.Split(','))
                if (
                    pinyin == py + "0"
                    || pinyin == py + "1"
                    || pinyin == py + "2"
                    || pinyin == py + "3"
                    || pinyin == py + "4"
                    || pinyin == py + "5"
                )
                    return pinyin;

        Debug.WriteLine("找不到" + str + "的拼音" + py + "对应的音调");
        return py + "1";
    }

    /// <summary>
    /// 判断给出的词和拼音是否有效
    /// </summary>
    public static bool ValidatePinyin(string word, List<string> pinyin)
    {
        var pinyinList = pinyin;
        if (word.Length != pinyinList.Count) return false;
        for (var i = 0; i < word.Length; i++)
        {
            var charPinyinList = GetPinYinOfChar(word[i]);
            if (!charPinyinList.Contains(pinyinList[i])) return false;
        }

        return true;
    }

    #region Init

    private static readonly Dictionary<char, List<string>> dictionary = new();
    private static readonly Dictionary<char, IList<string>> pyDictionary = new();

    /// <summary>
    /// 字的拼音(包括音调)
    /// </summary>
    private static Dictionary<char, List<string>> PinYinWithToneDict
    {
        get
        {
            if (dictionary.Count == 0)
            {
                var pyList = DictionaryHelper.GetAll();

                foreach (var code in pyList)
                {
                    var hz = code.Word;
                    var py = code.Pinyins;
                    if (!string.IsNullOrEmpty(py)) dictionary.Add(hz, new List<string>(py.Split(';')));
                }
            }

            return dictionary;
        }
    }

    /// <summary>
    /// 字的拼音，不包括音调
    /// </summary>
    public static Dictionary<char, IList<string>> PinYinDict
    {
        get
        {
            if (pyDictionary.Count == 0)
            {
                var pyList = DictionaryHelper.GetAll();

                foreach (var code in pyList)
                {
                    var hz = code.Word;
                    var pys = code.Pinyins;
                    if (!string.IsNullOrEmpty(pys))
                        foreach (var s in pys.Split(','))
                        {
                            var py = s.Remove(s.Length - 1); //remove tone
                            if (pyDictionary.ContainsKey(hz))
                            {
                                if (!pyDictionary[hz].Contains(py)) pyDictionary[hz].Add(py);
                            }
                            else
                            {
                                pyDictionary.Add(hz, new List<string> { py });
                            }
                        }
                }
            }

            return pyDictionary;
        }
    }

    #endregion
}

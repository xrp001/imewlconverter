using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ImeWlConverter.Core.Helpers;

/// <summary>
/// 根据拼音，超音速写输入法编码生成
/// </summary>
public static class ChaoyinHelper
{
    private static readonly Regex regex = new(@"^[a-zA-Z]+\d$");

    private static readonly IList<string> ShenmuY = new List<string>();

    private static IDictionary<string, string>? pinyinCodeMapping;

    private static IDictionary<string, string> PinyinCodeMapping
    {
        get
        {
            if (pinyinCodeMapping == null)
            {
                pinyinCodeMapping = new Dictionary<string, string>();
                var lines = DictionaryHelper
                    .GetResourceContent("ChaoyinCodeMapping.txt")
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var array = line.Split('\t');
                    pinyinCodeMapping.Add(array[0], array[1]);
                    if (array[2] == "Y") ShenmuY.Add(array[0]);
                }
            }

            return pinyinCodeMapping;
        }
    }

    /// <summary>
    /// 获得一个拼音对应的超音编码
    /// </summary>
    public static string? GetChaoyin(string pinyin)
    {
        if (string.IsNullOrEmpty(pinyin)) throw new Exception("找不到拼音");
        var yindiao = 10;
        if (regex.IsMatch(pinyin))
        {
            yindiao = Convert.ToInt32(pinyin[pinyin.Length - 1].ToString());
            pinyin = pinyin.Substring(0, pinyin.Length - 1);
        }

        if (!PinyinCodeMapping.ContainsKey(pinyin))
        {
            Debug.WriteLine("Can not find Chaoyin code by pinyin=" + pinyin);
            return null;
        }

        var zy = PinyinCodeMapping[pinyin];
        Debug.WriteLine("Pinyin:" + pinyin + ",Chaoyin:" + zy);
        return zy;
    }

    /// <summary>
    /// 获得一个词语的超音编码
    /// </summary>
    public static string GetChaoyin(IList<string> pinyins)
    {
        var result = new StringBuilder();
        if (pinyins.Count == 1) return GetChaoyin(pinyins[0]) ?? "";

        if (pinyins.Count == 2)
        {
            result.Append(PinyinCodeMapping[pinyins[0]]);
            result.Append(PinyinCodeMapping[pinyins[1]]);
            if (ShenmuY.Contains(pinyins[1])) result.Append(";");
        }
        else if (pinyins.Count == 3)
        {
            result.Append(PinyinCodeMapping[pinyins[0]][0]);
            result.Append(PinyinCodeMapping[pinyins[1]][0]);
            result.Append(PinyinCodeMapping[pinyins[2]]);
            if (ShenmuY.Contains(pinyins[2]))
                result.Append("'");
            else
                result.Append(";");
        }
        else if (pinyins.Count == 4)
        {
            result.Append(PinyinCodeMapping[pinyins[0]][0]);
            result.Append(PinyinCodeMapping[pinyins[1]][0]);
            result.Append(PinyinCodeMapping[pinyins[2]][0]);
            if (ShenmuY.Contains(pinyins[3]))
            {
                result.Append(PinyinCodeMapping[pinyins[3]][0]);
                result.Append(PinyinCodeMapping[pinyins[3]][0]);
            }
            else
            {
                result.Append(PinyinCodeMapping[pinyins[3]]);
            }
        }
        else if (pinyins.Count == 5)
        {
            result.Append(PinyinCodeMapping[pinyins[0]][0]);
            result.Append(PinyinCodeMapping[pinyins[1]][0]);
            result.Append(PinyinCodeMapping[pinyins[2]][0]);
            result.Append(PinyinCodeMapping[pinyins[3]][0]);
            result.Append(PinyinCodeMapping[pinyins[4]][0]);
        }
        else
        {
            result.Append(PinyinCodeMapping[pinyins[0]][0]);
            result.Append(PinyinCodeMapping[pinyins[1]][0]);
            result.Append(PinyinCodeMapping[pinyins[2]][0]);
            result.Append(PinyinCodeMapping[pinyins[3]][0]);
            result.Append(PinyinCodeMapping[pinyins[pinyins.Count - 1]][0]);
        }

        return result.ToString();
    }
}

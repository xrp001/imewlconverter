using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ImeWlConverter.Core.Helpers;

public static class ZhuyinHelper
{
    private static readonly Regex regex = new(@"^[a-zA-Z]+\d$");
    private static IDictionary<string, string>? zhuyinDic;
    private static IDictionary<string, string>? pinyinDic;

    private static IDictionary<string, string> ZhuyinDic
    {
        get
        {
            if (zhuyinDic == null)
            {
                zhuyinDic = new Dictionary<string, string>();
                foreach (
                    var line in DictionaryHelper.GetResourceContent("Zhuyin.txt")
                        .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                )
                {
                    var arr = line.Split('\t');

                    var zhuyinCode = arr[0];
                    var pinyin = arr[1];

                    if (!zhuyinDic.ContainsKey(pinyin))
                        zhuyinDic.Add(pinyin, zhuyinCode);
                    else
                        Debug.WriteLine(pinyin + " mapping more than 1 zhuyin");
                }
            }

            return zhuyinDic;
        }
    }

    private static IDictionary<string, string> PinyinDic
    {
        get
        {
            if (pinyinDic == null)
            {
                pinyinDic = new Dictionary<string, string>();
                foreach (
                    var line in DictionaryHelper.GetResourceContent("Zhuyin.txt")
                        .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                )
                {
                    var arr = line.Split('\t');

                    var zhuyinCode = arr[0];
                    var pinyin = arr[1];

                    if (!pinyinDic.ContainsKey(zhuyinCode))
                        pinyinDic.Add(zhuyinCode, pinyin);
                    else
                        Debug.WriteLine(pinyin + " mapping more than 1 pinyin");
                }
            }

            return pinyinDic;
        }
    }

    public static string? GetZhuyin(string pinyin)
    {
        if (string.IsNullOrEmpty(pinyin)) throw new Exception("找不到拼音");
        var yindiao = 10;
        if (regex.IsMatch(pinyin))
        {
            yindiao = Convert.ToInt32(pinyin[pinyin.Length - 1].ToString());
            pinyin = pinyin.Substring(0, pinyin.Length - 1);
        }

        if (!ZhuyinDic.ContainsKey(pinyin))
        {
            Debug.WriteLine("Can not find zhuyin by pinyin=" + pinyin);
            return null;
        }

        var zy = ZhuyinDic[pinyin] + GetYindiaoZhuyin(yindiao);
        Debug.WriteLine("Pinyin:" + pinyin + ",Zhuyin:" + zy);
        return zy;
    }

    public static IList<string?> GetZhuyin(IList<string> pinyins)
    {
        var result = new List<string?>();
        foreach (var code in pinyins) result.Add(GetZhuyin(code));
        return result;
    }

    private static string GetYindiaoZhuyin(int yindiao)
    {
        return yindiao switch
        {
            1 => "",
            2 => "ˊ",
            3 => "ˇ",
            4 => "ˋ",
            5 => "·",
            _ => ""
        };
    }

    private static int GetYindiaoPinyin(char yindiao)
    {
        return yindiao switch
        {
            'ˊ' => 2,
            'ˇ' => 3,
            'ˋ' => 4,
            '·' => 5,
            _ => 1
        };
    }

    /// <summary>
    /// 根据注音获得不包含音调的拼音
    /// </summary>
    public static string? GetPinyin(string zhuyin)
    {
        var lastChar = zhuyin[zhuyin.Length - 1];
        var yindiao = GetYindiaoPinyin(lastChar);
        if (yindiao != 1) zhuyin = zhuyin.Substring(0, zhuyin.Length - 1);
        if (PinyinDic.ContainsKey(zhuyin)) return PinyinDic[zhuyin];
        Debug.WriteLine("can not fine the pinyin of zhuyin:" + zhuyin);
        return null;
    }
}

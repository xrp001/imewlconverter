using System.Reflection;

namespace ImeWlConverter.Core.Helpers;

public static class DictionaryHelper
{
    private static readonly Dictionary<char, ChineseCode> dictionary = new();

    private static Dictionary<char, ChineseCode> Dict
    {
        get
        {
            if (dictionary.Count == 0)
            {
                var allPinYin = GetResourceContent("ChineseCode.txt");
                var pyList = allPinYin.Split(
                    new[] { "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries
                );
                for (var i = 0; i < pyList.Length; i++)
                {
                    var hzpy = pyList[i].Split('\t');
                    var hz = Convert.ToChar(hzpy[1]);

                    dictionary.Add(
                        hz,
                        new ChineseCode(
                            Code: hzpy[0],
                            Word: hzpy[1][0],
                            Wubi86: hzpy[2],
                            Wubi98: hzpy[3],
                            WubiNewAge: hzpy[4],
                            Pinyins: hzpy[5],
                            Freq: Convert.ToDouble(hzpy[6])
                        )
                    );
                }
            }

            return dictionary;
        }
    }

    public static ChineseCode GetCode(char c)
    {
        if (Dict.TryGetValue(c, out var code))
            return code;
        throw new Exception("给定关键字不在字典中，【" + c + "】");
    }

    public static List<ChineseCode> GetAll()
    {
        return new List<ChineseCode>(Dict.Values);
    }

    public static string GetResourceContent(string fileName)
    {
        var assembly = typeof(DictionaryHelper).Assembly;

        using var stream = assembly.GetManifestResourceStream(
            "ImeWlConverter.Core.Resources." + fileName
        );
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource not found: {fileName}");

        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }
}

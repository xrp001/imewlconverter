using System.Text;

namespace ImeWlConverter.Core.Helpers;

public static class UserCodingHelper
{
    public static IDictionary<char, IList<string>> GetCodingDict(
        string filePath,
        Encoding encoding
    )
    {
        var codingContent = FileOperationHelper.ReadFile(filePath, encoding);
        var dic = new Dictionary<char, IList<string>>();
        foreach (
            var line in codingContent.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            )
        )
        {
            var l = line.Split('\t');
            if (l.Length != 2) throw new Exception("无效的自定义编码格式：" + line);
            var c = l[0][0];
            var code = l[1];
            if (!dic.ContainsKey(c))
                dic.Add(c, new List<string> { code });
            else
                dic[c].Add(code);
        }

        return dic;
    }
}

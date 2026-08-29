using System.Text;

namespace ImeWlConverter.Core.Helpers;

public static class CollectionHelper
{
    public static string GetString(IEnumerable<string>? list, string split, BuildType buildType)
    {
        var sb = new StringBuilder();

        if (list == null) return "";

        foreach (var s in list) sb.Append(s + split);
        if (sb.Length == 0) return "";
        if (buildType == BuildType.RightContain) return sb.ToString();
        if (buildType == BuildType.FullContain) return split + sb;
        var str = sb.ToString();
        if (split.Length > 0) str = str.Remove(sb.Length - 1);
        if (buildType == BuildType.None) return str;
        return split + str;
    }

    /// <summary>
    /// Cartesian product (recursive)
    /// </summary>
    private static string Descartes(
        IList<IList<string>> list,
        int count,
        IList<string> result,
        string data
    )
    {
        var temp = data;
        var astr = list[count];
        foreach (var item in astr)
            if (count + 1 < list.Count)
                temp += Descartes(list, count + 1, result, data + item);
            else
                result.Add(data + item);

        return temp;
    }

    /// <summary>
    /// 多音字情况下，做笛卡尔积
    /// </summary>
    public static IList<string> Descartes(IList<IList<string>> codes)
    {
        var result = new List<string>();
        Descartes(codes, 0, result, string.Empty);
        return result;
    }

    /// <summary>
    /// 只取每个字的第一个编码，返回这些编码的List
    /// </summary>
    public static IList<string> DescarteIndex1(IList<IList<string>> codes)
    {
        var result = new List<string>();
        foreach (var code in codes) result.Add(code[0]);
        return result;
    }

    public static IList<string> CartesianProduct(IList<IList<string>> codes, string split)
    {
        var count = 1;
        foreach (var code in codes) count *= code.Count;
        var result = new List<string>();
        for (var i = 0; i < count; i++)
        {
            var line = new string[codes.Count];
            for (var j = 0; j < codes.Count; j++) line[j] = codes[j][i % codes[j].Count];
            result.Add(string.Join(split, line));
        }

        return result;
    }

    public static IList<string> CartesianProduct(
        IList<IList<string>> codes,
        string split,
        BuildType buildType
    )
    {
        var list = CartesianProduct(codes, split);
        if (buildType == BuildType.None)
            return list;
        var result = new List<string>();
        foreach (var line in list)
        {
            var newline = line;
            if (buildType == BuildType.FullContain || buildType == BuildType.LeftContain) newline = split + newline;
            if (buildType == BuildType.FullContain || buildType == BuildType.RightContain) newline = newline + split;
            result.Add(newline);
        }

        return result;
    }
}

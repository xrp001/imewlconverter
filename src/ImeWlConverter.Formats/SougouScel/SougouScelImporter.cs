namespace ImeWlConverter.Formats.SougouScel;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Sougou Pinyin scel cell dictionary importer (binary).</summary>
[FormatPlugin("scel", "搜狗细胞词库scel", 20)]
public sealed partial class SougouScelImporter : BinaryFormatImporter
{
    private Dictionary<int, string> _pyDic = new();

    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        using var fs = new MemoryStream();
        input.CopyTo(fs);
        fs.Position = 0;

        return ReadScel(fs);
    }

    private IReadOnlyList<WordEntry> ReadScel(MemoryStream fs)
    {
        _pyDic = new Dictionary<int, string>();
        var result = new List<WordEntry>();

        // 未展开的词条数（同音词算1个）
        fs.Position = 0x120;
        var dictLen = ReadInt32(fs);

        // 拼音表的长度
        fs.Position = 0x1540;
        var pyDicLen = ReadInt32(fs);

        for (var i = 0; i < pyDicLen; i++)
        {
            var idx = ReadInt16(fs);
            var size = ReadInt16(fs);
            var str = new byte[size];
            fs.ReadExactly(str, 0, size);
            var py = Encoding.Unicode.GetString(str);
            _pyDic.Add(idx, py);
        }

        for (var i = 0; i < dictLen; i++)
        {
            try
            {
                result.AddRange(ReadAPinyinWord(fs));
            }
            catch
            {
                // 跳过解析失败的词条
            }
        }

        return result;
    }

    private IList<WordEntry> ReadAPinyinWord(MemoryStream fs)
    {
        var num = new byte[4];
        fs.ReadExactly(num, 0, 4);
        var samePYcount = num[0] + num[1] * 256;
        var count = num[2] + num[3] * 256;

        // 接下来读拼音
        var str = new byte[256];
        for (var i = 0; i < count; i++)
            str[i] = (byte)fs.ReadByte();

        var wordPY = new List<string>();
        for (var i = 0; i < count / 2; i++)
        {
            var key = str[i * 2] + str[i * 2 + 1] * 256;
            if (key < _pyDic.Count)
                wordPY.Add(_pyDic[key]);
            else
                wordPY.Add(((char)(key - _pyDic.Count + 97)).ToString());
        }

        // 接下来读词语
        var pyAndWord = new List<WordEntry>();
        for (var s = 0; s < samePYcount; s++)
        {
            num = new byte[2];
            fs.ReadExactly(num, 0, 2);
            var hzBytecount = num[0] + num[1] * 256;
            str = new byte[hzBytecount];
            fs.ReadExactly(str, 0, hzBytecount);
            var word = Encoding.Unicode.GetString(str);

            // 跳过 unknown1 (2 bytes) + unknown2 (4 bytes)
            ReadInt16(fs);
            ReadInt32(fs);

            pyAndWord.Add(new WordEntry
            {
                Word = word,
                Rank = 0,
                CodeType = CodeType.Pinyin,
                Code = wordPY.Count > 0
                    ? WordCode.FromSingle(wordPY.ToArray())
                    : null
            });

            // 接下来6个字节跳过
            var temp = new byte[6];
            fs.ReadExactly(temp, 0, 6);
        }

        return pyAndWord;
    }

    #region 二进制读取辅助

    private static short ReadInt16(Stream fs)
    {
        var temp = new byte[2];
        fs.ReadExactly(temp, 0, 2);
        return BitConverter.ToInt16(temp, 0);
    }

    private static int ReadInt32(Stream fs)
    {
        var temp = new byte[4];
        fs.ReadExactly(temp, 0, 4);
        return BitConverter.ToInt32(temp, 0);
    }

    #endregion
}

namespace ImeWlConverter.Formats.LingoesLd2;

using System.IO.Compression;
using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Lingoes ld2 dictionary importer (binary).</summary>
[FormatPlugin("ld2", "灵格斯ld2", 200)]
public sealed partial class LingoesLd2Importer : BinaryFormatImporter
{
    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);
        var fileBytes = ms.ToArray();

        using var fs = new MemoryStream(fileBytes);
        var words = Parse(fs);
        if (words == null || words.Count == 0)
            return Array.Empty<WordEntry>();

        var result = new List<WordEntry>(words.Count);
        foreach (var word in words)
        {
            result.Add(new WordEntry
            {
                Word = word,
                Rank = 0,
                CodeType = CodeType.English,
                IsEnglish = true
            });
        }

        return result;
    }

    #region 解析入口

    private IList<string>? Parse(MemoryStream fs)
    {
        var bs = ReadArray(fs, 4);
        fs.Position = 0x5c;
        var offsetData = ReadInt32(fs) + 0x60;
        if (fs.Length > offsetData)
        {
            fs.Position = offsetData;
            var type = ReadInt32(fs);
            fs.Position = offsetData + 4;
            var offsetWithInfo = ReadInt32(fs) + offsetData + 12;
            if (type == 3)
                return ReadDictionary(fs, offsetData);
            if (fs.Length > offsetWithInfo - 0x1C)
                return ReadDictionary(fs, offsetWithInfo);
        }

        return null;
    }

    private IList<string> ReadDictionary(MemoryStream fs, int offsetWithIndex)
    {
        fs.Position = offsetWithIndex;
        var type = ReadInt32(fs);
        var limit = ReadInt32(fs) + offsetWithIndex + 8;
        var offsetIndex = offsetWithIndex + 0x1C;
        var offsetCompressedDataHeader = ReadInt32(fs) + offsetIndex;
        var inflatedWordsIndexLength = ReadInt32(fs);
        var inflatedWordsLength = ReadInt32(fs);
        var inflatedXmlLength = ReadInt32(fs);

        var deflateStreams = new List<int>();
        fs.Position = offsetCompressedDataHeader + 8;
        var offset = ReadInt32(fs);
        while (offset + fs.Position < limit)
        {
            offset = ReadInt32(fs);
            deflateStreams.Add(offset);
        }

        var offsetCompressedData = fs.Position;

        var inflatedFile = Inflate(fs, offsetCompressedData, deflateStreams);

        return Extract(
            inflatedFile,
            inflatedWordsIndexLength,
            inflatedWordsIndexLength + inflatedWordsLength
        );
    }

    #endregion

    #region 解压

    private static byte[] Inflate(MemoryStream dataRawBytes, long startP, List<int> deflateStreams)
    {
        var temp = new List<byte>();
        var startOffset = startP;
        var lastOffset = startOffset;
        try
        {
            foreach (var offsetRelative in deflateStreams)
            {
                var offset = startOffset + offsetRelative;
                temp.AddRange(Decompress(dataRawBytes, lastOffset, offset - lastOffset));
                lastOffset = offset;
            }
        }
        catch
        {
            // 解压缩失败时返回已解压的部分
        }

        return temp.ToArray();
    }

    private static List<byte> Decompress(MemoryStream data, long offset, long length)
    {
        var t = new List<byte>();
        data.Position = offset;
        var compressedBytes = new byte[length];
        data.ReadExactly(compressedBytes, 0, (int)length);

        using var compressedStream = new MemoryStream(compressedBytes);
        // Skip zlib header (2 bytes) if present
        var b1 = compressedStream.ReadByte();
        var b2 = compressedStream.ReadByte();
        if (b1 != 0x78)
        {
            // Not a zlib header, reset and try raw deflate
            compressedStream.Position = 0;
        }

        using var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress);
        var buffer = new byte[8192];
        int len;
        while ((len = deflateStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < len; i++)
                t.Add(buffer[i]);
        }

        return t;
    }

    #endregion

    #region 解析词条

    private static IList<string> Extract(byte[] dataRawBytes, int offsetDefs, int offsetXml)
    {
        var dataLen = 10;
        var defTotal = offsetDefs / dataLen - 1;
        var words = new string[defTotal];
        var wordEncoding = Encoding.UTF8;

        for (var i = 0; i < defTotal; i++)
        {
            var kv = ReadDefinitionData(
                dataRawBytes,
                offsetDefs,
                offsetXml,
                dataLen,
                wordEncoding,
                i
            );
            words[i] = kv.Key;
        }

        return new List<string>(words);
    }

    private static KeyValuePair<string, string> ReadDefinitionData(
        byte[] inflatedBytes,
        int offsetWords,
        int offsetXml,
        int dataLen,
        Encoding wordStringDecoder,
        int i)
    {
        var idxData = new int[6];
        GetIdxData(inflatedBytes, dataLen * i, idxData);
        var lastWordPos = idxData[0];
        var lastXmlPos = idxData[1];
        var flags = idxData[2];
        var refs = idxData[3];
        var currentWordOffset = idxData[4];
        var currenXmlOffset = idxData[5];

        var xmlEncoding = Encoding.UTF8;
        var xml = xmlEncoding.GetString(
            inflatedBytes,
            offsetXml + lastXmlPos,
            currenXmlOffset - lastXmlPos
        );
        while (refs-- > 0)
        {
            var position = offsetWords + lastWordPos;
            var ref1 = BitConverter.ToInt32(inflatedBytes, position);
            GetIdxData(inflatedBytes, dataLen * ref1, idxData);
            lastXmlPos = idxData[1];
            currenXmlOffset = idxData[5];
            if (string.IsNullOrEmpty(xml))
                xml = xmlEncoding.GetString(
                    inflatedBytes,
                    offsetXml + lastXmlPos,
                    currenXmlOffset - lastXmlPos
                );
            else
                xml = xmlEncoding.GetString(
                        inflatedBytes,
                        offsetXml + lastXmlPos,
                        currenXmlOffset - lastXmlPos
                    ) + ", " + xml;
            lastWordPos += 4;
        }

        var position1 = offsetWords + lastWordPos;
        var w = ReadArrayFromBytes(inflatedBytes, position1, currentWordOffset - lastWordPos);
        var word = wordStringDecoder.GetString(w);
        return new KeyValuePair<string, string>(word, xml);
    }

    private static void GetIdxData(byte[] dataRawBytes, int position, int[] wordIdxData)
    {
        wordIdxData[0] = BitConverter.ToInt32(dataRawBytes, position);
        wordIdxData[1] = BitConverter.ToInt32(dataRawBytes, position + 4);
        wordIdxData[2] = dataRawBytes[position + 8] & 0xff;
        wordIdxData[3] = dataRawBytes[position + 9] & 0xff;
        wordIdxData[4] = BitConverter.ToInt32(dataRawBytes, position + 10);
        wordIdxData[5] = BitConverter.ToInt32(dataRawBytes, position + 14);
    }

    #endregion

    #region 二进制读取辅助

    private static byte[] ReadArray(Stream fs, int count)
    {
        var bytes = new byte[count];
        fs.ReadExactly(bytes, 0, count);
        return bytes;
    }

    private static byte[] ReadArrayFromBytes(byte[] source, int position, int count)
    {
        var bytes = new byte[count];
        for (var i = 0; i < count; i++)
            bytes[i] = source[position + i];
        return bytes;
    }

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

    private static long ReadInt64(Stream fs)
    {
        var temp = new byte[8];
        fs.ReadExactly(temp, 0, 8);
        return BitConverter.ToInt64(temp, 0);
    }

    #endregion
}

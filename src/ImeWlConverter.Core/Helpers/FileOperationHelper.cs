using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using UtfUnknown;

namespace ImeWlConverter.Core.Helpers;

public static class FileOperationHelper
{
    public static string GetCurrentFolderPath()
    {
        return Path.GetDirectoryName(AppContext.BaseDirectory)!;
    }

    /// <summary>
    /// 自动判断文字编码，然后进行读取
    /// </summary>
    public static string ReadFile(string path)
    {
        if (!File.Exists(path)) return "";
        var c = GetEncodingType(path);
        return ReadFile(path, c);
    }

    public static string ReadFile(string path, Encoding encoding)
    {
        if (!File.Exists(path)) return "";
        using var sr = new StreamReader(path, encoding);
        return sr.ReadToEnd();
    }

    /// <summary>
    /// 获取文件的流式读取器,用于处理大文件避免内存溢出
    /// </summary>
    public static StreamReader? GetStreamReader(string path, Encoding encoding)
    {
        if (!File.Exists(path)) return null;
        return new StreamReader(path, encoding);
    }

    /// <summary>
    /// 检查文件是否应该使用流式处理(文件大于10MB)
    /// </summary>
    public static bool ShouldUseStreaming(string path)
    {
        if (!File.Exists(path)) return false;
        var fileInfo = new FileInfo(path);
        return fileInfo.Length > 10 * 1024 * 1024;
    }

    /// <summary>
    /// 将一个字符串写入文件，采用覆盖的方式
    /// </summary>
    public static bool WriteFile(string path, Encoding coding, string content)
    {
        try
        {
            using var sw = new StreamWriter(path, false, coding);
            sw.Write(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static StreamWriter WriteFile(string path, Encoding coding)
    {
        return new StreamWriter(path, false, coding);
    }

    /// <summary>
    /// 写一行文本到文件，追加的方式
    /// </summary>
    public static bool WriteFileLine(string path, string line)
    {
        try
        {
            using var sw = new StreamWriter(path, true);
            sw.WriteLine(line);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool WriteFileLine(StreamWriter sw, string line)
    {
        try
        {
            sw.WriteLine(line);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static StreamWriter GetWriteFileStream(string path, Encoding coding)
    {
        return new StreamWriter(path, false, coding);
    }

    public static Encoding GetEncodingType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("文件路径不能为空", nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"文件不存在: {fileName}");
        }

        try
        {
            var result = CharsetDetector.DetectFromFile(fileName);
            var resultDetected = result?.Detected;

            if (resultDetected == null || resultDetected.Confidence < 0.7)
            {
                try
                {
                    return Encoding.GetEncoding("GB18030");
                }
                catch
                {
                    return Encoding.GetEncoding("GB2312");
                }
            }

            return resultDetected.Encoding ?? Encoding.UTF8;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"检测文件编码失败: {fileName}, 错误: {ex.Message}");
            return Encoding.UTF8;
        }
    }

    public static void WriteFileHeader(FileStream fs, Encoding encoding)
    {
        if (encoding == Encoding.UTF8)
        {
            fs.WriteByte(0xEF);
            fs.WriteByte(0xBB);
            fs.WriteByte(0xBF);
        }
        else if (encoding == Encoding.Unicode)
        {
            fs.WriteByte(0xFF);
            fs.WriteByte(0xFE);
        }
        else if (encoding == Encoding.BigEndianUnicode)
        {
            fs.WriteByte(0xFE);
            fs.WriteByte(0xFF);
        }
    }

    /// <summary>
    /// 根据文本框输入的一个路径，返回文件列表
    /// </summary>
    public static IList<string> GetFilesPath(string input)
    {
        var result = new List<string>();
        foreach (var path in input.Split('|')) result.AddRange(GetFilesPathFor1(path.Trim()));
        return result;
    }

    /// <summary>
    /// 获取文件大小
    /// </summary>
    public static long GetFileSize(string sFullName)
    {
        long lSize = 0;
        if (File.Exists(sFullName))
            lSize = new FileInfo(sFullName).Length;
        return lSize;
    }

    private static IList<string> GetFilesPathFor1(string input)
    {
        if (input.Contains("*"))
        {
            var dic = Path.GetDirectoryName(input)!;
            var filen = Path.GetFileName(input);
            return Directory.GetFiles(dic, filen, SearchOption.AllDirectories);
        }

        if (Directory.Exists(input))
            return Directory.GetFiles(input, "*.*", SearchOption.AllDirectories);
        if (File.Exists(input))
            return new List<string> { input };
        return new List<string>();
    }

    /// <summary>
    /// 压缩文件
    /// </summary>
    public static bool ZipFile(string fileToZip, string zipedFile)
    {
        var result = true;
        ZipOutputStream? zipStream = null;
        FileStream? fs = null;
        ZipEntry? ent = null;

        if (!File.Exists(fileToZip))
            return false;

        try
        {
            fs = File.OpenRead(fileToZip);
            var buffer = new byte[fs.Length];
            fs.ReadExactly(buffer);
            fs.Close();

            fs = File.Create(zipedFile);
            zipStream = new ZipOutputStream(fs);
            ent = new ZipEntry(Path.GetFileName(fileToZip));
            zipStream.PutNextEntry(ent);
            zipStream.SetLevel(6);

            zipStream.Write(buffer, 0, buffer.Length);
        }
        catch
        {
            result = false;
        }
        finally
        {
            if (zipStream != null)
            {
                zipStream.Finish();
                zipStream.Close();
            }

            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }

        return result;
    }

    /// <summary>
    /// 解压功能(解压压缩文件到指定目录)
    /// </summary>
    public static bool UnZip(string fileToUnZip, string zipedFolder)
    {
        var result = true;
        ZipInputStream? zipStream = null;

        if (!File.Exists(fileToUnZip))
            return false;

        if (!Directory.Exists(zipedFolder))
            Directory.CreateDirectory(zipedFolder);

        try
        {
            zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
            ZipEntry? ent;
            while ((ent = zipStream.GetNextEntry()) != null)
                if (!string.IsNullOrEmpty(ent.Name))
                {
                    var fileName = Path.Combine(zipedFolder, ent.Name);
                    fileName = fileName.Replace('/', Path.DirectorySeparatorChar);

                    if (fileName.EndsWith(Path.DirectorySeparatorChar))
                    {
                        Directory.CreateDirectory(fileName);
                        continue;
                    }

                    using var streamWriter = File.Create(fileName);
                    var buffer = new byte[10240];
                    var size = zipStream.Read(buffer, 0, buffer.Length);
                    while (size > 0)
                    {
                        streamWriter.Write(buffer, 0, size);
                        size = zipStream.Read(buffer, 0, buffer.Length);
                    }
                }
        }
        catch
        {
            result = false;
        }
        finally
        {
            if (zipStream != null)
            {
                zipStream.Close();
                zipStream.Dispose();
            }
        }

        return result;
    }
}

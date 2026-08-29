/*
 *   Copyright © 2009-2020 studyzy(深蓝,曾毅)

 *   This program "IME WL Converter(深蓝词库转换)" is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.

 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.

 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Results;

namespace Studyzy.IMEWLConverter.Test;

public abstract class BaseTest
{
    protected IFormatExporter? exporter;
    protected IFormatImporter? importer;

    /// <summary>
    ///     深蓝测试
    /// </summary>
    protected WordEntry WlData = new()
    {
        Rank = 10,
        Code = WordCode.FromSingle(new[] { "shen", "lan", "ce", "shi" }),
        Word = "深蓝测试",
        CodeType = CodeType.Pinyin
    };

    protected abstract string StringData { get; }

    /// <summary>
    ///     深蓝测试
    ///     词库转换
    /// </summary>
    protected IReadOnlyList<WordEntry> WlListData
    {
        get
        {
            var wordEntry = new WordEntry
            {
                Rank = 80,
                Code = WordCode.FromSingle(new[] { "ci", "ku", "zhuan", "huan" }),
                Word = "词库转换",
                CodeType = CodeType.Pinyin
            };
            return new List<WordEntry> { WlData, wordEntry };
        }
    }

    protected string GetFullPath(string fileName)
    {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return Path.Combine(assemblyLocation!, "Test", fileName);
    }

    protected ImportResult ImportFromFile(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return importer!.ImportAsync(stream).GetAwaiter().GetResult();
    }

    protected ExportResult ExportToStream(IReadOnlyList<WordEntry> entries, Stream output)
    {
        return exporter!.ExportAsync(entries, output).GetAwaiter().GetResult();
    }
}

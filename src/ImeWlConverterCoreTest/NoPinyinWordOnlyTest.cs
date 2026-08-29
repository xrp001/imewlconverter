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

using System.IO;
using System.Text;
using Xunit;
using ImeWlConverter.Formats.NoPinyinWordOnly;

namespace Studyzy.IMEWLConverter.Test;

public class NoPinyinWordOnlyTest : BaseTest
{
    public NoPinyinWordOnlyTest()
    {
        importer = new NoPinyinWordOnlyImporter();
        exporter = new NoPinyinWordOnlyExporter();
    }

    protected override string StringData => Resource4Test.NoPinyinWordOnly;

    [Fact]
    public void TestExport()
    {
        using var ms = new MemoryStream();
        var result = ExportToStream(WlListData, ms);
        Assert.Equal(2, result.EntryCount);

        ms.Position = 0;
        var text = new StreamReader(ms, Encoding.UTF8).ReadToEnd();
        Assert.Contains("深蓝测试", text);
        Assert.Contains("词库转换", text);
    }

    [Fact]
    public void TestImport()
    {
        var bytes = Encoding.UTF8.GetBytes(StringData);
        using var ms = new MemoryStream(bytes);
        var result = importer!.ImportAsync(ms).GetAwaiter().GetResult();
        Assert.Equal(10, result.Entries.Count);
    }

    [Fact]
    public void TestImportFile()
    {
        var result = ImportFromFile(GetFullPath("纯汉字.txt"));
        Assert.True(result.Entries.Count > 0);
    }
}

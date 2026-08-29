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

using System;
using System.IO;
using System.Text;
using Xunit;
using ImeWlConverter.Formats.GooglePinyin;

namespace Studyzy.IMEWLConverter.Test;

public class GooglePinyinTest : BaseTest
{
    public GooglePinyinTest()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        exporter = new GooglePinyinExporter();
        importer = new GooglePinyinImporter();
    }

    protected override string StringData => Resource4Test.GooglePinyin;

    [Fact]
    public void TestExport()
    {
        using var ms = new MemoryStream();
        var result = ExportToStream(WlListData, ms);
        Assert.Equal(2, result.EntryCount);
    }

    [Fact]
    public void TestImport()
    {
        var bytes = Encoding.GetEncoding("GBK").GetBytes(StringData);
        using var ms = new MemoryStream(bytes);
        var result = importer!.ImportAsync(ms).GetAwaiter().GetResult();
        Assert.NotNull(result.Entries);
        Assert.Equal(10, result.Entries.Count);
    }
}

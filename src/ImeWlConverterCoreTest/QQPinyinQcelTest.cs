/*
 *   Copyright © 2022 yfdyh000

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
using Xunit;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Formats.QQPinyinQcel;

namespace Studyzy.IMEWLConverter.Test;

public class QQPinyinQcelTest : BaseTest
{
    public QQPinyinQcelTest()
    {
        importer = new QQPinyinQcelImporter();
    }

    protected override string StringData => throw new NotImplementedException();

    [Theory]
    [InlineData("星际战甲.qcel")]
    public void TestImportQcelWithAlphabet(string filePath)
    {
        var result = ImportFromFile(GetFullPath(filePath));
        var lib = result.Entries;
        Assert.True(lib.Count > 0);

        Assert.Equal(4675, lib.Count);
        Assert.Equal(CodeType.Pinyin, lib[0].CodeType);
        Assert.Equal("a'ka'ta", lib[2].Code?.GetPrimaryCode("'"));
        Assert.Equal(0, lib[0].Rank);
        Assert.Equal("阿卡塔", lib[2].Word);
    }
}

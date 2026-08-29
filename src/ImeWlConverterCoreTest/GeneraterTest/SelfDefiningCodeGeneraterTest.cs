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
using System.Linq;
using Xunit;
using ImeWlConverter.Core.CodeGeneration.Generators;

namespace Studyzy.IMEWLConverter.Test.GeneraterTest;

public class SelfDefiningCodeGeneraterTest
{
    [Fact]
    public void TestGenerateCode()
    {
        var generator = new SelfDefiningCodeGenerator();
        generator.MappingDictionary = new Dictionary<char, IList<string>>();
        generator.MappingDictionary.Add('深', new[] { "shen" });
        generator.MappingDictionary.Add('蓝', new[] { "lan" });
        generator.Is1Char1Code = false;

        generator.MutiWordCodeFormat =
            @"code_e2=p11+p12+p21+p22
code_e3=p11+p21+p31+p32
code_a4=p11+p21+p31+n11";
        var result = generator.GenerateCode("深蓝");
        // One-word-one-code mode: single segment with single code
        Assert.True(result.Segments.Count > 0);
        Assert.Equal("shla", result.Segments[0][0]);

        result = generator.GenerateCode("深深蓝");
        Assert.True(result.Segments.Count > 0);
        Assert.Equal("ssla", result.Segments[0][0]);

        result = generator.GenerateCode("深蓝深蓝");
        Assert.True(result.Segments.Count > 0);
        Assert.Equal("slsl", result.Segments[0][0]);
    }

    [Fact]
    public void TestGeneratePinyinFormatCode()
    {
        var generator = new SelfDefiningCodeGenerator();
        generator.MappingDictionary = new Dictionary<char, IList<string>>();
        generator.MappingDictionary.Add('深', new[] { "ipws" });
        generator.MappingDictionary.Add('蓝', new[] { "ajtl" });

        generator.Is1Char1Code = true;
        var result = generator.GenerateCode("深蓝");
        // Is1Char1Code mode: each char is a segment
        Assert.Equal(2, result.Segments.Count);
        Assert.Equal("ipws", result.Segments[0][0]);
        Assert.Equal("ajtl", result.Segments[1][0]);
    }

    [Fact]
    public void TestGenerateMutiPinyinFormatCode()
    {
        var generator = new SelfDefiningCodeGenerator();
        generator.MappingDictionary = new Dictionary<char, IList<string>>();
        generator.MappingDictionary.Add('深', new[] { "ipws", "ebcd" });
        generator.MappingDictionary.Add('蓝', new[] { "ajtl" });

        generator.Is1Char1Code = true;
        var result = generator.GenerateCode("深蓝");
        Assert.Equal(2, result.Segments.Count);
        Assert.Contains("ipws", result.Segments[0]);
    }
}

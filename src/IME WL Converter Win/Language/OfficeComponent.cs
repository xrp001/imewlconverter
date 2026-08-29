/*
 *   Copyright © 2009-2020 studyzy(深蓝,曾毅)
 *
 *   This program "IME WL Converter(深蓝词库转换)" is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Reflection;
using ImeWlConverter.Abstractions.Contracts;
using Microsoft.Office.Interop.Word;

namespace Studyzy.IMEWLConverter.Language;

internal class OfficeComponent : IChineseConverter, IDisposable
{
    #region IDisposable Members

    public void Dispose()
    {
        GC.Collect();
    }

    #endregion

    #region IChineseConverter Members

    public string ToSimplified(string traditional)
    {
        var doc = new Document();
        doc.Content.Text = traditional;
        doc.Content.TCSCConverter(
            WdTCSCConverterDirection.wdTCSCConverterDirectionTCSC,
            true,
            true
        );
        var des = doc.Content.Text;
        object saveChanges = false;
        object originalFormat = Missing.Value;
        object routeDocument = Missing.Value;
        doc.Close(ref saveChanges, ref originalFormat, ref routeDocument);
        GC.Collect();
        return des;
    }

    public string ToTraditional(string simplified)
    {
        var doc = new Document();
        doc.Content.Text = simplified;
        doc.Content.TCSCConverter(
            WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC,
            true,
            true
        );
        var des = doc.Content.Text;
        object saveChanges = false;
        object originalFormat = Missing.Value;
        object routeDocument = Missing.Value;
        doc.Close(ref saveChanges, ref originalFormat, ref routeDocument);
        GC.Collect();
        return des;
    }

    #endregion
}

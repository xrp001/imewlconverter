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

using System.Collections.Generic;
using System.Windows.Forms;

namespace Studyzy.IMEWLConverter;

/// <summary>
/// Maps format IDs to their configuration forms.
/// Some formats (e.g., Rime, SelfDefining) need user configuration before import/export.
/// </summary>
internal class CoreWinFormMapping
{
    // Format IDs that require configuration dialogs
    private static readonly Dictionary<string, System.Func<Form>> FormatFormFactory = new()
    {
        { "rime", () => new RimeConfigForm() },
        { "ld2", () => new Ld2EncodingConfigForm() },
        { "xiaoxiao", () => new XiaoxiaoConfigForm() },
        { "self", () => new SelfDefiningConfigForm() },
        { "phrase", () => new PhraseFormatConfigForm() },
        { "xiaoxiao_erbi", () => new ErbiTypeForm() },
        { "win10mspy", () => new PinyinConfigForm() },
        { "gboard", () => new PinyinConfigForm() },
    };

    public Form? GetConfigForm(string formatId)
    {
        if (FormatFormFactory.TryGetValue(formatId, out var factory))
            return factory();
        return null;
    }
}

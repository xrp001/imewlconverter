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
using System.ComponentModel;
using System.Windows.Forms;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Options;

namespace Studyzy.IMEWLConverter;

public partial class ChineseConverterSelectForm : Form
{
    private static int selectedTranslateIndex;

    public ChineseConverterSelectForm()
    {
        InitializeComponent();
        SelectedConversionMode = ChineseConversionMode.None;

        if (selectedTranslateIndex == 1)
        {
            rbtnNotTrans.Checked = false;
            rbtnTransToChs.Checked = true;
            rbtnTransToCht.Checked = false;
        }
        else if (selectedTranslateIndex == 2)
        {
            rbtnNotTrans.Checked = false;
            rbtnTransToChs.Checked = false;
            rbtnTransToCht.Checked = true;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ChineseConversionMode SelectedConversionMode { get; set; }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (rbtnNotTrans.Checked)
        {
            selectedTranslateIndex = 0;
            SelectedConversionMode = ChineseConversionMode.None;
        }
        else if (rbtnTransToChs.Checked)
        {
            selectedTranslateIndex = 1;
            SelectedConversionMode = ChineseConversionMode.TraditionalToSimplified;
        }
        else if (rbtnTransToCht.Checked)
        {
            selectedTranslateIndex = 2;
            SelectedConversionMode = ChineseConversionMode.SimplifiedToTraditional;
        }

        DialogResult = DialogResult.OK;
    }
}

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
using System.Windows.Forms;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Core.WordRank;

namespace Studyzy.IMEWLConverter;

public partial class WordRankGenerateForm : Form
{
    private static IWordRankGenerator wordRankGenerator = new DefaultWordRankGenerator();

    public WordRankGenerateForm()
    {
        InitializeComponent();
    }

    public IWordRankGenerator SelectedWordRankGenerator => wordRankGenerator;

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (rbtnDefault.Checked)
        {
            wordRankGenerator = new DefaultWordRankGenerator { DefaultRank = (int)numRank.Value };
        }
        else if (rbtnLlm.Checked)
        {
            wordRankGenerator = new LlmWordRankGenerator(new LlmConfig
            {
                ApiEndpoint = txtLlmEndpoint.Text,
                ApiKey = txtLlmKey.Text,
                Model = txtLlmModel.Text
            });
        }

        DialogResult = DialogResult.OK;
    }

    private void WordRankGenerateForm_Load(object sender, EventArgs e)
    {
        if (wordRankGenerator is DefaultWordRankGenerator defaultGen)
        {
            rbtnDefault.Checked = true;
        }

        UpdateLlmControls();
    }

    private void rbtnLlm_CheckedChanged(object sender, EventArgs e)
    {
        UpdateLlmControls();
    }

    private void UpdateLlmControls()
    {
        var enabled = rbtnLlm.Checked;
        txtLlmEndpoint.Enabled = enabled;
        txtLlmKey.Enabled = enabled;
        txtLlmModel.Enabled = enabled;
    }
}

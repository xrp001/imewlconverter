using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ImeWlConverterMac.Views;

public partial class WordRankGenerateWindow : Window
{
    public int DefaultRank { get; private set; } = 1;
    public bool ForceOverride { get; private set; }
    public string SelectedMode { get; private set; } = "default";

    public WordRankGenerateWindow()
    {
        InitializeComponent();
        LoadConfig();
    }

    private void LoadConfig()
    {
        rbtnDefault.IsChecked = true;
        numRank.Value = DefaultRank;
        cbxForceUseNewRank.IsChecked = ForceOverride;
    }

    private void BtnOK_Click(object? sender, RoutedEventArgs e)
    {
        if (rbtnDefault.IsChecked == true)
        {
            SelectedMode = "default";
            DefaultRank = (int)(numRank.Value ?? 1);
        }
        else if (rbtnLlm.IsChecked == true)
        {
            SelectedMode = "llm";
        }
        else if (rbtnCalc.IsChecked == true)
        {
            SelectedMode = "calc";
        }

        ForceOverride = cbxForceUseNewRank.IsChecked ?? false;

        Close(true);
    }

    private void BtnCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}

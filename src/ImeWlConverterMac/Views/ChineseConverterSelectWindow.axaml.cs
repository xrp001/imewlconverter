using Avalonia.Controls;
using Avalonia.Interactivity;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Options;

namespace ImeWlConverterMac.Views;

public partial class ChineseConverterSelectWindow : Window
{
    public ChineseConversionMode SelectedConversionMode { get; private set; }

    public ChineseConverterSelectWindow()
    {
        InitializeComponent();
        SelectedConversionMode = ChineseConversionMode.None;
        LoadConfig();
    }

    public ChineseConverterSelectWindow(ChineseConversionMode currentMode)
    {
        InitializeComponent();
        SelectedConversionMode = currentMode;
        LoadConfig();
    }

    private void LoadConfig()
    {
        // 设置转换类型
        switch (SelectedConversionMode)
        {
            case ChineseConversionMode.None:
                rbtnNotTrans.IsChecked = true;
                break;
            case ChineseConversionMode.TraditionalToSimplified:
                rbtnTransToChs.IsChecked = true;
                break;
            case ChineseConversionMode.SimplifiedToTraditional:
                rbtnTransToCht.IsChecked = true;
                break;
        }

        // 设置转换引擎 - macOS 只支持系统内核
        rbtnKernel.IsChecked = true;
    }

    private void BtnOK_Click(object? sender, RoutedEventArgs e)
    {
        // 获取转换类型
        if (rbtnNotTrans.IsChecked == true)
        {
            SelectedConversionMode = ChineseConversionMode.None;
        }
        else if (rbtnTransToChs.IsChecked == true)
        {
            SelectedConversionMode = ChineseConversionMode.TraditionalToSimplified;
        }
        else if (rbtnTransToCht.IsChecked == true)
        {
            SelectedConversionMode = ChineseConversionMode.SimplifiedToTraditional;
        }

        Close(true);
    }

    private void BtnCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}

using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Core.Helpers;

/// <summary>
/// Helper for CodeType enum operations.
/// </summary>
public static class CodeTypeHelper
{
    public static string GetCodeTypeName(CodeType codeType)
    {
        return codeType switch
        {
            CodeType.Pinyin => "拼音",
            CodeType.Wubi86 => "五笔86",
            CodeType.Wubi98 => "五笔98",
            CodeType.WubiNewAge => "五笔新世纪",
            CodeType.QingsongErbi => "二笔",
            CodeType.English => "英语",
            CodeType.Yong => "永码",
            CodeType.Zhengma => "郑码",
            CodeType.InnerCode => "内码",
            CodeType.Cangjie5 => "仓颉",
            CodeType.TerraPinyin => "地球拼音",
            CodeType.Zhuyin => "注音",
            CodeType.Chaoyin => "超音",
            CodeType.ChaoqiangErbi => "超强二笔",
            CodeType.XiandaiErbi => "现代二笔",
            CodeType.YinxingErbi => "隐形二笔",
            CodeType.UserDefine => "用户自定义",
            CodeType.UserDefinePhrase => "用户自定义短语",
            CodeType.NoCode => "无编码",
            CodeType.Phrase => "短语",
            CodeType.Shuangpin => "双拼",
            _ => "未知"
        };
    }
}

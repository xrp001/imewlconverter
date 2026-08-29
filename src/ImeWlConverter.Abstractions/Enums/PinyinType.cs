namespace ImeWlConverter.Abstractions.Enums;

/// <summary>
/// Pinyin scheme types used by different IME systems.
/// </summary>
public enum PinyinType
{
    /// <summary>Full pinyin (全拼).</summary>
    FullPinyin = 0,

    /// <summary>Xiaohe Shuangpin (小鹤双拼).</summary>
    XiaoHe,

    /// <summary>ZiRanMa Shuangpin (自然码双拼).</summary>
    ZiRanMa,

    /// <summary>Microsoft Shuangpin (微软双拼).</summary>
    Microsoft,

    /// <summary>Sogou Shuangpin (搜狗双拼).</summary>
    Sogou,

    /// <summary>ABC Shuangpin.</summary>
    ABC,

    /// <summary>ZhiNeng ABC Shuangpin (智能ABC双拼).</summary>
    ZhiNengABC,

    /// <summary>PinyinPlusPlus Shuangpin (拼音加加双拼).</summary>
    PinyinPlusPLus,

    /// <summary>USBi Shuangpin (万能五笔双拼).</summary>
    USBi,

    /// <summary>PinyinJiaJia Shuangpin (拼音加加).</summary>
    PinyinJiaJia
}

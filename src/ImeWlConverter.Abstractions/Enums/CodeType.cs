namespace ImeWlConverter.Abstractions.Enums;

/// <summary>
/// Supported encoding/code types for word entries.
/// </summary>
public enum CodeType
{
    /// <summary>Standard Hanyu Pinyin.</summary>
    Pinyin = 0,

    /// <summary>Wubi 86 version.</summary>
    Wubi86,

    /// <summary>Wubi 98 version.</summary>
    Wubi98,

    /// <summary>Wubi New Age version.</summary>
    WubiNewAge,

    /// <summary>Zhengma encoding.</summary>
    Zhengma,

    /// <summary>Cangjie 5 encoding.</summary>
    Cangjie5,

    /// <summary>Terra Pinyin (地球拼音).</summary>
    TerraPinyin,

    /// <summary>Bopomofo/Zhuyin encoding.</summary>
    Zhuyin,

    /// <summary>Qingsong Erbi (青松二笔).</summary>
    QingsongErbi,

    /// <summary>Chaoqiang Erbi (超强二笔).</summary>
    ChaoqiangErbi,

    /// <summary>Xiandai Erbi (现代二笔).</summary>
    XiandaiErbi,

    /// <summary>Yinxing Erbi (隐形二笔).</summary>
    YinxingErbi,

    /// <summary>Chaoyin encoding (超音).</summary>
    Chaoyin,

    /// <summary>English words.</summary>
    English,

    /// <summary>User-defined encoding.</summary>
    UserDefine,

    /// <summary>No code associated.</summary>
    NoCode,

    /// <summary>Phrase-based encoding.</summary>
    Phrase,

    /// <summary>Shuangpin (双拼) encoding.</summary>
    Shuangpin,

    /// <summary>Yong encoding (永码).</summary>
    Yong,

    /// <summary>Internal code (内码).</summary>
    InnerCode,

    /// <summary>User-defined phrase (用户自定义短语).</summary>
    UserDefinePhrase,

    /// <summary>Unknown encoding type.</summary>
    Unknown
}

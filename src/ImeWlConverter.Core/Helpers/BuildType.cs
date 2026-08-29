namespace ImeWlConverter.Core.Helpers;

/// <summary>
/// Defines how separators are placed around a joined string.
/// </summary>
public enum BuildType
{
    /// <summary>字符串左边包含分隔符</summary>
    LeftContain,

    /// <summary>字符串右边包含分隔符</summary>
    RightContain,

    /// <summary>字符串两侧都不包含分隔符</summary>
    None,

    /// <summary>字符串两侧都有分隔符</summary>
    FullContain
}

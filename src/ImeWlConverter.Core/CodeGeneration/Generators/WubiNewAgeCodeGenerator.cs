using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Core.Helpers;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// WubiNewAgeCodeGenerator 新世纪五笔编码生成器。
/// </summary>
public sealed class WubiNewAgeCodeGenerator : WubiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.WubiNewAge;

    protected override string GetWubiCode(ChineseCode code) => code.WubiNewAge;
}

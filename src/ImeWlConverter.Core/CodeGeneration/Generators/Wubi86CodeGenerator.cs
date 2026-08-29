using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Core.Helpers;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// Wubi86CodeGenerator 五笔86版编码生成器。
/// </summary>
public sealed class Wubi86CodeGenerator : WubiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.Wubi86;

    protected override string GetWubiCode(ChineseCode code) => code.Wubi86;
}

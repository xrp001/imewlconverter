using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Core.Helpers;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// Wubi98CodeGenerator 五笔98版编码生成器。
/// </summary>
public sealed class Wubi98CodeGenerator : WubiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.Wubi98;

    protected override string GetWubiCode(ChineseCode code) => code.Wubi98;
}

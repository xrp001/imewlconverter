using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// QingsongErbiCodeGenerator 青松二笔编码生成器。
/// </summary>
public sealed class QingsongErbiCodeGenerator : ErbiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.QingsongErbi;

    protected override int DicColumnIndex => 4;
}

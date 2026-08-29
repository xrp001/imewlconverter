using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// ChaoqiangErbiCodeGenerator 超强二笔编码生成器。
/// </summary>
public sealed class ChaoqiangErbiCodeGenerator : ErbiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.ChaoqiangErbi;

    protected override int DicColumnIndex => 3;
}

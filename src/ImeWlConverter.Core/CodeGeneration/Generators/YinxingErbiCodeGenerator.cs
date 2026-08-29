using ImeWlConverter.Abstractions.Enums;

namespace ImeWlConverter.Core.CodeGeneration.Generators;

/// <summary>
/// YinxingErbiCodeGenerator 隐形二笔（音形二笔）编码生成器。
/// </summary>
public sealed class YinxingErbiCodeGenerator : ErbiCodeGeneratorBase
{
    public override CodeType SupportedType => CodeType.YinxingErbi;

    protected override int DicColumnIndex => 2;
}

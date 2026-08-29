# IME WL Converter Codebase - Detailed Analysis Report

**Generated:** 2026-05-08  
**Scope:** Comprehensive exploration of the IME WL Converter project structure, dependencies, and architecture

---

## 1. PROJECT REFERENCES (PackageReference & ProjectReference)

### 1.1 src/ImeWlConverterCmd/ImeWlConverterCmd.csproj
**File Path:** `src/ImeWlConverterCmd/ImeWlConverterCmd.csproj` (Lines 1-55)

**Target Framework:** `net10.0`  
**Output Type:** `Exe`

**PackageReferences:**
```xml
<ItemGroup>
    <PackageReference Include="UTF.Unknown" Version="2.5.1"/>
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1"/>
</ItemGroup>
```

**ProjectReferences:**
```xml
<ItemGroup>
    <ProjectReference Include="..\ImeWlConverterCore\ImeWlConverterCore.csproj">
    </ProjectReference>
</ItemGroup>
```

---

### 1.2 src/ImeWlConverterMac/ImeWlConverterMac.csproj
**File Path:** `src/ImeWlConverterMac/ImeWlConverterMac.csproj` (Lines 1-60)

**Target Framework:** `net10.0`  
**Output Type:** `WinExe` (macOS App Bundle)

**PackageReferences:**
```xml
<ItemGroup>
    <PackageReference Include="Avalonia" Version="11.2.3" />
    <PackageReference Include="Avalonia.Desktop" Version="11.2.3" />
    <PackageReference Include="Avalonia.Themes.Fluent" Version="11.2.3" />
    <PackageReference Include="Avalonia.Fonts.Inter" Version="11.2.3" />
    <PackageReference Condition="'$(Configuration)' == 'Debug'" Include="Avalonia.Diagnostics" Version="11.2.3" />
</ItemGroup>
```

**ProjectReferences:**
```xml
<ItemGroup>
    <ProjectReference Include="../ImeWlConverterCore/ImeWlConverterCore.csproj" />
</ItemGroup>
```

---

### 1.3 src/ImeWlConverter.Core/ImeWlConverter.Core.csproj
**File Path:** `src/ImeWlConverter.Core/ImeWlConverter.Core.csproj` (Lines 1-19)

**Target Framework:** `net10.0`  
**Output Type:** `Library` (implicit)

**ProjectReferences:**
```xml
<ItemGroup>
    <ProjectReference Include="..\ImeWlConverter.Abstractions\ImeWlConverter.Abstractions.csproj"/>
    <ProjectReference Include="..\ImeWlConverterCore\ImeWlConverterCore.csproj"/>
</ItemGroup>
```

**PackageReferences:**
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.7"/>
</ItemGroup>
```

---

### 1.4 src/ImeWlConverter.Formats/ImeWlConverter.Formats.csproj
**File Path:** `src/ImeWlConverter.Formats/ImeWlConverter.Formats.csproj` (Lines 1-21)

**Target Framework:** `net10.0`  
**Output Type:** `Library`

**ProjectReferences:**
```xml
<ItemGroup>
    <ProjectReference Include="..\ImeWlConverter.Abstractions\ImeWlConverter.Abstractions.csproj"/>
    <ProjectReference Include="..\ImeWlConverterCore\ImeWlConverterCore.csproj"/>
</ItemGroup>
<ItemGroup>
    <ProjectReference Include="..\ImeWlConverter.SourceGenerators\ImeWlConverter.SourceGenerators.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false"/>
</ItemGroup>
```

**PackageReferences:**
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0"/>
    <PackageReference Include="System.Text.Encoding.CodePages" Version="9.0.0"/>
</ItemGroup>
```

---

### 1.5 src/ImeWlConverter.Abstractions/ImeWlConverter.Abstractions.csproj
**File Path:** `src/ImeWlConverter.Abstractions/ImeWlConverter.Abstractions.csproj` (Lines 1-10)

**Target Framework:** `net10.0`  
**Output Type:** `Library`  
**No dependencies** - Pure abstraction library with interfaces and models only.

---

### 1.6 src/ImeWlConverterCore/ImeWlConverterCore.csproj
**File Path:** `src/ImeWlConverterCore/ImeWlConverterCore.csproj` (Lines 1-67)

**Target Framework:** `net10.0`  
**Output Type:** `Library`  
**Platforms:** `AnyCPU;x86`

**PackageReferences:**
```xml
<ItemGroup>
    <PackageReference Include="SharpZipLib" Version="1.4.2"/>
    <PackageReference Include="UTF.Unknown" Version="2.5.1"/>
</ItemGroup>
```

**Embedded Resources:**
```xml
<ItemGroup>
    <EmbeddedResource Include="Resources\Cangjie5.txt"/>
    <EmbeddedResource Include="Resources\ChaoyinCodeMapping.txt"/>
    <EmbeddedResource Include="Resources\ChineseCode.txt"/>
    <EmbeddedResource Include="Resources\Erbi.txt"/>
    <EmbeddedResource Include="Resources\Shuangpin.txt"/>
    <EmbeddedResource Include="Resources\WordPinyin.txt"/>
    <EmbeddedResource Include="Resources\Zhengma.txt"/>
    <EmbeddedResource Include="Resources\Zhuyin.txt"/>
</ItemGroup>
```

---

## 2. CLI ENTRY FLOW

### 2.1 src/ImeWlConverterCmd/Program.cs
**File Path:** `src/ImeWlConverterCmd/Program.cs` (Lines 1-77)

**Entry Point:**
```csharp
private static int Main(string[] args)
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    
    // 检测旧格式参数（包含冒号）
    if (args.Any(arg => arg.Contains(":") && (arg.StartsWith("-i:") || arg.StartsWith("-o:") ||
                                               arg.StartsWith("-c:") || arg.StartsWith("-f:") ||
                                               arg.StartsWith("-ft:") || arg.StartsWith("-r:") ||
                                               arg.StartsWith("-ct:") || arg.StartsWith("-os:") ||
                                               arg.StartsWith("-mc:") || arg.StartsWith("-ld2:"))))
    {
        // Display migration guide and exit
        return 1;
    }
    
    // 使用新的命令行解析系统
    var rootCommand = CommandBuilder.Build();
    return rootCommand.Invoke(args);
}
```

**Key Points:**
- Line 29: `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` - Enables code page support
- Lines 32-36: Legacy format detection (colon-based parameters)
- Line 73: Delegates to `CommandBuilder.Build()` for new System.CommandLine parsing

---

### 2.2 src/ImeWlConverterCmd/FormatRegistrar.cs
**File Path:** `src/ImeWlConverterCmd/FormatRegistrar.cs` (Lines 1-93)

**Core Registration Method:**
```csharp
public static (Dictionary<string, IWordLibraryImport> imports, 
               Dictionary<string, IWordLibraryExport> exports, 
               Dictionary<string, string> names)
    RegisterAll()
{
    // Creates three dictionaries for format registration
    var imports = new Dictionary<string, IWordLibraryImport>();
    var exports = new Dictionary<string, IWordLibraryExport>();
    var names = new Dictionary<string, string>();
    
    // Import + Export formats (48 registrations)
    Register(imports, exports, names, ConstantString.SOUGOU_PINYIN_C, 
             ConstantString.SOUGOU_PINYIN, new SougouPinyin());
    // ... (47 more registrations)
    
    // Import only formats (11 registrations)
    Register(imports, exports, names, ConstantString.SOUGOU_PINYIN_BIN_C, 
             ConstantString.SOUGOU_PINYIN_BIN, new SougouPinyinBinFromPython());
    // ... (10 more registrations)
    
    // Export only formats (12 registrations)
    Register(imports, exports, names, ConstantString.QQ_PINYIN_C, 
             ConstantString.QQ_PINYIN, new QQPinyin());
    // ... (11 more registrations)
    
    return (imports, exports, names);
}
```

**Registration Helper (Lines 79-92):**
```csharp
private static void Register<T>(
    Dictionary<string, IWordLibraryImport> imports,
    Dictionary<string, IWordLibraryExport> exports,
    Dictionary<string, string> names,
    string code,
    string displayName,
    T instance)
{
    names[code] = displayName;
    if (instance is IWordLibraryImport importer)
        imports[code] = importer;
    if (instance is IWordLibraryExport exporter)
        exports[code] = exporter;
}
```

**Key Insight:**
- **Hard-coded registration** of 71 format types (no reflection in CLI)
- Each format is registered by code (e.g., "scel"), display name, and instance
- Supports mixed Import/Export/Import-only/Export-only capabilities
- Used by `CommandBuilder.ShowSupportedFormats()` and `ExecuteConversion()`

---

## 3. NEW ARCHITECTURE PIPELINE

### 3.1 src/ImeWlConverter.Core/Pipeline/ConversionPipeline.cs
**File Path:** `src/ImeWlConverter.Core/Pipeline/ConversionPipeline.cs` (Lines 1-78)

**Key Class: ConversionPipeline**
```csharp
public sealed class ConversionPipeline : IConversionPipeline
{
    private readonly IEnumerable<IFormatImporter> _importers;
    private readonly IEnumerable<IFormatExporter> _exporters;
    private readonly IProgressReporter? _progressReporter;

    public ConversionPipeline(
        IEnumerable<IFormatImporter> importers,
        IEnumerable<IFormatExporter> exporters,
        IProgressReporter? progressReporter = null)
    {
        _importers = importers;
        _exporters = exporters;
        _progressReporter = progressReporter;
    }

    public async Task<Result<ConversionResult>> ExecuteAsync(
        ConversionRequest request,
        CancellationToken ct = default)
    {
        // 1. Find importer/exporter by format ID
        var importer = _importers.FirstOrDefault(i => i.Metadata.Id == request.InputFormatId);
        if (importer is null)
            return Result<ConversionResult>.Failure($"Unknown input format: {request.InputFormatId}");

        var exporter = _exporters.FirstOrDefault(e => e.Metadata.Id == request.OutputFormatId);
        if (exporter is null)
            return Result<ConversionResult>.Failure($"Unknown output format: {request.OutputFormatId}");

        // 2. Import all input files
        var allEntries = new List<WordEntry>();
        foreach (var inputPath in request.InputPaths)
        {
            ct.ThrowIfCancellationRequested();
            _progressReporter?.Report(new ProgressInfo(0, 0, $"Importing {Path.GetFileName(inputPath)}..."));

            using var stream = File.OpenRead(inputPath);
            var importResult = await importer.ImportAsync(stream, request.Options.Import, ct);
            allEntries.AddRange(importResult.Entries);
        }

        var importedCount = allEntries.Count;

        // 3. Filter pipeline (to be wired up with DI; pass-through for now)
        var exportedCount = allEntries.Count;
        var filteredCount = importedCount - exportedCount;

        // 4. Export
        _progressReporter?.Report(new ProgressInfo(0, 0, "Exporting..."));
        using var outputStream = File.Create(request.OutputPath);
        await exporter.ExportAsync(allEntries, outputStream, request.Options.Export, ct);

        return Result<ConversionResult>.Success(new ConversionResult
        {
            ImportedCount = importedCount,
            ExportedCount = exportedCount,
            FilteredCount = filteredCount
        });
    }
}
```

**Pipeline Stages:**
1. **Discovery** - Find importer/exporter by ID (Lines 40-45)
2. **Import** - Read all input files to `WordEntry[]` (Lines 48-58)
3. **Filter** - Placeholder for filter pipeline (Lines 62-64)
4. **Export** - Write entries to output stream (Lines 67-69)

---

### 3.2 src/ImeWlConverter.Core/Adapters/LegacyImporterAdapter.cs
**File Path:** `src/ImeWlConverter.Core/Adapters/LegacyImporterAdapter.cs` (Lines 1-126)

**Adapter Pattern Implementation:**
```csharp
public sealed class LegacyImporterAdapter : IFormatImporter
{
    private readonly IWordLibraryImport _legacyImporter;

    public LegacyImporterAdapter(IWordLibraryImport legacyImporter, FormatMetadata metadata)
    {
        _legacyImporter = legacyImporter;
        Metadata = metadata;
    }

    public FormatMetadata Metadata { get; }

    public Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, 
                                         CancellationToken ct = default)
    {
        // Legacy API works with file paths, not streams
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var fs = File.Create(tempFile))
            {
                input.CopyTo(fs);
            }

            var legacyList = _legacyImporter.Import(tempFile);
            var entries = ConvertToWordEntries(legacyList);
            return Task.FromResult(new ImportResult
            {
                Entries = entries,
                ErrorCount = 0
            });
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    /// <summary>Maps legacy CodeType to new CodeType.</summary>
    internal static NewCodeType MapCodeType(OldCodeType old) => old switch
    {
        OldCodeType.Pinyin => NewCodeType.Pinyin,
        OldCodeType.Wubi => NewCodeType.Wubi86,
        OldCodeType.Wubi98 => NewCodeType.Wubi98,
        OldCodeType.WubiNewAge => NewCodeType.WubiNewAge,
        OldCodeType.Zhengma => NewCodeType.Zhengma,
        OldCodeType.Cangjie => NewCodeType.Cangjie5,
        OldCodeType.TerraPinyin => NewCodeType.TerraPinyin,
        OldCodeType.Zhuyin => NewCodeType.Zhuyin,
        OldCodeType.English => NewCodeType.English,
        OldCodeType.UserDefine => NewCodeType.UserDefine,
        OldCodeType.NoCode => NewCodeType.NoCode,
        OldCodeType.QingsongErbi => NewCodeType.QingsongErbi,
        OldCodeType.ChaoqiangErbi => NewCodeType.ChaoqiangErbi,
        OldCodeType.XiandaiErbi => NewCodeType.XiandaiErbi,
        OldCodeType.Chaoyin => NewCodeType.Chaoyin,
        _ => NewCodeType.Pinyin
    };
}
```

**Bridging:**
- Wraps legacy `IWordLibraryImport` in new `IFormatImporter` interface
- Converts legacy `CodeType` to new `CodeType` enum
- Uses temp file since legacy API expects file paths, not streams

---

### 3.3 src/ImeWlConverter.Core/Adapters/LegacyExporterAdapter.cs
**File Path:** `src/ImeWlConverter.Core/Adapters/LegacyExporterAdapter.cs` (Lines 1-119)

**Similar Adapter for Exporters:**
```csharp
public sealed class LegacyExporterAdapter : IFormatExporter
{
    private readonly IWordLibraryExport _legacyExporter;

    public FormatMetadata Metadata { get; }

    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries,
        Stream output,
        ExportOptions? options = null,
        CancellationToken ct = default)
    {
        var legacyList = ConvertToWordLibraryList(entries);
        var lines = _legacyExporter.Export(legacyList);

        var encoding = _legacyExporter.Encoding ?? Encoding.UTF8;
        using var writer = new StreamWriter(output, encoding, leaveOpen: true);

        var exportedCount = 0;
        foreach (var line in lines)
        {
            ct.ThrowIfCancellationRequested();
            writer.Write(line);
            exportedCount++;
        }

        writer.Flush();

        return Task.FromResult(new ExportResult
        {
            EntryCount = exportedCount,
            ErrorCount = 0
        });
    }

    /// <summary>Maps new CodeType to legacy CodeType.</summary>
    internal static OldCodeType MapCodeType(NewCodeType newType) => newType switch
    {
        NewCodeType.Pinyin => OldCodeType.Pinyin,
        NewCodeType.Wubi86 => OldCodeType.Wubi,
        NewCodeType.Wubi98 => OldCodeType.Wubi98,
        NewCodeType.WubiNewAge => OldCodeType.WubiNewAge,
        NewCodeType.Zhengma => OldCodeType.Zhengma,
        NewCodeType.Cangjie5 => OldCodeType.Cangjie,
        NewCodeType.TerraPinyin => OldCodeType.TerraPinyin,
        NewCodeType.Zhuyin => OldCodeType.Zhuyin,
        NewCodeType.English => OldCodeType.English,
        NewCodeType.UserDefine => OldCodeType.UserDefine,
        NewCodeType.NoCode => OldCodeType.NoCode,
        NewCodeType.QingsongErbi => OldCodeType.QingsongErbi,
        NewCodeType.ChaoqiangErbi => OldCodeType.ChaoqiangErbi,
        NewCodeType.XiandaiErbi => OldCodeType.XiandaiErbi,
        NewCodeType.Chaoyin => OldCodeType.Chaoyin,
        _ => OldCodeType.Pinyin
    };
}
```

---

## 4. INTERFACES WITHOUT IMPLEMENTATIONS

### 4.1 All Interfaces in src/ImeWlConverter.Abstractions/Contracts/

| Interface | File | Implementation Status | Notes |
|-----------|------|----------------------|-------|
| `IFormatImporter` | `IFormatImporter.cs` | **IMPLEMENTED** | Adapter: `LegacyImporterAdapter`, Format implementations in `ImeWlConverter.Formats` |
| `IFormatExporter` | `IFormatExporter.cs` | **IMPLEMENTED** | Adapter: `LegacyExporterAdapter`, Format implementations in `ImeWlConverter.Formats` |
| `IConversionPipeline` | `IConversionPipeline.cs` | **IMPLEMENTED** | `ConversionPipeline` in `ImeWlConverter.Core` |
| `ICodeGenerator` | `ICodeGenerator.cs` | **NO DIRECT IMPL** | Legacy: `IWordCodeGenerater` in `ImeWlConverterCore/Generaters/` |
| `IChineseConverter` | `IChineseConverter.cs` | **NO IMPLEMENTATION** | ❌ Interface defined but NO concrete implementation found |
| `ILlmClient` | `ILlmClient.cs` | **IMPLEMENTED** | `HttpLlmClient` in `ImeWlConverter.Core/LlmIntegration/` |
| `IPinyinDictionary` | `IPinyinDictionary.cs` | **NO DIRECT IMPL** | Legacy classes exist but no explicit implementation |
| `IProgressReporter` | `IProgressReporter.cs` | **NO IMPLEMENTATION** | ❌ Interface defined but NO concrete implementation found |
| `IWordFilter` | `IWordFilter.cs` | **NO DIRECT IMPL** | Legacy: `ISingleFilter` in `ImeWlConverterCore/Filters/` |
| `IWordRankGenerator` | `IWordRankGenerator.cs` | **NO DIRECT IMPL** | Legacy: `IWordRankGenerater` in `ImeWlConverterCore/Generaters/` |

**UNIMPLEMENTED INTERFACES (MISSING):**
- `IChineseConverter` (Lines 1-8 in Contracts folder) - **NO IMPLEMENTATION FOUND**
- `IProgressReporter` (in Contracts folder) - **NO IMPLEMENTATION FOUND**

---

## 5. SOURCE GENERATOR OUTPUT

### 5.1 src/ImeWlConverter.SourceGenerators/FormatRegistrationGenerator.cs
**File Path:** `src/ImeWlConverter.SourceGenerators/FormatRegistrationGenerator.cs` (Lines 1-223)

**Generator Type:** `IIncrementalGenerator`

**Core Generator Implementation (Lines 19-46):**
```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    var formatClasses = context.SyntaxProvider
        .ForAttributeWithMetadataName(
            "ImeWlConverter.Abstractions.FormatPluginAttribute",
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => GetFormatInfo(ctx))
        .Where(static info => info is not null)
        .Select(static (info, _) => info!.Value);

    var collected = formatClasses.Collect();

    context.RegisterSourceOutput(collected, static (spc, formats) =>
    {
        if (formats.IsEmpty) return;

        // Generate DI registration
        spc.AddSource("FormatRegistry.g.cs",
            SourceText.From(GenerateRegistrationCode(formats), Encoding.UTF8));

        // Generate Metadata property for each format class
        foreach (var format in formats)
        {
            var source = GenerateMetadataCode(format);
            var hintName = format.ClassName + ".Metadata.g.cs";
            spc.AddSource(hintName, SourceText.From(source, Encoding.UTF8));
        }
    });
}
```

**Generated Code Pattern (Lines 130-162):**
```csharp
// FormatRegistry.g.cs - Auto-generated
public static class FormatRegistry
{
    /// <summary>Register all discovered format importers and exporters.</summary>
    public static IServiceCollection AddAllFormats(this IServiceCollection services)
    {
        services.AddSingleton<IFormatImporter, SougouPinyinImporter>();
        services.AddSingleton<IFormatImporter, BaiduPinyinImporter>();
        // ... (discovers all classes with [FormatPlugin] attribute)
        return services;
    }
}

// SougouPinyin.Metadata.g.cs - Per-format
partial class SougouPinyinImporter
{
    public ImeWlConverter.Abstractions.Models.FormatMetadata Metadata { get; } =
        new("scel", "搜狗输入法词库", 100, 
            SupportsImport: true, SupportsExport: true, IsBinary: false);
}
```

**Attribute Detection (Lines 49-102):**
- Scans for `[FormatPluginAttribute]` on classes
- Extracts: ID, display name, sort order, import/export capability, binary flag
- Determines if needs "override" keyword for abstract base classes
- Inheritance detection: `TextImporterBase`, `TextExporterBase`, `BinaryImporterBase`

---

## 6. ENCODING.REGISTERPROVIDER CALLS (36 Total)

**Files Using `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)`:**

### In New Formats (src/ImeWlConverter.Formats/ - 20 files):
1. `BaiduBcd/BaiduBcdImporter.cs`
2. `BaiduBdict/BaiduBdictImporter.cs`
3. `Win10Ms/Win10MsWubiImporter.cs`
4. `Win10Ms/Win10MsPinyinImporter.cs`
5. `QQPinyinQpyd/QQPinyinQpydImporter.cs`
6. `JidianMBDict/JidianMBDictImporter.cs`
7. `GooglePinyin/GooglePinyinExporter.cs`
8. `GooglePinyin/GooglePinyinImporter.cs`
9. `CangjiePlatform/CangjiePlatformImporter.cs`
10. `CangjiePlatform/CangjiePlatformExporter.cs`
11. `Xiaoxiao/XiaoxiaoImporter.cs`
12. `Xiaoxiao/XiaoxiaoErbiExporter.cs`
13. `ZiGuangUwl/ZiGuangUwlImporter.cs`
14. `SelfDefining/SelfDefiningImporter.cs`
15. `Win10MsSelfStudy/Win10MsPinyinSelfStudyImporter.cs`
16. `BaiduPinyinBackup/BaiduPinyinBackupImporter.cs`
17. `SougouPinyin/SougouPinyinImporter.cs`
18. `SougouPinyin/SougouPinyinExporter.cs`
19. `SougouScel/SougouScelImporter.cs`
20. `SougouBin/SougouBinImporter.cs`

### In Legacy Core (src/ImeWlConverterCore/ - 11 files):
- `MainBody.cs` (Line 1)
- `IME/Gboard.cs`
- `IME/SinaPinyin.cs`
- `IME/GooglePinyin.cs`
- `IME/CangjiePlatform.cs`
- `IME/Xiaoxiao.cs`
- `IME/XiaoxiaoErbi.cs`
- `IME/SougouPinyin.cs`
- `Language/SystemKernel.cs`

### In CLI & Tests (5 files):
- `ImeWlConverterCmd/Program.cs` (Line 29)
- `ImeWlConverterCoreTest/NoPinyinWordOnlyTest.cs`
- `ImeWlConverterCoreTest/HelperTest/FileOperationTest.cs` (2 occurrences)

### In Windows GUI:
- `IME WL Converter Win/EncodingComboBox.cs`

**Total Count:** 36 occurrences

**Why needed:** Enables support for legacy Windows code pages (GBK, GB2312, BIG5, etc.) required for parsing IME dictionary files.

---

## 7. LEGACY CORE STRUCTURE

### 7.1 src/ImeWlConverterCore/IME/ - Format Implementations (55 files)
```
BaiduPinyin.cs
BaiduPinyinBackup.cs
BaiduPinyinBdict.cs
BaiduShouji.cs
BaiduShoujiBcd.cs
BaiduShoujiEng.cs
BaseImport.cs
BaseTextImport.cs
BingPinyin.cs
CangjiePlatform.cs
Chaoyin.cs
ChinesePyim.cs
Emoji.cs
FitInput.cs
Gboard.cs
GooglePinyin.cs
Jidian.cs
JidianMBDict.cs
JidianZhengma.cs
LibIMEText.cs
Libpinyin.cs
LingoesLd2.cs
MacPlist.cs
MsPinyin.cs
NoPinyinWordOnly.cs
PinyinJiaJia.cs
QQPinyin.cs
QQPinyinEng.cs
QQPinyinQcel.cs
QQPinyinQpyd.cs
QQShouji.cs
QQWubi.cs
Rime.cs
RimeUserDb.cs
SelfDefining.cs
ShouxinPinyin.cs
SinaPinyin.cs
SougouPinyin.cs
SougouPinyinBinFromPython.cs
SougouPinyinDict.cs
SougouPinyinScel.cs
UserDefinePhrase.cs
Win10MsPinyin.cs
Win10MsPinyinSelfStudy.cs
Win10MsWubi.cs
Wubi86.cs
Wubi98.cs
WubiNewAge.cs
Xiaoxiao.cs
XiaoxiaoErbi.cs
XiaoyaWubi.cs
YahooKeyKey.cs
ZiGuangPinyin.cs
ZiGuangPinyinUwl.cs
iFlyIME.cs
```

### 7.2 src/ImeWlConverterCore/Generaters/ - Code Generators (22 files)
```
BaseCodeGenerater.cs          - Abstract base
CalcWordRankGenerater.cs      - Frequency calculation
Cangjie5Generater.cs          - Cangjie 5 encoding
ChaoqiangErbiGenerater.cs     - Chaoqiang Erbi encoding
ChaoyinGenerater.cs           - Chaoyin encoding
DefaultWordRankGenerater.cs   - Default frequency (1)
ErbiGenerater.cs              - Erbi base class
IWordCodeGenerater.cs         - Interface
IWordRankGenerater.cs         - Rank interface
LlmWordRankGenerater.cs       - LLM-based ranking
PhraseGenerater.cs            - Phrase generation
PinyinGenerater.cs            - Pinyin generation
QingsongErbiGenerater.cs      - Qingsong Erbi
SelfDefiningCodeGenerater.cs  - Custom encoding
TerraPinyinGenerater.cs       - Terra Pinyin
Wubi86Generater.cs            - Wubi 86
Wubi98Generater.cs            - Wubi 98
WubiNewAgeGenerater.cs        - Wubi New Age
XiandaiErbiGenerater.cs       - Xiandai Erbi
YinxingErbiGenerater.cs       - Yinxing Erbi
ZhengmaGenerater.cs           - Zhengma encoding
ZhuyinGenerater.cs            - Zhuyin/Bopomofo
```

### 7.3 src/ImeWlConverterCore/Filters/ - Word Filtering (16 files)
```
ChinesePunctuationFilter.cs    - Remove Chinese punctuation
DistinctFilter.cs             - Remove duplicates
EmojiReplacer.cs              - Replace emoji
EnglishFilter.cs              - Filter English
EnglishPunctuationFilter.cs    - Remove English punctuation
FirstCJKFilter.cs             - Keep first CJK only
IBatchFilter.cs               - Batch processing interface
IReplaceFilter.cs             - Replace interface
ISingleFilter.cs              - Single word interface
LengthFilter.cs               - Filter by word length
NoAlphabetCodeFilter.cs       - Filter non-alphabetic codes
NumberFilter.cs               - Filter numbers
RankFilter.cs                 - Filter by word frequency rank
RankPercentageFilter.cs       - Filter by rank percentage
SpaceFilter.cs                - Filter spaces
ShuangpinReplacer.cs          - Replace with Shuangpin
```

---

## 8. NEW FORMATS STRUCTURE

### Example: Importer/Exporter Pattern

**src/ImeWlConverter.Formats/BaiduPinyin/**
```
BaiduPinyinImporter.cs    - [FormatPlugin("bdpy", "百度拼音")]
                            Implements IFormatImporter
                            
BaiduPinyinExporter.cs    - Implements IFormatExporter
```

**src/ImeWlConverter.Formats/SougouPinyin/**
```
SougouPinyinImporter.cs   - [FormatPlugin("scel", "搜狗拼音")]
                            Implements IFormatImporter
                            
SougouPinyinExporter.cs   - Implements IFormatExporter
```

**Pattern per Format:**
- One directory per IME format
- Separate Importer and Exporter classes
- Decorated with `[FormatPluginAttribute]`
- Implements `IFormatImporter` and/or `IFormatExporter`
- Auto-discovered by source generator
- Registered via generated `FormatRegistry.g.cs`

**Total Formats in New Structure:** 43+ directories

---

## 9. ENUM DUPLICATIONS

### 9.1 Legacy CodeType - src/ImeWlConverterCore/Entities/CodeType.cs
**Lines 20-117 (Old enum, 17 values)**

```csharp
public enum CodeType
{
    UserDefinePhrase = 0,
    Wubi,
    Wubi98,
    WubiNewAge,
    Zhengma,
    Cangjie,
    Unknown,
    UserDefine,
    Pinyin,
    Yong,
    QingsongErbi,
    ChaoqiangErbi,
    ChaoqingYinxin,
    English,
    InnerCode,
    XiandaiErbi,
    Zhuyin,
    TerraPinyin,
    Chaoyin,
    NoCode
}
```

### 9.2 New CodeType - src/ImeWlConverter.Abstractions/Enums/CodeType.cs
**Lines 6-61 (New enum, 20 values)**

```csharp
public enum CodeType
{
    Pinyin = 0,
    Wubi86,
    Wubi98,
    WubiNewAge,
    Zhengma,
    Cangjie5,
    TerraPinyin,
    Zhuyin,
    QingsongErbi,
    ChaoqiangErbi,
    XiandaiErbi,
    YinxingErbi,        // NEW
    Chaoyin,
    English,
    UserDefine,
    NoCode,
    Phrase,             // NEW
    Shuangpin           // NEW
}
```

### 9.3 Mapping Between Enums
**Implemented in LegacyImporterAdapter (Lines 107-125):**
```csharp
internal static NewCodeType MapCodeType(OldCodeType old) => old switch
{
    OldCodeType.Pinyin => NewCodeType.Pinyin,
    OldCodeType.Wubi => NewCodeType.Wubi86,
    OldCodeType.Wubi98 => NewCodeType.Wubi98,
    OldCodeType.WubiNewAge => NewCodeType.WubiNewAge,
    OldCodeType.Zhengma => NewCodeType.Zhengma,
    OldCodeType.Cangjie => NewCodeType.Cangjie5,
    OldCodeType.TerraPinyin => NewCodeType.TerraPinyin,
    OldCodeType.Zhuyin => NewCodeType.Zhuyin,
    OldCodeType.English => NewCodeType.English,
    OldCodeType.UserDefine => NewCodeType.UserDefine,
    OldCodeType.NoCode => NewCodeType.NoCode,
    OldCodeType.QingsongErbi => NewCodeType.QingsongErbi,
    OldCodeType.ChaoqiangErbi => NewCodeType.ChaoqiangErbi,
    OldCodeType.XiandaiErbi => NewCodeType.XiandaiErbi,
    OldCodeType.Chaoyin => NewCodeType.Chaoyin,
    _ => NewCodeType.Pinyin
};
```

**Similar mapping in LegacyExporterAdapter (Lines 100-118)**

---

## 10. MACOS GUI FORMAT LOADING (Reflection-Based)

### 10.1 src/ImeWlConverterMac/ViewModels/MainWindowViewModel.cs
**Reflection Loading Method (Lines 201-247)**

```csharp
private void LoadImeList()
{
    var assembly = typeof(MainBody).Assembly;  // Gets ImeWlConverterCore assembly
    var types = assembly.GetTypes();
    var cbxImportItems = new List<ComboBoxShowAttribute>();
    var cbxExportItems = new List<ComboBoxShowAttribute>();

    foreach (var type in types)
    {
        // Filter by namespace: must be in Studyzy.IMEWLConverter.IME
        if (type.Namespace != null && type.Namespace.StartsWith("Studyzy.IMEWLConverter.IME"))
        {
            // Look for [ComboBoxShowAttribute]
            var att = type.GetCustomAttributes(typeof(ComboBoxShowAttribute), false);
            if (att.Length > 0)
            {
                var cbxa = att[0] as ComboBoxShowAttribute;
                if (cbxa != null)
                {
                    // Check if implements IWordLibraryImport
                    if (type.GetInterface("IWordLibraryImport") != null)
                    {
                        cbxImportItems.Add(cbxa);
                        var instance = assembly.CreateInstance(type.FullName!) as IWordLibraryImport;
                        if (instance != null)
                            _imports.Add(cbxa.Name, instance);
                    }

                    // Check if implements IWordLibraryExport
                    if (type.GetInterface("IWordLibraryExport") != null)
                    {
                        cbxExportItems.Add(cbxa);
                        var instance = assembly.CreateInstance(type.FullName!) as IWordLibraryExport;
                        if (instance != null)
                            _exports.Add(cbxa.Name, instance);
                    }
                }
            }
        }
    }

    // Sort by ComboBoxShowAttribute.Index
    cbxImportItems.Sort((a, b) => a.Index - b.Index);
    cbxExportItems.Sort((a, b) => a.Index - b.Index);

    // Populate UI collections
    ImportTypes.Clear();
    foreach (var item in cbxImportItems)
        ImportTypes.Add(item.Name);

    ExportTypes.Clear();
    foreach (var item in cbxExportItems)
        ExportTypes.Add(item.Name);
}
```

**Key Reflection Steps:**
1. **Line 203:** Get assembly containing legacy implementations
2. **Line 204:** Get all types
3. **Line 210:** Filter by namespace (must start with "Studyzy.IMEWLConverter.IME")
4. **Line 212:** Get `[ComboBoxShowAttribute]` custom attribute
5. **Line 218:** Check for `IWordLibraryImport` interface
6. **Line 221:** Create instance via reflection: `assembly.CreateInstance(type.FullName!)`
7. **Line 222-223:** Store in dictionary by attribute name

**Differences from CLI:**
- **CLI (FormatRegistrar):** Hard-coded instantiation of all 71 formats
- **GUI (LoadImeList):** Dynamic reflection-based discovery from assembly

---

## 11. COMMANDLINEOPTIONS CLASS

### 11.1 src/ImeWlConverterCore/CommandLineOptions.cs
**File Path:** `src/ImeWlConverterCore/CommandLineOptions.cs` (Lines 1-103)

**Full Class Definition:**
```csharp
#nullable enable

using System.Collections.Generic;

namespace Studyzy.IMEWLConverter;

/// <summary>
/// 命令行选项的强类型模型
/// </summary>
public class CommandLineOptions
{
    /// <summary>
    /// 输入词库格式代码（如 scel, ggpy, qqpy）
    /// </summary>
    public string InputFormat { get; set; } = string.Empty;

    /// <summary>
    /// 输出词库格式代码（如 ggpy, rime, self）
    /// </summary>
    public string OutputFormat { get; set; } = string.Empty;

    /// <summary>
    /// 输出文件路径或目录路径
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>
    /// 输入文件路径列表（支持多文件）
    /// </summary>
    public List<string> InputFiles { get; set; } = new();

    /// <summary>
    /// 编码映射文件路径（用于自定义编码）
    /// </summary>
    public string? CodeFile { get; set; }

    /// <summary>
    /// 过滤条件字符串（如 "len:1-100|rm:eng"）
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 自定义格式规范（如 "213, nyyn"）
    /// </summary>
    public string? CustomFormat { get; set; }

    /// <summary>
    /// 词频生成器类型（llm, 或固定数字）
    /// </summary>
    public string? RankGenerator { get; set; }

    /// <summary>
    /// LLM API Endpoint
    /// </summary>
    public string? LlmEndpoint { get; set; }

    /// <summary>
    /// LLM API Key
    /// </summary>
    public string? LlmKey { get; set; }

    /// <summary>
    /// LLM Model Name
    /// </summary>
    public string? LlmModel { get; set; }

    /// <summary>
    /// 多字词编码生成规则
    /// </summary>
    public string? MultiCode { get; set; }

    /// <summary>
    /// 编码类型（pinyin, wubi, zhengma, cangjie, zhuyin）
    /// </summary>
    public string? CodeType { get; set; }

    /// <summary>
    /// 目标操作系统（windows, macos, linux）
    /// </summary>
    public string? TargetOS { get; set; }

    /// <summary>
    /// Lingoes ld2 文件编码设置
    /// </summary>
    public string? Ld2Encoding { get; set; }
}
```

**Properties (14 total):**
- **Required:** `InputFormat`, `OutputFormat`, `OutputPath`, `InputFiles`
- **Optional:** `CodeFile`, `Filter`, `CustomFormat`, `RankGenerator`, `LlmEndpoint`, `LlmKey`, `LlmModel`, `MultiCode`, `CodeType`, `TargetOS`, `Ld2Encoding`

**Usage Location:**
- Created in `CommandBuilder.cs` Line 218
- Populated from parsed command-line arguments
- Passed to `ExecuteConversion(options)` for processing

---

## 12. TEST STRUCTURE

### 12.1 Root Test Files - src/ImeWlConverterCoreTest/
**21 test files at root level:**
```
BaiduBdictTest.cs              - Tests Baidu Bdict importer
BaiduShoujiTest.cs             - Tests Baidu Shouji format
BaseTest.cs                    - Base test class
EmojiTest.cs                   - Tests Emoji handling
GooglePinyinTest.cs            - Tests Google Pinyin importer
Ld2ParseTest.cs                - Tests Lingoes LD2 parsing
NoPinyinWordOnlyTest.cs        - Tests word-only export
PerformanceTest.cs             - Performance/stress tests
PinyinJiaJiaTest.cs            - Tests Pinyin JiaJia
QQPinyinQcelTest.cs            - Tests QQ Pinyin QCEL
QQPinyinQpydTest.cs            - Tests QQ Pinyin QPYD
QQPinyinTest.cs                - Tests QQ Pinyin importer
Resource4Test.cs               - Tests resource loading
RimeTest.cs                    - Tests Rime format
SelectedParsePatternTest.cs     - Tests custom parse patterns
SelfDefiningTest.cs            - Tests custom encoding
SougouPinyinBinTest.cs         - Tests Sougou Pinyin binary
SougouPinyinScelExportTest.cs  - Tests Sougou SCEL export
SougouPinyinScelTest.cs        - Tests Sougou SCEL format
SougouPinyinTest.cs            - Tests Sougou Pinyin importer
Win10MsPinyinSelfStudyTest.cs  - Tests Windows 10 self-study
```

### 12.2 Test Subdirectories

**FilterTest/ (1 file):**
```
AllFilterTest.cs               - Tests all filter types
```

**GeneraterTest/ (6 files):**
```
ErbiTest.cs                    - Tests Erbi code generation
LlmWordRankGeneraterTest.cs    - Tests LLM-based ranking
PinyinTest.cs                  - Tests Pinyin generation
SelfDefiningCodeGeneraterTest.cs - Tests custom code generation
TerraPinyinTest.cs             - Tests Terra Pinyin generation
ZhuyinTest.cs                  - Tests Zhuyin generation
```

**HelperTest/ (4 files):**
```
CollectionHelperTest.cs        - Tests collection utilities
DictionaryHelperTest.cs        - Tests dictionary utilities
FileOperationTest.cs           - Tests file I/O helpers
HttpHelperTest.cs              - Tests HTTP utilities
```

**Total Test Count:** 32 test files + numerous test resources

---

## SUMMARY

**Key Architectural Insights:**

| Aspect | Details |
|--------|---------|
| **Old Architecture** | Monolithic `ImeWlConverterCore` with hardcoded format types, legacy `IWordLibraryImport/Export` interfaces |
| **New Architecture** | Modular design with `ImeWlConverter.Core`, `.Abstractions`, `.Formats` + dependency injection |
| **Dual Support** | Both old and new formats run alongside via adapter pattern (until full migration) |
| **Format Discovery** | CLI: static registration; GUI: runtime reflection |
| **Source Generator** | Automatically discovers formats via `[FormatPlugin]` attribute and generates DI code |
| **Encoding Support** | 36 files register code pages for legacy Windows encodings |
| **Enum Mapping** | Bidirectional mapping between old and new `CodeType` enums |
| **Test Coverage** | 32+ test files covering importers, exporters, filters, code generators |


## 为什么

当前项目仅支持将搜狗细胞词库 `.scel` 文件解析为内部词库对象（`SougouPinyinScel` 只实现了 `IWordLibraryImport`），不支持将词库导出为 `.scel` 格式。用户有大量场景需要将其他输入法词库转换为搜狗细胞词库格式，以便导入搜狗输入法使用。这是社区长期以来的需求，补全此功能可以使搜狗 scel 格式成为完整的双向转换格式。

## 变更内容

- **新增**：在 `SougouPinyinScel` 类上实现 `IWordLibraryExport` 接口，支持将 `WordLibraryList` 导出为符合搜狗细胞词库二进制格式规范的 `.scel` 文件
- **新增**：支持在 CLI 中通过 `-o scel` 参数指定导出为 scel 格式
- **新增**：导出时生成合法的 scel 文件头（magic number、拼音表、词库元信息等），确保搜狗输入法可正常识别并导入
- **修改**：在 `ConsoleRun.LoadImeList()` 中注册 scel 格式的导出能力

## 功能 (Capabilities)

### 新增功能

- `scel-export`: 将内部词库数据序列化为搜狗细胞词库 `.scel` 二进制格式，包括构建拼音索引表、词条数据区、文件头元信息，生成的文件可被搜狗输入法正常导入

### 修改功能

（无现有规范需求变更）

## 影响

- **代码**：`src/ImeWlConverterCore/IME/SougouPinyinScel.cs` — 新增导出逻辑；`src/ImeWlConverterCore/ConsoleRun.cs` — 注册导出格式
- **接口**：`SougouPinyinScel` 类签名变更，新增实现 `IWordLibraryExport`
- **依赖**：无新增外部依赖，使用现有的 `BinFileHelper`、`Encoding` 等基础设施
- **测试**：需新增单元测试验证导出文件的二进制结构正确性，以及集成测试验证 scel → 导入 → 导出 → scel 的往返一致性

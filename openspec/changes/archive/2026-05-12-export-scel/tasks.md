## 1. 接口与基础架构

- [x] 1.1 新增 `IBinaryWordLibraryExport` 接口，定义 `ExportToBinary(WordLibraryList wlList, string outputPath)` 方法
- [x] 1.2 修改 `MainBody` 中的导出逻辑，检测导出目标是否实现 `IBinaryWordLibraryExport`，若是则调用二进制导出路径而非文本写入
- [x] 1.3 在 `ConsoleRun.LoadImeList()` 中为 scel 格式注册导出能力（将 `SougouPinyinScel` 实例添加到导出列表）

## 2. 核心导出实现

- [x] 2.1 在 `SougouPinyinScel` 类上新增 `IWordLibraryExport` 和 `IBinaryWordLibraryExport` 接口实现
- [x] 2.2 实现拼音索引表构建逻辑：从 `WordLibraryList` 中收集所有不重复拼音音节，按字母序排序生成索引映射
- [x] 2.3 实现同音词归组逻辑：将拼音序列相同的词条合并为词组
- [x] 2.4 实现文件头写入：magic number（8字节）、保留区域（填充至 0x011F）
- [x] 2.5 实现统计信息写入：词组数和词条总数写入 0x0120-0x012F
- [x] 2.6 实现元信息区写入：名称、类型、描述、示例词（UTF-16LE，固定区域大小）
- [x] 2.7 实现拼音表写入：拼音条目数 + 逐条写入索引号、字节长度、拼音字符串
- [x] 2.8 实现词条数据区写入：按词组格式写入（词组头 + 拼音索引数组 + 词条列表）
- [x] 2.9 实现 `ExportLine()` 方法抛出不支持异常（与 Import 端的 `ImportLine()` 行为一致）

## 3. 拼音处理

- [x] 3.1 实现拼音标准化逻辑：将带声调、大写等非标准格式转换为小写无声调拼音
- [x] 3.2 实现无拼音词条处理：导出前检测并使用 PinyinGenerater 补全拼音，无法生成时跳过并报错

## 4. 测试

- [x] 4.1 编写单元测试：验证拼音索引表构建的正确性（音节收集、去重、排序）
- [x] 4.2 编写单元测试：验证生成的 scel 文件二进制结构（文件头、统计信息、拼音表、词条区域）
- [x] 4.3 编写往返测试：导出为 scel 后重新导入，验证词条数据一致性（词语和拼音完全匹配）
- [x] 4.4 编写集成测试：通过 CLI `-o scel` 参数导出文件，验证文件可生成且格式正确

#!/bin/bash

# 深蓝词库转换工具 - macOS版本构建脚本

set -e

echo "🚀 开始构建深蓝词库转换工具 macOS版本..."

# 检查.NET是否安装
if ! command -v dotnet &> /dev/null; then
    echo "❌ 错误: 未找到.NET SDK，请先安装.NET 6.0或更高版本"
    echo "下载地址: https://dotnet.microsoft.com/download"
    exit 1
fi

# 显示.NET版本
echo "📋 .NET版本信息:"
dotnet --version

# 清理之前的构建
echo "🧹 清理之前的构建..."
dotnet clean

# 恢复依赖项
echo "📦 恢复NuGet包..."
dotnet restore

# 构建Debug版本
echo "🔨 构建Debug版本..."
dotnet build -c Debug

# 运行测试（如果有的话）
if [ -f "test" ]; then
    echo "🧪 运行测试..."
    dotnet test
fi

# 构建Release版本
echo "🔨 构建Release版本..."
dotnet build -c Release

# 发布自包含版本（Intel Mac）
echo "📱 发布Intel Mac版本..."
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true

# 发布自包含版本（Apple Silicon Mac）
echo "📱 发布Apple Silicon Mac版本..."
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true

# 创建通用二进制文件（如果需要）
echo "🔗 创建发布目录结构..."
mkdir -p dist/intel
mkdir -p dist/apple-silicon

# 从 csproj 中读取 TargetFramework
TFM=$(grep '<TargetFramework>' ImeWlConverterMac.csproj | sed 's/.*<TargetFramework>\(.*\)<\/TargetFramework>.*/\1/')

# 复制发布文件
cp -r bin/Release/${TFM}/osx-x64/publish/* dist/intel/
cp -r bin/Release/${TFM}/osx-arm64/publish/* dist/apple-silicon/

echo "✅ 构建完成！"
echo ""
echo "📁 发布文件位置:"
echo "   Intel Mac: ./dist/intel/"
echo "   Apple Silicon Mac: ./dist/apple-silicon/"
echo ""
echo "🚀 运行方法:"
echo "   Intel Mac: ./dist/intel/ImeWlConverterMac"
echo "   Apple Silicon Mac: ./dist/apple-silicon/ImeWlConverterMac"
echo ""
echo "💡 提示: 首次运行可能需要在系统偏好设置中允许运行未签名的应用程序"
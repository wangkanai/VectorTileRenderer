# VectorTileRenderer - .NET 8.0 Migration

This document explains how to migrate the VectorTileRenderer solution from .NET Framework 4.8 to .NET 8.0.

## Benefits of Migrating to .NET 8.0

- **Performance Improvements**: .NET 8.0 offers significant performance improvements over .NET Framework 4.8
- **Modern C# Features**: Access to the latest C# language features (up to C# 12)
- **Cross-Platform Support**: Potential to run on Linux and macOS with minimal code changes
- **Improved Tooling**: Better IDE integration, hot reload, and other developer productivity features
- **Modern Libraries**: Access to newer and more efficient .NET libraries
- **Better Security**: Enhanced security features and regular security updates

## Migration Steps

### 1. Prepare Your Environment

Make sure you have installed:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended)

### 2. Run the Migration Script

```powershell
./migrate-to-net8.ps1
```

This script will:
- Back up your existing project files
- Replace them with SDK-style .NET 8.0 project files
- Set up a global.json file to target .NET 8.0

### 3. Restore and Build

```powershell
dotnet restore
dotnet build
```

### 4. Fix Any Build Issues

Review the build output for errors. Common issues include:

- Missing NuGet packages
- Obsolete API usage
- Windows-specific code that needs adaptation

### 5. Test the Application

Run the application to test basic functionality:

```powershell
dotnet run --project Static.Demo.WPF
dotnet run --project Gmap.Demo.WinForms
dotnet run --project Mapsui.Demo.WPF
```

### 6. Address Advanced Issues

Follow the detailed instructions in `NET8-MIGRATION-GUIDE.md` for:
- API breaking changes
- Performance optimizations
- Security considerations

## Project Structure Changes

The migration converts the old-style .csproj files to the new SDK-style format:

**Before (partial example):**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{46423799-3B17-4E62-B0AB-5C5A13EE9EB2}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>VectorTileRenderer</RootNamespace>
    <AssemblyName>VectorTileRenderer</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  <!-- Many lines omitted -->
</Project>
```

**After:**
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="protobuf-net" Version="3.2.30" />
    <PackageReference Include="SkiaSharp" Version="2.88.7" />
    <PackageReference Include="SkiaSharp.Views.WPF" Version="2.88.7" />
    <PackageReference Include="System.Data.SQLite.Core" Version="1.0.118" />
  </ItemGroup>

</Project>
```

## Support

For questions or issues with this migration, please open an issue on the GitHub repository.
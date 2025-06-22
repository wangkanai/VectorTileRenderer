# .NET 8.0 Upgrade Plan

## Execution Steps

1. Validate that .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade VectorTileRenderer.csproj
4. Upgrade Gmap.Demo.WinForms.csproj
5. Upgrade Mapsui.Demo.WPF.csproj
6. Upgrade Static.Demo.WPF.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:-------------|:-----------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name | Current Version | New Version | Description |
|:-------------|:---------------:|:-----------:|:------------|
| BruTile | 1.0.0 | 5.0.6 | Recommended for .NET 8.0 |
| Clipper | 6.4.0 | | No supported version found for .NET 8.0 |
| GMap.NET.WindowsForms | 1.7.5 | | No supported version found for .NET 8.0 |
| Mapbox.VectorTile | 1.0.4-alpha2 | | No supported version found for .NET 8.0 |
| Mapsui | 1.3.2 | 4.1.9 | Recommended for .NET 8.0 |
| Microsoft.NETCore.Platforms | 2.0.2 | 8.0.0-preview.7.23375.6 | Recommended for .NET 8.0 |
| Newtonsoft.Json | 9.0.1 | 13.0.3 | Security vulnerability |
| SkiaSharp.Views | 1.60.0 | | No supported version found for .NET 8.0 |
| Stub.System.Data.SQLite.Core.NetFramework | 1.0.115;1.0.115.0 | | No supported version found for .NET 8.0 |
| System.Diagnostics.DiagnosticSource | 4.4.1 | 8.0.1 | Recommended for .NET 8.0 |

### Project upgrade details

#### VectorTileRenderer modifications

Project properties changes:
  - Target framework should be changed from `.NETFramework,Version=v4.8` to `net8.0`
  - Project file needs to be converted to SDK-style

NuGet packages changes:
  - Clipper: No supported version found for .NET 8.0
  - Mapbox.VectorTile: No supported version found for .NET 8.0
  - Microsoft.NETCore.Platforms should be updated from `2.0.2` to `8.0.0-preview.7.23375.6`
  - Newtonsoft.Json should be updated from `9.0.1` to `13.0.3` (*security vulnerability*)
  - SkiaSharp.Views: No supported version found for .NET 8.0
  - Stub.System.Data.SQLite.Core.NetFramework: No supported version found for .NET 8.0
  - System.Diagnostics.DiagnosticSource should be updated from `4.4.1` to `8.0.1`
  - Multiple packages will be removed as their functionality is included in the new framework reference: 
    Microsoft.Win32.Primitives, NETStandard.Library, System.AppContext, System.Collections, System.Collections.Concurrent, System.Console, System.Diagnostics.Debug, System.Diagnostics.Tools, System.Diagnostics.Tracing, System.Globalization, System.Globalization.Calendars, System.IO, System.IO.Compression, System.IO.Compression.ZipFile, System.IO.FileSystem, System.IO.FileSystem.Primitives, System.Linq, System.Linq.Expressions, System.Net.Primitives, System.Net.Sockets, System.ObjectModel, System.Reflection, System.Reflection.Extensions, System.Reflection.Primitives, System.Resources.ResourceManager, System.Runtime, System.Runtime.Extensions, System.Runtime.InteropServices, System.Runtime.InteropServices.RuntimeInformation, System.Runtime.Numerics, System.Security.Cryptography.Algorithms, System.Security.Cryptography.Encoding, System.Security.Cryptography.Primitives, System.Security.Cryptography.X509Certificates, System.Text.Encoding, System.Text.Encoding.Extensions, System.Text.RegularExpressions, System.Threading, System.Threading.Tasks, System.Threading.Timer, System.Xml.ReaderWriter, System.Xml.XDocument

#### Gmap.Demo.WinForms modifications

Project properties changes:
  - Target framework should be changed from `.NETFramework,Version=v4.8` to `net8.0-windows`
  - Project file needs to be converted to SDK-style

NuGet packages changes:
  - GMap.NET.WindowsForms: No supported version found for .NET 8.0
  - Stub.System.Data.SQLite.Core.NetFramework: No supported version found for .NET 8.0

#### Mapsui.Demo.WPF modifications

Project properties changes:
  - Target framework should be changed from `.NETFramework,Version=v4.8` to `net8.0-windows`
  - Project file needs to be converted to SDK-style

NuGet packages changes:
  - BruTile should be updated from `1.0.0` to `5.0.6`
  - Mapsui should be updated from `1.3.2` to `4.1.9`
  - Newtonsoft.Json should be updated from `9.0.1` to `13.0.3` (*security vulnerability*)
  - SkiaSharp.Views: No supported version found for .NET 8.0
  - Stub.System.Data.SQLite.Core.NetFramework: No supported version found for .NET 8.0
  - Multiple packages will be removed as their functionality is included in the new framework reference: 
    System.Collections, System.Diagnostics.Debug, System.Runtime.Extensions, System.Threading

#### Static.Demo.WPF modifications

Project properties changes:
  - Target framework should be changed from `.NETFramework,Version=v4.8` to `net8.0-windows`
  - Project file needs to be converted to SDK-style

NuGet packages changes:
  - Stub.System.Data.SQLite.Core.NetFramework: No supported version found for .NET 8.0
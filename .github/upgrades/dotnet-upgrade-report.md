# .NET 8 Upgrade Report

## Project modifications

| Project name                   | Old Target Framework  | New Target Framework | Commits                   |
|:-------------------------------|:---------------------:|:-------------------:|---------------------------|
| VectorTileRenderer             | .NETFramework,v4.8    | net8.0              | a42902ec                  |
| Mapsui.Demo.WPF               | .NETFramework,v4.8    | net8.0-windows       | 4d6c0036                  |
| Static.Demo.WPF               | .NETFramework,v4.8    | net8.0-windows       |                           |

## NuGet Packages

| Package Name                        | Old Version | New Version | Projects                                 |
|:------------------------------------|:-----------:|:-----------:|------------------------------------------|
| BruTile                             | 1.0.0       | 5.0.6       | Mapsui.Demo.WPF                         |
| Mapsui                              | 1.3.2       | 4.1.9       | Mapsui.Demo.WPF                         |
| Newtonsoft.Json                     | 9.0.1       | 13.0.3      | VectorTileRenderer, Mapsui.Demo.WPF     |
| Microsoft.NETCore.Platforms         | 2.0.2       | 8.0.0-preview.7.23375.6 | VectorTileRenderer         |
| System.Diagnostics.DiagnosticSource | 4.4.1       | 8.0.1       | VectorTileRenderer                      |

## Failed Upgrades

| Project name                 | Reason for Failure                                            |
|:-----------------------------|:--------------------------------------------------------------|
| Gmap.Demo.WinForms           | This project failed to upgrade to .NET 8.0-windows. The GMap.NET.WindowsForms package doesn't have a version compatible with .NET 8.0-windows. |

## All commits

| Commit ID | Description                                |
|:----------|:-------------------------------------------|
| a42902ec  | Refactor SkiaCanvas.cs: Remove unused and commented code |
| 4d6c0036  | Add async tile fetching to VectorMbTilesProvider.cs |

## Next steps

- Investigate alternative approaches for the Gmap.Demo.WinForms project, which could include:
  1. Finding an alternative mapping library that supports .NET 8.0-windows
  2. Forking and updating the GMap.NET.WindowsForms package to work with .NET 8.0-windows
  3. Keeping this project on .NET Framework 4.8 if compatibility with GMap.NET.WindowsForms is essential
- Review and test all upgraded projects thoroughly to ensure proper functionality
- Check for any performance impacts or behavioral changes in the upgraded components
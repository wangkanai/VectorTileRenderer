@echo off
echo Starting migration to .NET 8.0...

:: Create backup folder
set BACKUP_FOLDER=backup-net48-%date:~10,4%%date:~4,2%%date:~7,2%-%time:~0,2%%time:~3,2%%time:~6,2%
set BACKUP_FOLDER=%BACKUP_FOLDER: =0%
mkdir %BACKUP_FOLDER%

:: Backup existing project files
echo Backing up existing project files...
copy "VectorTileRenderer\VectorTileRenderer.csproj" "%BACKUP_FOLDER%\VectorTileRenderer.csproj" /Y
copy "Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj" "%BACKUP_FOLDER%\Gmap.Demo.WinForms.csproj" /Y
copy "Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj" "%BACKUP_FOLDER%\Mapsui.Demo.WPF.csproj" /Y
copy "Static.Demo.WPF\Static.Demo.WPF.csproj" "%BACKUP_FOLDER%\Static.Demo.WPF.csproj" /Y

:: Create new project files with .NET 8.0 content
echo Creating .NET 8.0 project files...

:: VectorTileRenderer.csproj
echo ^<Project Sdk="Microsoft.NET.Sdk"^> > VectorTileRenderer\VectorTileRenderer.csproj
echo. >> VectorTileRenderer\VectorTileRenderer.csproj
echo   ^<PropertyGroup^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<TargetFramework^>net8.0-windows^</TargetFramework^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<UseWPF^>true^</UseWPF^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<GenerateAssemblyInfo^>false^</GenerateAssemblyInfo^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo   ^</PropertyGroup^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo. >> VectorTileRenderer\VectorTileRenderer.csproj
echo   ^<ItemGroup^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<PackageReference Include="Newtonsoft.Json" Version="13.0.3" /^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<PackageReference Include="protobuf-net" Version="3.2.30" /^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<PackageReference Include="SkiaSharp" Version="2.88.7" /^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<PackageReference Include="SkiaSharp.Views.WPF" Version="2.88.7" /^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo     ^<PackageReference Include="System.Data.SQLite.Core" Version="1.0.118" /^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo   ^</ItemGroup^> >> VectorTileRenderer\VectorTileRenderer.csproj
echo. >> VectorTileRenderer\VectorTileRenderer.csproj
echo ^</Project^> >> VectorTileRenderer\VectorTileRenderer.csproj

:: Gmap.Demo.WinForms.csproj
echo ^<Project Sdk="Microsoft.NET.Sdk"^> > Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo. >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^<PropertyGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<OutputType^>WinExe^</OutputType^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<TargetFramework^>net8.0-windows^</TargetFramework^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<UseWindowsForms^>true^</UseWindowsForms^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<UseWPF^>true^</UseWPF^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<GenerateAssemblyInfo^>false^</GenerateAssemblyInfo^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^</PropertyGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo. >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^<ItemGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<PackageReference Include="GMap.NET.Core" Version="2.1.7" /^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<PackageReference Include="GMap.NET.WinForms" Version="2.1.7" /^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^</ItemGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo. >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^<ItemGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo     ^<ProjectReference Include="..\VectorTileRenderer\VectorTileRenderer.csproj" /^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo   ^</ItemGroup^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo. >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj
echo ^</Project^> >> Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj

:: Mapsui.Demo.WPF.csproj
echo ^<Project Sdk="Microsoft.NET.Sdk"^> > Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo. >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^<PropertyGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<OutputType^>WinExe^</OutputType^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<TargetFramework^>net8.0-windows^</TargetFramework^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<UseWPF^>true^</UseWPF^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<GenerateAssemblyInfo^>false^</GenerateAssemblyInfo^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^</PropertyGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo. >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^<ItemGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<PackageReference Include="BruTile" Version="5.0.5" /^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<PackageReference Include="Mapsui" Version="4.1.1" /^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<PackageReference Include="Mapsui.WPF" Version="4.1.1" /^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^</ItemGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo. >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^<ItemGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo     ^<ProjectReference Include="..\VectorTileRenderer\VectorTileRenderer.csproj" /^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo   ^</ItemGroup^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo. >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj
echo ^</Project^> >> Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj

:: Static.Demo.WPF.csproj
echo ^<Project Sdk="Microsoft.NET.Sdk"^> > Static.Demo.WPF\Static.Demo.WPF.csproj
echo. >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo   ^<PropertyGroup^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo     ^<OutputType^>WinExe^</OutputType^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo     ^<TargetFramework^>net8.0-windows^</TargetFramework^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo     ^<UseWPF^>true^</UseWPF^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo     ^<GenerateAssemblyInfo^>false^</GenerateAssemblyInfo^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo   ^</PropertyGroup^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo. >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo   ^<ItemGroup^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo     ^<ProjectReference Include="..\VectorTileRenderer\VectorTileRenderer.csproj" /^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo   ^</ItemGroup^> >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo. >> Static.Demo.WPF\Static.Demo.WPF.csproj
echo ^</Project^> >> Static.Demo.WPF\Static.Demo.WPF.csproj

:: Create global.json
echo Creating global.json...
echo { > global.json
echo   "sdk": { >> global.json
echo     "version": "8.0.100", >> global.json
echo     "rollForward": "latestFeature" >> global.json
echo   } >> global.json
echo } >> global.json

echo Migration to .NET 8.0 completed!
echo Backup of original files created in: %BACKUP_FOLDER%
echo Please run 'dotnet restore' and 'dotnet build' to verify the migration.
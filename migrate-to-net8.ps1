Write-Host "Starting migration to .NET 8.0..." -ForegroundColor Green

# Create backup folder
$backupFolder = "backup-net48-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
New-Item -Path $backupFolder -ItemType Directory -Force

# Backup existing project files
Write-Host "Backing up existing project files..." -ForegroundColor Cyan
Copy-Item "VectorTileRenderer\VectorTileRenderer.csproj" -Destination "$backupFolder\VectorTileRenderer.csproj" -ErrorAction SilentlyContinue
Copy-Item "Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj" -Destination "$backupFolder\Gmap.Demo.WinForms.csproj" -ErrorAction SilentlyContinue
Copy-Item "Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj" -Destination "$backupFolder\Mapsui.Demo.WPF.csproj" -ErrorAction SilentlyContinue
Copy-Item "Static.Demo.WPF\Static.Demo.WPF.csproj" -Destination "$backupFolder\Static.Demo.WPF.csproj" -ErrorAction SilentlyContinue

# Replace project files with new SDK-style .NET 8.0 projects
Write-Host "Replacing project files with .NET 8.0 versions..." -ForegroundColor Cyan
Copy-Item "VectorTileRenderer.csproj.new" -Destination "VectorTileRenderer\VectorTileRenderer.csproj" -Force
Copy-Item "Gmap.Demo.WinForms.csproj.new" -Destination "Gmap.Demo.WinForms\Gmap.Demo.WinForms.csproj" -Force
Copy-Item "Mapsui.Demo.WPF.csproj.new" -Destination "Mapsui.Demo.WPF\Mapsui.Demo.WPF.csproj" -Force
Copy-Item "Static.Demo.WPF.csproj.new" -Destination "Static.Demo.WPF\Static.Demo.WPF.csproj" -Force

# Clean up temporary files
Write-Host "Cleaning up..." -ForegroundColor Cyan
Remove-Item "*.new" -ErrorAction SilentlyContinue

# Update app.config files to support .NET 8.0
Get-ChildItem -Path . -Include "app.config" -Recurse | ForEach-Object {
    $configPath = $_.FullName
    $configBackup = "$backupFolder\$(Split-Path -Leaf $configPath)"
    
    # Backup the file
    Copy-Item $configPath -Destination $configBackup -ErrorAction SilentlyContinue
    
    # Update to .NET 8.0 compatible config if needed
    # This is a simplified approach - complex config files might need more adjustments
    $content = Get-Content $configPath -Raw -ErrorAction SilentlyContinue
    if ($content -match "supportedRuntime") {
        $content = $content -replace '<supportedRuntime version="[^"]*"', '<supportedRuntime version="v8.0"'
        Set-Content -Path $configPath -Value $content
    }
}

# Create a global.json file to specify .NET 8.0 SDK
$globalJsonContent = @"
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestFeature"
  }
}
"@
Set-Content -Path "global.json" -Value $globalJsonContent

Write-Host "Migration to .NET 8.0 completed!" -ForegroundColor Green
Write-Host "Backup of original files created in: $backupFolder" -ForegroundColor Yellow
Write-Host "Please run 'dotnet restore' and 'dotnet build' to verify the migration." -ForegroundColor Yellow
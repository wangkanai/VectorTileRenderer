# VectorTileRenderer Migration to .NET 8.0

This document describes the migration process from .NET Framework 4.8 to .NET 8.0 for the VectorTileRenderer solution.

## Automated Migration

The `migrate-to-net8.ps1` script performs the following operations:

1. Creates a backup of the original project files
2. Replaces all project files with SDK-style .NET 8.0 versions
3. Updates app.config files if present
4. Creates a global.json file to specify the .NET 8.0 SDK

## Manual Steps After Migration

After running the script, you may need to perform these additional steps:

### 1. Check NuGet Package References

Some packages may need to be manually updated to versions compatible with .NET 8.0:

```
dotnet restore
```

If you see package compatibility issues, update them to the newest compatible versions.

### 2. Fix API Breaking Changes

Some .NET Framework APIs are not available in .NET 8.0 or work differently:

- `System.Drawing.Common` has platform limitations - use alternative drawing APIs when possible
- `System.Windows.Forms` differs in some behaviors in .NET 8.0
- WPF rendering behavior might differ slightly

The `VectorTileRenderer.Compatibility.Net8Compatibility` class provides some helpers for common issues.

### 3. Address File/Path Issues

.NET 8.0 has improved path handling, which might cause issues with:

- Relative paths that worked in .NET Framework
- Case-sensitive path handling in cross-platform scenarios

### 4. Update CI/CD Pipelines

If you're using CI/CD pipelines, update them to use .NET 8.0 SDK.

### 5. Performance Profiling

.NET 8.0 has different performance characteristics than .NET Framework 4.8:

- JIT compilation behavior differs
- Memory allocation patterns are different
- Some operations may be faster/slower

Run performance tests to compare the migrated application against the original.

## Known Issues

### SkiaSharp Integration

The SkiaSharp integration might require updates for .NET 8.0 compatibility. Check the following:

- Event handling in rendering paths
- GPU acceleration features
- Memory management patterns

### WPF/WinForms Interop

Some interop between WPF and Windows Forms may require adjustments:

- Cross-thread UI operations
- BitmapSource/Image conversions

### Security Changes

.NET 8.0 has enhanced security settings:

- More restrictive file access permissions by default
- Stricter validation of inputs and outputs

## Testing Strategy

1. Run unit tests if available
2. Test map rendering with different datasets
3. Verify UI responsiveness with larger maps
4. Check memory consumption during extended operations

## Rollback Procedure

If needed, you can revert to the original .NET Framework 4.8 version:

1. Copy the backup project files back to their original locations
2. Delete the global.json file
3. Restore NuGet packages
4. Rebuild the solution
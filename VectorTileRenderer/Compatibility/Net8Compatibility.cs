using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VectorTileRenderer.Compatibility
{
    /// <summary>
    /// Provides compatibility helpers for the migration from .NET Framework 4.8 to .NET 8.0
    /// </summary>
    public static class Net8Compatibility
    {
        /// <summary>
        /// Helper method to safely load a bitmap from a file with cross-.NET compatibility
        /// </summary>
        public static BitmapSource SafeLoadBitmap(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    return decoder.Frames[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading bitmap: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Helper method to create an awaitable Task from a potentially non-Task-based operation
        /// </summary>
        public static Task<T> ToTask<T>(Func<T> function)
        {
            try
            {
                return Task.FromResult(function());
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<T>();
                tcs.SetException(ex);
                return tcs.Task;
            }
        }
        
        /// <summary>
        /// Helper for synchronization context issues that might arise in .NET 8
        /// </summary>
        public static async Task<T> RunOnThreadPoolAsync<T>(Func<T> function)
        {
            return await Task.Run(() => function());
        }
    }
}
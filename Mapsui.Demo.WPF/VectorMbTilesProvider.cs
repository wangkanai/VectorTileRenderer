using BruTile;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VectorTileRenderer;

namespace Mapsui.Demo.WPF
{
    class VectorMbTilesProvider : ITileProvider
    {
        Style style;
        VectorTileRenderer.Sources.MbTilesSource provider;
        string cachePath;

        public VectorMbTilesProvider(string path, string stylePath, string cachePath)
        {
            this.cachePath = cachePath;
            style = new Style(stylePath);
            style.FontDirectory = @"styles/fonts/";

            provider = new VectorTileRenderer.Sources.MbTilesSource(path);
            style.SetSourceProvider("openmaptiles", provider);
        }
        
        public byte[] GetTile(TileInfo tileInfo)
        {
            try
            {
                var canvas = new SkiaCanvas();
                var bitmapSource = Renderer.RenderCached(cachePath, style, canvas, (int)tileInfo.Index.Col, (int)tileInfo.Index.Row, Convert.ToInt32(tileInfo.Index.Level), 256, 256, 1).Result;
                return GetBytesFromBitmapSource(bitmapSource);
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetTileAsync(TileInfo tileInfo)
        {
            // Implement asynchronous version
            return await Task.FromResult(GetTile(tileInfo));
        }

        static byte[] GetBytesFromBitmapSource(BitmapSource bmp)
        {
            if(bmp == null)
            {
                return null;
            }

            var encoder = new PngBitmapEncoder();
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(stream);
                byte[] bit = stream.ToArray();
                return bit;
            }
        }
    }
}

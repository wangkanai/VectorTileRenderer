using BruTile;
using BruTile.Predefined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorTileRenderer;

namespace Mapsui.Demo.WPF
{
    class VectorMbTilesSource : BruTile.ITileSource
    {
        VectorMbTilesProvider provider;

        public ITileSchema Schema { get; }
        public string Name { get; } = "VectorMbTileSource";
        public Attribution Attribution { get; } = new Attribution();

        public VectorMbTilesSource(string path, string stylePath, string cachePath)
        {
            Schema = GetTileSchema();
            provider = new VectorMbTilesProvider(path, stylePath, cachePath);
        }

        public static ITileSchema GetTileSchema()
        {
            var schema = new GlobalSphericalMercator(YAxis.TMS);
            return schema;
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            return provider.GetTile(tileInfo);
        }

        public async Task<byte[]> GetTileAsync(TileInfo tileInfo)
        {
            // Implement asynchronous version
            return await Task.FromResult(provider.GetTile(tileInfo));
        }

        public ITileProvider Provider
        {
            get { return provider; }
        }
    }
}

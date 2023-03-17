using GoRogue;
using GoRogue.SpatialMaps;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Utils.Extensions
{
    public static class MapExtensions
    {
        public static IEnumerable<IReadOnlySpatialMap<T>> CorrectGetLayersInMask<T>(
            this IReadOnlyLayeredSpatialMap<T> self, uint layerMask = uint.MaxValue) where T : IHasLayer
        {
            foreach (var num in self.LayerMasker.Layers(layerMask >> self.StartingLayer))
                yield return self.GetLayer(num + self.StartingLayer);
        }

        public static IReadOnlySpatialMap<T> UnrollSpatialMap<T>(this IEnumerable<IReadOnlySpatialMap<T>> self)
            where T : class, IHasID
        {
            var map = new SpatialMap<T>();
            foreach (var layer in self)
            {
                foreach (var item in layer)
                {
                    map.Add(item.Item, item.Position);
                }
            }
            return map;
        }
    }
}

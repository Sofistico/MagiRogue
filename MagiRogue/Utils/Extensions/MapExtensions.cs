using GoRogue;
using GoRogue.SpatialMaps;
using System.Collections.Generic;

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
    }
}

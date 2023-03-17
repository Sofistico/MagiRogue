using GoRogue;
using GoRogue.SpatialMaps;
using System;
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

        public static IEnumerable<T> UnrollSpatialMap<T>(this IEnumerable<IReadOnlySpatialMap<T>> self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            foreach (var layer in self)
            {
                foreach (var item in layer)
                    yield return item.Item;
            }
        }
    }
}

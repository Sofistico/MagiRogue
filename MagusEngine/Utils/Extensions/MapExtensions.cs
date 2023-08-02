using GoRogue;
using SadRogue.Primitives.SpatialMaps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Utils.Extensions
{
    public static class MapExtensions
    {
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

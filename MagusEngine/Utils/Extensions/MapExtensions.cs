using SadRogue.Primitives.SpatialMaps;
using System;
using System.Collections.Generic;

namespace MagusEngine.Utils.Extensions
{
    public static class MapExtensions
    {
        public static IEnumerable<T> UnrollSpatialMap<T>(this IEnumerable<IReadOnlySpatialMap<T>> self)
            where T : notnull
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return UnrollSpatialMapInlineMethod();

            IEnumerable<T> UnrollSpatialMapInlineMethod()
            {
                foreach (var layer in self)
                {
                    foreach (var item in layer)
                        yield return item.Item;
                }
            }
        }
    }
}

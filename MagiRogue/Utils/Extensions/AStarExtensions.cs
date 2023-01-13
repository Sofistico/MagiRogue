﻿using GoRogue.Pathing;
using SadRogue.Primitives;

namespace MagiRogue.Utils.Extensions
{
    public static class AStarExtensions
    {
        public static Path ShortestPath(this AStar aStar, Point originCoord, Point endCoord,
            Distance distanceMeasurement,
            bool assumeEndPointWalkable = true)
        {
            Distance curretnDistanceMeasure = aStar.DistanceMeasurement;
            aStar.DistanceMeasurement = distanceMeasurement;
            Path path = aStar.ShortestPath(originCoord, endCoord, assumeEndPointWalkable);
            aStar.DistanceMeasurement = curretnDistanceMeasure;
            return path;
        }
    }
}
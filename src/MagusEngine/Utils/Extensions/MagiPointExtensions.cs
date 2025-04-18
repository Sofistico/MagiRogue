﻿using GoRogue.GameFramework;
using GoRogue.Random;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Utils.Extensions
{
    public static class MagiPointExtensions
    {
        public static float PointMagnitude(this Point point)
        {
            if (point.X < 0 || point.Y < 0)
                return 0;
            return MathF.Sqrt((point.X * point.X) + (point.Y * point.Y));
        }

        public static IEnumerable<Point> GetTileLocationsAlongLine(this int xOrigin,
            int yOrigin,
            int xDestination,
            int yDestination)
        {
            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    xOrigin += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    yOrigin += sy;
                }
            }
        }

        /// <summary>
        /// Returns a list of points expressing the perimeter of a rectangle
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public static List<Point> GetBorderCellLocations(this Rectangle room)
        {
            //establish room boundaries
            int xMin = room.MinExtentX;
            int xMax = room.MaxExtentX;
            int yMin = room.MinExtentY;
            int yMax = room.MaxExtentY;

            // build a list of room border cells using a series of straight lines
            List<Point> borderCells = GetTileLocationsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        public static Point GetPointNextTo(this Point point)
        {
            Direction[] directions = new Direction[]
            {
                Direction.Down,
                Direction.Up,
                Direction.Left,
                Direction.Right,
            };

            Direction directionToGo = directions.GetRandomItemFromList();
            return new Point(point.X + directionToGo.DeltaX, point.Y + directionToGo.DeltaY);
        }

        public static Point GetPointNextToWithCardinals()
        {
            Direction[] directions = new Direction[]
            {
                Direction.Down,
                Direction.Up,
                Direction.Left,
                Direction.Right,
                Direction.DownLeft,
                Direction.UpLeft,
                Direction.DownRight,
                Direction.UpRight
            };

            Direction directionToGo = directions.GetRandomItemFromList();
            return new Point(directionToGo.DeltaX, directionToGo.DeltaY);
        }

        public static Point FindClosest(this Point point, Point[] pointsToSearch)
        {
            Point closestPoint = Point.None;
            double previousDistance = 999;
            for (int i = 0; i < pointsToSearch.Length; i++)
            {
                Point possibleClosestPoint = pointsToSearch[i];
                double distanceBeetweenPoints = Distance.Chebyshev.Calculate(possibleClosestPoint, point);
                if (distanceBeetweenPoints < previousDistance)
                {
                    closestPoint = possibleClosestPoint;
                    previousDistance = distanceBeetweenPoints;
                }
            }
            return closestPoint;
        }

        public static Point FindClosest(this Point point, IEnumerable<Point> pointsToSearch)
        {
            Point closestPoint = Point.None;
            double previousDistance = 999;
            foreach (var possibleClosestPoint in pointsToSearch)
            {
                double distanceBeetweenPoints = Distance.Chebyshev.Calculate(possibleClosestPoint, point);
                if (distanceBeetweenPoints < previousDistance)
                {
                    closestPoint = possibleClosestPoint;
                    previousDistance = distanceBeetweenPoints;
                }
            }
            return closestPoint;
        }

        public static (Point, double) FindClosestAndReturnDistance(this Point point, Point[] pointsToSearch)
        {
            Point closestPoint = Point.None;
            double previousDistance = 999;
            for (int i = 0; i < pointsToSearch.Length; i++)
            {
                Point possibleClosestPoint = pointsToSearch[i];
                double distanceBeetweenPoints = Distance.Chebyshev.Calculate(possibleClosestPoint, point);
                if (distanceBeetweenPoints < previousDistance)
                {
                    closestPoint = possibleClosestPoint;
                    previousDistance = distanceBeetweenPoints;
                }
            }
            return (closestPoint, previousDistance);
        }

        public static (Point, double) FindClosestAndReturnDistance(this Point point, IEnumerable<Point> pointsToSearch)
        {
            Point closestPoint = Point.None;
            double previousDistance = 999;
            foreach (var possibleClosestPoint in pointsToSearch)
            {
                double distanceBeetweenPoints = Distance.Chebyshev.Calculate(possibleClosestPoint, point);
                if (distanceBeetweenPoints < previousDistance)
                {
                    closestPoint = possibleClosestPoint;
                    previousDistance = distanceBeetweenPoints;
                }
            }
            return (closestPoint, previousDistance);
        }

        public static Point GetRandomCoordinateWithinSquareRadius(this Point center, int squareSize, bool matchXLength = true)
        {
            int halfSquareSize = squareSize / 2;
            int x;
            var rand = GlobalRandom.DefaultRNG;
            int y = rand.NextInt(center.Y - halfSquareSize, center.Y + halfSquareSize);

            if (matchXLength)
                x = rand.NextInt(center.X - squareSize, center.X + squareSize);
            else
                x = rand.NextInt(center.X - halfSquareSize, center.X + halfSquareSize);

            return new Point(x, y);
        }

        public static T? GetClosest<T>(this Point originPos, int range, List<T> listT, IGameObject? caller) where T : IGameObject
        {
            T? closest = default;
            double bestDistance = double.MaxValue;

            IGameObjIterator(originPos, range, listT, caller, ref closest!, ref bestDistance);

            return closest;
        }

        private static void IGameObjIterator<T>(Point originPos,
            int range,
            ICollection<T> listT,
            IGameObject? caller,
            ref T closest,
            ref double bestDistance) where T : IGameObject
        {
            foreach (T t in listT)
            {
                if (t is null) continue;
                if (caller != null && t.ID != caller.ID)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }
        }

        public static T? GetClosest<T>(this Point originPos, int range, T[] listT, IGameObject? caller) where T : IGameObject
        {
            T? closest = default;
            double bestDistance = double.MaxValue;

            IGameObjIterator(originPos, range, listT, caller, ref closest!, ref bestDistance);

            return closest;
        }

        public static (T?, double) GetClosestAndDistance<T>(this Point originPos, int range, T[] listT, IGameObject? caller) where T : IGameObject
        {
            T? closest = default;
            double bestDistance = double.MaxValue;

            IGameObjIterator(originPos, range, listT, caller, ref closest!, ref bestDistance);

            return (closest, bestDistance);
        }

        public static double GetDistance(this Point originPos, Point posToCompare)
        {
            return Distance.Chebyshev.Calculate(originPos, posToCompare);
        }
    }
}

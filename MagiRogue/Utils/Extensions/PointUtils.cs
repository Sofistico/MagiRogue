using GoRogue.GameFramework;
using MagiRogue.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Utils.Extensions
{
    public static class PointUtils
    {
        public static float PointMagnitude(this Point point) =>
            MathF.Sqrt((point.X * point.X) + (point.Y * point.Y));

        public static IEnumerable<Point> GetTileLocationsAlongLine(int xOrigin,
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
        public static List<Point> GetBorderCellLocations(Rectangle room)
        {
            //establish room boundaries
            int xMin = room.ToMonoRectangle().Left;
            int xMax = room.ToMonoRectangle().Right;
            int yMin = room.ToMonoRectangle().Bottom;
            int yMax = room.ToMonoRectangle().Top;

            // build a list of room border cells using a series of
            // straight lines
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
            return new SadRogue.Primitives.Point(point.X + directionToGo.DeltaX, point.Y + directionToGo.DeltaY);
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
            return new SadRogue.Primitives.Point(directionToGo.DeltaX, directionToGo.DeltaY);
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
            var rand = GameLoop.GlobalRand;
            int y = rand.NextInt(center.Y - halfSquareSize, center.Y + halfSquareSize);

            if (matchXLength)
                x = rand.NextInt(center.X - squareSize, center.X + squareSize);
            else
                x = rand.NextInt(center.X - halfSquareSize, center.X + halfSquareSize);

            return new Point(x, y);
        }

        public static T? GetClosest<T>(this Point originPos, int range, List<T> listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;

            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return closest;
        }

        public static T? GetClosest<T>(this Point originPos, int range, T[] listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;

            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return closest;
        }

        public static (T?, double) GetClosestAndDistance<T>(this Point originPos, int range, T[] listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;
            double finalDistance = 0;
            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);
                    finalDistance = distance;
                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return (closest, finalDistance);
        }
    }
}

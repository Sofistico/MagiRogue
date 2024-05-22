using GoRogue.FOV;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MagusEngine.Utils
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Returns a cone
        /// </summary>
        /// <param name="originCoordinate">Where the cone begins</param>
        /// <param name="radius">The radius of the cone</param>
        /// <param name="target">The target that is creating the cone</param>
        /// <returns></returns>
        public static Shape Cone(this Point originCoordinate,
            double radius,
            Target target,
            double coneSpan = 90,
            bool ignoresWalls = false)
        {
            double angle = 0;
            if (target.TravelPath.Length > 0)
            {
                angle = Point.BearingOfLine(target.TravelPath.GetStep(0) - originCoordinate);
            }
            RecursiveShadowcastingFOV coneFov;
            if (!ignoresWalls)
            {
                coneFov = new(Find.CurrentMap.TransparencyView);
            }
            else
            {
                var arr = new bool[Find.CurrentMap.Width * Find.CurrentMap.Height];
                Array.Fill(arr, true);
                var boolArray = new ArrayView<bool>(arr, Find.CurrentMap.Width);
                coneFov = new(boolArray);
            }

            coneFov.Calculate(target.TravelPath.GetStep(0).X, target.TravelPath.GetStep(0).Y, radius, Distance.Euclidean, angle, coneSpan);

            return new Shape(coneFov.CurrentFOV);
        }

        private static IEnumerable<Point> GetConePointsFromRadius(Point origin, Point target, double radius, double span)
        {
            var apex = GetApexFromRadius(origin, target, radius, Distance.Chebyshev);
            return GetConePoints(origin, apex, span);
        }

        private static Point GetApexFromRadius(Point origin, Point target, double radius, Distance distanceCalculation)
        {
            Point start = origin;

            var end = target;

            while (distanceCalculation.Calculate(origin, end) < radius)
                end += target - origin;

            if (distanceCalculation.Calculate(origin, end).Equals(radius))
                return end;

            // Go back until we find the right point
            foreach (var point in Lines.GetBresenhamLine(end, start))
            {
                if (distanceCalculation.Calculate(origin, point) < radius) // Last point was the right one
                    return end;

                end = point;
            }

            throw new UnreachableException();
        }

        /// <summary>
        /// Generates the 3 points of a triangle that defines a cone starting from "origin", facing toward "facingPoint", with the specified span.
        /// </summary>
        /// <param name="origin">The origin point of the cone</param>
        /// <param name="facingPoint">The point at which the cone is facing.</param>
        /// <param name="span">
        /// The angle, in degrees, that specifies the full arc contained in the cone --
        /// <paramref name="span"/> / 2 degrees are included on either side of the cone's center line.
        /// </param>
        /// <returns>The three points defining the triangle.</returns>
        private static IEnumerable<Point> GetConePoints(Point origin, Point facingPoint, double span)
        {
            // Normalize span
            span %= 360;

            // Get the vector from "origin" to "facingPoint"
            var vec = facingPoint - origin;
            // Get a perpendicular (tangent) vector
            var tangentVec = new Point(vec.Y, -vec.X);
            // Get the half-span of the cone
            double halfSpanRadians = SadRogue.Primitives.MathHelpers.ToRadian(span / 2);

            // Get the scale of the tangent vector
            double scale = Math.Tan(halfSpanRadians);
            // Scale the tangent vector by the scale
            var scaledTangentVec = tangentVec * scale;

            // Get the left and right points of the cone
            var left = facingPoint - scaledTangentVec;
            var right = facingPoint + scaledTangentVec;

            // Return the points of the cone
            yield return origin;
            yield return left;
            yield return right;
        }

        public static Shape HollowCircleFromOriginPoint(this Point origin, int radius)
        {
            return new Shape(Shapes.GetCircle(origin, radius));
        }

        public static Shape CircleFromOriginPoint(this Point origin, int radius)
        {
            return new Shape(Radius.Circle.PositionsInRadius(origin, radius));
        }

        public static bool PointInsideACircle(this Point center, Point point, double radius)
        {
            double dx = center.X - point.X;
            double dy = center.Y - point.Y;
            double distance = (dx * dx) + (dy * dy); // little optmization, rather than get the square root, i just put
            // distance^2<=radius^2
            return distance <= radius * radius;
        }
    }

    public readonly struct Shape
    {
        public readonly IEnumerable<Point> Points { get; }

        public Shape(IEnumerable<Point> points)
        {
            Points = points;
        }
    }

    /// <summary>
    /// Represents a simple 2d grid, does not hold any kind of objects and is there just to
    /// represent the space
    /// </summary>
    public struct Simple2DGrid
    {
        /// <summary>
        /// Also know as the rows
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Also know as the columns
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The total size of the grid
        /// </summary>
        public int Size { get; set; }

        public List<Point> Points { get; set; }

        public Simple2DGrid(int width, int height, List<Point> points)
        {
            Width = width;
            Height = height;
            Size = width * height;
            Points = points;
        }
    }

    /// <summary>
    /// Represents a simple 3d grid, does not hold any kind of objects and is there just to
    /// represent the space
    /// </summary>
    public struct Simple3DGrid
    {
        /// <summary>
        /// Also know as the rows
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Also know as the columns
        /// </summary>
        public int Height { get; set; }

        public int Depth { get; set; }

        /// <summary>
        /// The total size of the grid
        /// </summary>
        public int Size { get; set; }

        public Simple3DGrid(int width, int height, int depth)
        {
            Width = width;
            Height = height;
            Size = width * height * depth;
        }

        public readonly int ToIndex(int y, int x, int z)
        {
            return z + (y * Depth) + (x * Height * Depth);
        }
    }
}

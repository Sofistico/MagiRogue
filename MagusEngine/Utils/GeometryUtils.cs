using MagusEngine.Bus.UiBus;
using MagusEngine.Commands;
using MagusEngine.Services;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

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
        public static Shape Cone(this Point originCoordinate, double radius, Target target, double coneSpan = 90)
        {
            var map = GameLoop.GetCurrentMap();
            Point[] points;
            double angle = 0;
            if (target.TravelPath.Length > 0)
            {
                angle = Point.BearingOfLine(target.TravelPath.GetStep(0) - originCoordinate);
            }

            GoRogue.FOV.RecursiveShadowcastingFOV coneFov =
                new(map.TransparencyView);

            coneFov.Calculate(originCoordinate.X, originCoordinate.Y,
                radius, Distance.Chebyshev, angle, coneSpan);

            points = coneFov.CurrentFOV.ToArray();
#if DEBUG
            Locator.GetService<MessageBusService>().SendMessage<MessageSent>(new(angle.ToString()));
#endif
            return new Shape(points);
        }

        public static Shape HollowCircleFromOriginPoint(this Point origin, int radius)
        {
            return new Shape(Shapes.GetCircle(origin, radius).ToArray());
        }

        public static Shape CircleFromOriginPoint(this Point origin, int radius)
        {
            return new Shape(Radius.Circle.PositionsInRadius(origin, radius).ToArray());
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
        public readonly Point[] Points { get; }

        public Shape(Point[] points)
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

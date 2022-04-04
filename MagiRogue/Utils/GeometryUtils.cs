using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GoRogue.Pathing;
using MagiRogue.Commands;
using SadRogue.Primitives;

namespace MagiRogue.Utils
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
        public static Shape Cone(Point originCoordinate, double radius, Target target)
        {
            var map = GameLoop.GetCurrentMap();
            Point[] points;
            double angle = 0;
            if (target.TravelPath.Length > 0)
            {
                angle = Point.BearingOfLine(originCoordinate, target.TravelPath.GetStep(0));
                // Is a hacky way to ensure the angle goes from right from 0, cince bearing of line considers
                // up as the 0 point.
                angle = (angle + 90) % 360;
            }

            GoRogue.FOV.RecursiveShadowcastingFOV coneFov =
                new(map.TransparencyView);
            coneFov.Calculate(originCoordinate.X, originCoordinate.Y,
                radius, map.DistanceMeasurement, angle, 160);

            points = coneFov.CurrentFOV.ToArray();
#if DEBUG
            GameLoop.UIManager.MessageLog.Add(angle.ToString());
#endif
            return new Shape(points, radius);
        }
    }

    public struct Shape
    {
        public readonly Point[] Points { get; }

        public readonly double Radius { get; }

        public Shape(Point[] points, double radius)
        {
            Points = points;
            Radius = radius;
        }
    }

    /// <summary>
    /// Represents a simple 2d grid, does not hold any kind of objects and is there just to represent
    /// the space
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
}
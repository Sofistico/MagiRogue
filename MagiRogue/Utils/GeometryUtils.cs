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
            var map = GameLoop.World.CurrentMap;
            Point[] points;
            double angle = 0;
            if (target.TravelPath.Length > 0)
            {
                angle = Point.BearingOfLine(originCoordinate, target.TravelPath.GetStep(0));
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
        public Point[] Points { get; set; }

        public double Radius { get; set; }

        public Shape(Point[] points, double radius)
        {
            Points = points;
            Radius = radius;
        }
    }
}
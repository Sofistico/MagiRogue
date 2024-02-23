using SadRogue.Primitives;

namespace MagusEngine.Utils.Extensions
{
    public static class DirectionExtensions
    {
        public static Point TransformToPointFromOrigin(this Direction dir, Point originPoint, int distance)
        {
            var pointDir = new Point(dir.DeltaX * distance, dir.DeltaY * distance);
            return originPoint + pointDir;
        }
    }
}

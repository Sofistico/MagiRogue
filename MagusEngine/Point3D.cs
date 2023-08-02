namespace MagusEngine
{
    public struct Point3D
    {
        private Point internalPoint;

        public int X { get => internalPoint.X; set => internalPoint = internalPoint.WithX(value); }
        public int Y { get => internalPoint.Y; set => internalPoint = internalPoint.WithY(value); }
        public int Z { get; set; }

        public Point3D(Point point)
        {
            internalPoint = point;
            Z = 0;
        }

        public Point3D(Point point, int z)
        {
            internalPoint = point;
            Z = z;
        }

        public Point InternalPoint() => internalPoint;

        public int ToIndex(int width, int height) => X + Y * width + Z * width * height;
    }
}

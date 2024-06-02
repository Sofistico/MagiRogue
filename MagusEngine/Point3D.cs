namespace MagusEngine
{
    public struct Point3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3D(int x, int y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Point point) : this(point.X, point.Y)
        {
        }

        public Point3D(Point point, int z) : this(point.X, point.Y, z)
        {
        }

        public readonly int ToIndex(int width, int height) => X + (Y * width) + (Z * width * height);

        public static implicit operator Point(Point3D point)
        {
            return new Point(point.X, point.Y);
        }

        public static implicit operator Point3D(Point point)
        {
            return new Point3D(point.X, point.Y);
        }
    }
}

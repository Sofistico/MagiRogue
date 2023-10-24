namespace MagusEngine
{
    public struct Point3D
    {
        private Point internalPoint;

        public int X { readonly get => internalPoint.X; set => internalPoint = internalPoint.WithX(value); }
        public int Y { readonly get => internalPoint.Y; set => internalPoint = internalPoint.WithY(value); }
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

        public readonly Point InternalPoint() => internalPoint;

        public readonly int ToIndex(int width, int height) => X + (Y * width) + (Z * width * height);
    }
}

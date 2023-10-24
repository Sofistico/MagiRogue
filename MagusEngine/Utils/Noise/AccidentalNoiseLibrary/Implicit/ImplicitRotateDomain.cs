using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitRotateDomain : ImplicitModuleBase
    {
        private readonly double[,] rotationMatrix = new double[3, 3];

        public ImplicitRotateDomain(ImplicitModuleBase source, double x, double y, double z, double angle)
        {
            Source = source;
            X = new ImplicitConstant(x);
            Y = new ImplicitConstant(y);
            Z = new ImplicitConstant(z);
            Angle = new ImplicitConstant(angle);
        }

        public ImplicitModuleBase Source { get; set; }

        public ImplicitModuleBase X { get; set; }

        public ImplicitModuleBase Y { get; set; }

        public ImplicitModuleBase Z { get; set; }

        public ImplicitModuleBase Angle { get; set; }

        public void SetAxis(double x, double y, double z)
        {
            X = new ImplicitConstant(x);
            Y = new ImplicitConstant(y);
            Z = new ImplicitConstant(z);
        }

        public override double Get(double x, double y)
        {
            var d = Angle.Get(x, y) * 360.0 * 3.14159265 / 180.0;
            var cos2D = Math.Cos(d);
            var sin2D = Math.Sin(d);
            var nx = x * cos2D - y * sin2D;
            var ny = y * cos2D + x * sin2D;
            return Source.Get(nx, ny);
        }

        public override double Get(double x, double y, double z)
        {
            CalculateRotMatrix(x, y, z);
            var nx = rotationMatrix[0, 0] * x + rotationMatrix[1, 0] * y + rotationMatrix[2, 0] * z;
            var ny = rotationMatrix[0, 1] * x + rotationMatrix[1, 1] * y + rotationMatrix[2, 1] * z;
            var nz = rotationMatrix[0, 2] * x + rotationMatrix[1, 2] * y + rotationMatrix[2, 2] * z;
            return Source.Get(nx, ny, nz);
        }

        public override double Get(double x, double y, double z, double w)
        {
            CalculateRotMatrix(x, y, z, w);
            var nx = rotationMatrix[0, 0] * x + rotationMatrix[1, 0] * y + rotationMatrix[2, 0] * z;
            var ny = rotationMatrix[0, 1] * x + rotationMatrix[1, 1] * y + rotationMatrix[2, 1] * z;
            var nz = rotationMatrix[0, 2] * x + rotationMatrix[1, 2] * y + rotationMatrix[2, 2] * z;
            return Source.Get(nx, ny, nz, w);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            CalculateRotMatrix(x, y, z, w, u, v);
            var nx = rotationMatrix[0, 0] * x + rotationMatrix[1, 0] * y + rotationMatrix[2, 0] * z;
            var ny = rotationMatrix[0, 1] * x + rotationMatrix[1, 1] * y + rotationMatrix[2, 1] * z;
            var nz = rotationMatrix[0, 2] * x + rotationMatrix[1, 2] * y + rotationMatrix[2, 2] * z;
            return Source.Get(nx, ny, nz, w, u, v);
        }

        private void CalculateRotMatrix(double x, double y)
        {
            var angle = Angle.Get(x, y) * 360.0 * Math.PI / 180.0;
            var ax = X.Get(x, y);
            var ay = Y.Get(x, y);
            var az = Z.Get(x, y);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(double x, double y, double z)
        {
            var angle = Angle.Get(x, y, z) * 360.0 * Math.PI / 180.0;
            var ax = X.Get(x, y, z);
            var ay = Y.Get(x, y, z);
            var az = Z.Get(x, y, z);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(double x, double y, double z, double w)
        {
            var angle = Angle.Get(x, y, z, w) * 360.0 * Math.PI / 180.0;
            var ax = X.Get(x, y, z, w);
            var ay = Y.Get(x, y, z, w);
            var az = Z.Get(x, y, z, w);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }

        private void CalculateRotMatrix(double x, double y, double z, double w, double u, double v)
        {
            var angle = Angle.Get(x, y, z, w, u, v) * 360.0 * Math.PI / 180.0;
            var ax = X.Get(x, y, z, w, u, v);
            var ay = Y.Get(x, y, z, w, u, v);
            var az = Z.Get(x, y, z, w, u, v);

            var cosangle = Math.Cos(angle);
            var sinangle = Math.Sin(angle);

            rotationMatrix[0, 0] = 1.0 + (1.0 - cosangle) * (ax * ax - 1.0);
            rotationMatrix[1, 0] = -az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[2, 0] = ay * sinangle + (1.0 - cosangle) * ax * az;

            rotationMatrix[0, 1] = az * sinangle + (1.0 - cosangle) * ax * ay;
            rotationMatrix[1, 1] = 1.0 + (1.0 - cosangle) * (ay * ay - 1.0);
            rotationMatrix[2, 1] = -ax * sinangle + (1.0 - cosangle) * ay * az;

            rotationMatrix[0, 2] = -ay * sinangle + (1.0 - cosangle) * ax * az;
            rotationMatrix[1, 2] = ax * sinangle + (1.0 - cosangle) * ay * az;
            rotationMatrix[2, 2] = 1.0 + (1.0 - cosangle) * (az * az - 1.0);
        }
    }
}


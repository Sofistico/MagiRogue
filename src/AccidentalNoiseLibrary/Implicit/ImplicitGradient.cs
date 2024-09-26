using System;

namespace AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitGradient : ImplicitModuleBase
    {
        private double gradientX0;

        private double gradientY0;

        private double gradientZ0;

        private double gradientW0;

        private double gradientU0;

        private double gradientV0;

        private double gradientX1;

        private double gradientY1;

        private double gradientZ1;

        private double gradientW1;

        private double gradientU1;

        private double gradientV1;

        private double length2;

        private double length3;

        private double length4;

        private double length6;

        public ImplicitGradient(
            double x0 = 0.00, double x1 = 1.00, double y0 = 0.00, double y1 = 1.00, double z0 = 0.00, double z1 = 1.00,
            double w0 = 0.00, double w1 = 1.00, double u0 = 0.00, double u1 = 1.00, double v0 = 0.00, double v1 = 1.00)
        {
            SetGradient(x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, v1);
        }

        public void SetGradient(double x0, double x1, double y0, double y1)
        {
            SetGradient(x0, x1, y0, y1, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(double x0, double x1, double y0, double y1, double z0, double z1)
        {
            SetGradient(x0, x1, y0, y1, z0, z1, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(double x0, double x1, double y0, double y1, double z0, double z1, double w0, double w1)
        {
            SetGradient(x0, x1, y0, y1, z0, z1, w0, w1, 0.00, 0.00, 0.00, 0.00);
        }

        public void SetGradient(double x0, double x1, double y0, double y1, double z0, double z1, double w0, double w1, double u0, double u1, double v0, double v1)
        {
            gradientX0 = x0;
            gradientY0 = y0;
            gradientZ0 = z0;
            gradientW0 = w0;
            gradientU0 = u0;
            gradientV0 = v0;

            gradientX1 = x1 - x0;
            gradientY1 = y1 - y0;
            gradientZ1 = z1 - z0;
            gradientW1 = w1 - w0;
            gradientU1 = u1 - u0;
            gradientV1 = v1 - v0;

            length2 = gradientX1 * gradientX1 + gradientY1 * gradientY1;
            length3 = gradientX1 * gradientX1 + gradientY1 * gradientY1 + gradientZ1 * gradientZ1;
            length4 = gradientX1 * gradientX1 + gradientY1 * gradientY1 + gradientZ1 * gradientZ1 + gradientW1 * gradientW1;
            length6 = gradientX1 * gradientX1 + gradientY1 * gradientY1 + gradientZ1 * gradientZ1 + gradientW1 * gradientW1 + gradientU1 * gradientU1 + gradientV1 * gradientV1;
        }

        public override double Get(double x, double y)
        {
            var dx = x - gradientX0;
            var dy = y - gradientY0;
            var dp = dx * gradientX1 + dy * gradientY1;
            dp /= length2;
            return dp;
        }

        public override double Get(double x, double y, double z)
        {
            var dx = x - gradientX0;
            var dy = y - gradientY0;
            var dz = z - gradientZ0;
            var dp = dx * gradientX1 + dy * gradientY1 + dz * gradientZ1;
            dp /= length3;
            return dp;
        }

        public override double Get(double x, double y, double z, double w)
        {
            var dx = x - gradientX0;
            var dy = y - gradientY0;
            var dz = z - gradientZ0;
            var dw = w - gradientW0;
            var dp = dx * gradientX1 + dy * gradientY1 + dz * gradientZ1 + dw * gradientW1;
            dp /= length4;
            return dp;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            var dx = x - gradientX0;
            var dy = y - gradientY0;
            var dz = z - gradientZ0;
            var dw = w - gradientW0;
            var du = u - gradientU0;
            var dv = v - gradientV0;
            var dp = dx * gradientX1 + dy * gradientY1 + dz * gradientZ1 + dw * gradientW1 + du * gradientU1 + dv * gradientV1;
            dp /= length6;
            return dp;
        }
    }
}

using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit
{
    public sealed class ImplicitSphere : ImplicitModuleBase
    {
        public ImplicitSphere(
            double xCenter = 0.00, double yCenter = 0.00, double zCenter = 0.00,
            double wCenter = 0.00, double uCenter = 0.00, double vCenter = 0.00,
            double radius = 1.00)
        {
            XCenter = new ImplicitConstant(xCenter);
            YCenter = new ImplicitConstant(yCenter);
            ZCenter = new ImplicitConstant(zCenter);
            WCenter = new ImplicitConstant(wCenter);
            UCenter = new ImplicitConstant(uCenter);
            VCenter = new ImplicitConstant(vCenter);
            Radius = new ImplicitConstant(radius);
        }

        public ImplicitModuleBase XCenter { get; set; }

        public ImplicitModuleBase YCenter { get; set; }

        public ImplicitModuleBase ZCenter { get; set; }

        public ImplicitModuleBase WCenter { get; set; }

        public ImplicitModuleBase UCenter { get; set; }

        public ImplicitModuleBase VCenter { get; set; }

        public ImplicitModuleBase Radius { get; set; }

        public override double Get(double x, double y)
        {
            var dx = x - XCenter.Get(x, y);
            var dy = y - YCenter.Get(x, y);
            var len = Math.Sqrt(dx * dx + dy * dy);
            var rad = Radius.Get(x, y);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override double Get(double x, double y, double z)
        {
            var dx = x - XCenter.Get(x, y, z);
            var dy = y - YCenter.Get(x, y, z);
            var dz = z - ZCenter.Get(x, y, z);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            var rad = Radius.Get(x, y, z);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override double Get(double x, double y, double z, double w)
        {
            var dx = x - XCenter.Get(x, y, z, w);
            var dy = y - YCenter.Get(x, y, z, w);
            var dz = z - ZCenter.Get(x, y, z, w);
            var dw = w - WCenter.Get(x, y, z, w);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
            var rad = Radius.Get(x, y, z, w);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            var dx = x - XCenter.Get(x, y, z, w, u, v);
            var dy = y - YCenter.Get(x, y, z, w, u, v);
            var dz = z - ZCenter.Get(x, y, z, w, u, v);
            var dw = w - WCenter.Get(x, y, z, w, u, v);
            var du = u - UCenter.Get(x, y, z, w, u, v);
            var dv = v - VCenter.Get(x, y, z, w, u, v);
            var len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw + du * du + dv * dv);
            var rad = Radius.Get(x, y, z, w, u, v);
            var i = (rad - len) / rad;
            if (i < 0) i = 0;
            if (i > 1) i = 1;

            return i;
        }
    }
}

using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary
{
    public class MappingRanges
    {
        public static readonly MappingRanges Default = new MappingRanges();

        public double MapX0 = -1;
        public double MapY0 = -1;
        public double MapZ0 = -1;
        public double MapX1 = 1;
        public double MapY1 = 1;
        public double MapZ1 = 1;

        public double LoopX0 = -1;
        public double LoopY0 = -1;
        public double LoopZ0 = -1;
        public double LoopX1 = 1;
        public double LoopY1 = 1;
        public double LoopZ1 = 1;
    }
}
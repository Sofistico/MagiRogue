using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System
{
    public interface ITime
    {
        int EnergyPoolActor { get; set; }

        int Turns { get; set; }

        int Tick { get; set; }

        void ProcessTick();
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.Effects
{
    public interface ITimedEffect
    {
        /// <summary>
        /// When in the time has the effect been applied
        /// </summary>
        public int TurnApplied { get; }

        /// <summary>
        /// How many turns this effect will be applied to
        /// </summary>
        public int Turns { get; }
    }
}
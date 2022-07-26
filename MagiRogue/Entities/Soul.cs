using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Soul
    {
        public int MaxMana { get; internal set; }
        public double CurrentMana { get; internal set; }
        public int WillPower { get; internal set; }
        public int WildMana { get; internal set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Race
    {
        public string RaceName { get; set; }

        public Race(string raceName)
        {
            RaceName = raceName;
        }
    }
}
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Race
    {
        public string Id { get; set; }

        public string RaceName { get; set; }

        // Max lifespan of the race
        public int LifespanMax { get; set; }

        // min lifespan of the race
        public int LifespanMin { get; set; }
        public int AdulthoodAge { get; set; }
        public int RaceViewRadius { get; set; }
        public int BaseStrenghtScore { get; internal set; }

        public Race()
        {
        }

        public Race(string raceName)
        {
            RaceName = raceName;
        }

        private string DebuggerDisplay()
        {
            return string.Format($"{RaceName}");
        }
    }
}
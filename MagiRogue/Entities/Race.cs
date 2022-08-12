using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Race
    {
        public string Id { get; set; }
        public string RaceName { get; set; }

        public char RaceGlyph { get; set; }
        public string RaceForeground { get; set; }
        public string RaceBackground { get; set; }

        public int AverageVolume { get; set; }
        public int MaxVolume { get; set; }

        // Age related stuff
        public int LifespanMax { get; set; }
        public int LifespanMin { get; set; }
        public int? ChildAge { get; set; }
        public int? TeenAge { get; set; }
        public int AdulthoodAge { get; set; }

        // stats
        public int RaceViewRadius { get; set; }
        public int BaseStrenght { get; set; }
        public int BaseToughness { get; set; }
        public int BaseEndurance { get; set; }
        public int BaseInt { get; set; }
        public int BaseWillPower { get; set; }
        public int BasePrecision { get; set; }

        // Body
        public double RaceNormalLimbRegen { get; set; }
        public double BleedRegenaration { get; set; }
        public bool CanRegenLostLimbs { get; set; }

        public string[] BodyPlan { get; set; }

        // Civ tendencies
        // Temporary
        public bool ValidCivRace { get; set; }

        public Race()
        {
        }

        private string DebuggerDisplay()
        {
            return string.Format($"{RaceName}");
        }
    }
}
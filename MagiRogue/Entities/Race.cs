using System.Diagnostics;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public struct Race
    {
        public string RaceName { get; set; }

        public Race(string raceName)
        {
            RaceName = raceName;
        }

        private string DebggerDisplay()
        {
            return string.Format($"{nameof(Race)}: {RaceName}");
        }
    }
}
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public struct Race
    {
        [DataMember]
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
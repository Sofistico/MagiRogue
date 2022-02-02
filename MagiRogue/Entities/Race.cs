using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    [DataContract]
    public struct Race
    {
        [DataMember]
        public readonly string RaceName { get; }

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
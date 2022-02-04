using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Race
    {
        [DataMember]
        public string RaceName { get; set; }

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
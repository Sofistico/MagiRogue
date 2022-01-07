using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data
{
    [DataContract]
    public class GameState
    {
        [DataMember]
        public UniverseTemplate Universe { get; set; }

        public GameState()
        {
            Universe = GameLoop.Universe;
        }
    }
}
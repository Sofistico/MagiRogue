using MagiRogue.Data.Enumerators;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Planet.History
{
    public class Myth
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MythWho MythWho { get; set; }
        public MythAction MythAction { get; set; }
        public MythWhat MythWhat { get; set; }
        /*public List<string> WhatItCreated { get; set; } = new();
        public List<string> WhatItDestroyed { get; set; } = new();
        public List<string> WhatItAntagonized { get; set; } = new();
        public List<string> WhatItGave { get; set; } = new();
        public List<string> WhatItBlessed { get; set; } = new();
        public List<string> WhatItCursed { get; set; } = new();
        public List<string> WhatItModified { get; set; } = new();*/

        public Myth(int id)
        {
            Id = id;
        }

        public Myth(int id, string name, MythWho mythWho, MythAction action, MythWhat whatAction)
        {
            Id = id;
            Name = name;
            MythWho = mythWho;
            MythAction = action;
            MythWhat = whatAction;
        }
    }
}
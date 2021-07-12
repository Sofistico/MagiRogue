using GoRogue.Components.ParentAware;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly Magic _spellsKnow;

        public IObjectWithComponents Parent { get; set; }

        public SpellCastingAI(Magic spellsKnow)
        {
            _spellsKnow = spellsKnow;
        }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            throw new NotImplementedException();
        }
    }
}
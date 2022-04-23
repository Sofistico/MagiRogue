using GoRogue.Components.ParentAware;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Magic;
using MagiRogue.UI.Windows;
using System;

namespace MagiRogue.Components
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly MagicManager _spellsKnow;

        public IObjectWithComponents Parent { get; set; }

        public SpellCastingAI(MagicManager spellsKnow)
        {
            _spellsKnow = spellsKnow;
        }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            throw new NotImplementedException();
        }
    }
}
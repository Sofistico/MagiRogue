using GoRogue.Components.ParentAware;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Magic;
using MagiRogue.UI.Windows;
using System;

namespace MagusEngine.ECS.Components.Ai
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly MagicManager _spellsKnow;

        public object Parent { get; set; }

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
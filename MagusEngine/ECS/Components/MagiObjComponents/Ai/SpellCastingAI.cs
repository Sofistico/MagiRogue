using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;
using System;

namespace MagusEngine.ECS.Components.MagiObjComponents.Ai
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly MagicComponent _spellsKnow;

        public object Parent { get; set; }

        public SpellCastingAI(MagicComponent spellsKnow)
        {
            _spellsKnow = spellsKnow;
        }

        public (bool sucess, long ticks) RunAi(MagiMap map)
        {
            throw new NotImplementedException();
        }
    }
}
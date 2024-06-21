using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;
using System;

namespace MagusEngine.ECS.Components.EntityComponents.Ai
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly Magic _spellsKnow;

        public object Parent { get; set; }

        public SpellCastingAI(Magic spellsKnow)
        {
            _spellsKnow = spellsKnow;
        }

        public (bool sucess, long ticks) RunAi(MagiMap map)
        {
            throw new NotImplementedException();
        }
    }
}
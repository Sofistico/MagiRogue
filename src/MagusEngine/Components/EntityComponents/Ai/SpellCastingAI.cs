using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using System.Linq;

namespace MagusEngine.Components.EntityComponents.Ai
{
    public class SpellCastingAI : IAiComponent
    {
        private readonly Magic _spellsKnow;

        public object? Parent { get; set; }

        public SpellCastingAI(Magic spellsKnow)
        {
            _spellsKnow = spellsKnow;
        }

        public (bool sucess, long ticks) RunAi(MagiMap? map)
        {
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"This is the spells: {_spellsKnow.KnowSpells.Select(static i => i.Name + ", ")}"));
            return (true, TimeDefSpan.CentisecondsPerSecond);
        }
    }
}

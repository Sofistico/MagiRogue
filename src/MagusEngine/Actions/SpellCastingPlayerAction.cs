using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class SpellCastingPlayerAction : IExecuteAction
    {
        private readonly MessageBusService _bus;
        private readonly Actor _actor;

        public SpellCastingPlayerAction(Actor actor)
        {
            _bus = Locator.GetService<MessageBusService>();
            _actor = actor;
        }

        public bool Execute(Universe world)
        {
            _bus.SendMessage<OpenWindowEvent>(new(WindowTag.SpellCasting));
            var getPlayer = (Actor)Find.ControlledEntity!;

            var targetCursor = world!.CurrentMap!.TargetCursor ??= new Target(getPlayer.Position);
            var magic = getPlayer.GetComponent<Magic>();
            _bus.SendMessage<OpenSpellCastingWindowMessage>(new(magic.KnowSpells, getPlayer.Soul.CurrentMana, selectedSpell => targetCursor.OnSelectSpell(selectedSpell, getPlayer)));

            return true;
        }
    }
}

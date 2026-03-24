using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using SadConsole.UI;

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
            // SpellSelectWindow spell = new(_getPlayer.Soul.CurrentMana);

            _targetCursor ??= new Target(_getPlayer.Position);
            var magic = _getPlayer.GetComponent<Magic>();
            spell.Show(magic.KnowSpells, selectedSpell => _targetCursor.OnSelectSpell(selectedSpell, (Actor)uni.CurrentMap!.ControlledEntitiy!), _getPlayer.Soul.CurrentMana);

            return true;
        }
    }
}

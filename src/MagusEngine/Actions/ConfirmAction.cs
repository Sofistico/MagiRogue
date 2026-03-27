using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;

namespace MagusEngine.Actions
{
    public class ConfirmAction : IExecuteAction
    {
        private readonly MessageBusService _bus;

        public ConfirmAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            var targetCursor = world?.CurrentMap?.TargetCursor;
            Actor getPlayer = world!.Player!;
            if (targetCursor?.AnyTargeted() == true)
            {
                long timeTaken = 0;
                bool sucess;
                if (targetCursor.State == TargetState.LookMode)
                {
                    targetCursor.LookTarget();
                    return true;
                }
                else if (targetCursor.State == TargetState.TargetingSpell)
                {
                    (sucess, var spellCasted) = targetCursor.EndSpellTargetting();
                    if (sucess)
                        timeTaken = TimeHelper.GetCastingTime(getPlayer, spellCasted!);
                }
                else
                {
                    (sucess, var item) = targetCursor.EndItemTargetting();
                    if (sucess)
                        timeTaken = TimeHelper.GetShootingTime(getPlayer, item!.Mass);
                }
                if (sucess)
                {
                    world?.CurrentMap?.TargetCursor = null;
                    _bus.SendMessage<ProcessTurnEvent>(new(timeTaken, sucess));
                }

                return sucess;
            }
            else
            {
                _bus.SendMessage<AddMessageLog>(new("Invalid target!"));
                return false;
            }
        }
    }
}


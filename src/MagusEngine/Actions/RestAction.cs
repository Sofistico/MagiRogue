using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;

namespace MagusEngine.Actions
{
    public class RestAction : IExecuteAction
    {
        private readonly MessageBusService _messageBus;
        private readonly Actor _actor;

        public RestAction(Actor actor)
        {
            _messageBus = Locator.GetService<MessageBusService>();
            _actor = actor;
        }

        public bool Execute(Universe world)
        {
            Body bodyStats = _actor.Body;
            //Mind mindStats = _actor.Mind;
            Soul soulStats = _actor.Soul;

            if (bodyStats.Stamina < bodyStats.MaxStamina || soulStats.CurrentMana < soulStats.MaxMana)
            {
                // calculate here the amount of time that it will take in turns to rest to full
                double staminaDif, manaDif;

                staminaDif = MathMagi.Round((bodyStats.MaxStamina - bodyStats.Stamina) / _actor.GetStaminaRegen());
                manaDif = MathMagi.Round((soulStats.MaxMana - soulStats.CurrentMana) / _actor.GetManaRegen());

                double totalTurnsWait = staminaDif + manaDif;

                bool sus = ActionManager.WaitForNTurns((int)totalTurnsWait, _actor);

                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"You have rested for {totalTurnsWait} turns"));

                return sus;
            }

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You have no need to rest"));

            return false;
        }
    }
}

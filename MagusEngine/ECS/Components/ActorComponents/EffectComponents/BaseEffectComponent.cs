using GoRogue.Components.ParentAware;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;

namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public class BaseEffectComponent : ParentAwareComponentBase<MagiEntity>
    {
        public int TurnToRemove { get; set; }
        public int TurnApplied { get; set; }
        public string EffectMessage { get; set; }
        public string Tag { get; set; }

        public BaseEffectComponent(int turnApplied, int turnToRemove, string effectMessage, string tag, bool customTurnTimer = false)
        {
            TurnToRemove = turnToRemove;
            TurnApplied = turnApplied;
            EffectMessage = effectMessage;
            Tag = tag;
            if (!customTurnTimer)
                ConfigureTurnTimer();
        }

        public virtual void ConfigureTurnTimer()
        {
            if (Find.Universe is not null)
                Find.Universe.Time.TurnPassed += GetTime_TurnPassed;
        }

        protected virtual void GetTime_TurnPassed(object? sender, TimeDefSpan e)
        {
            if (e.Seconds >= TurnToRemove)
            {
                Parent?.RemoveComponent(Tag);
                Locator.GetService<MessageBusService>()?
                    .SendMessage<AddMessageLog>(new(EffectMessage, Parent == Find.Universe.Player));
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            }
        }

        ~BaseEffectComponent()
        {
            Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
        }
    }
}

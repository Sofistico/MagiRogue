using GoRogue.Components.ParentAware;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils.Extensions;

namespace MagusEngine.Components
{
    public enum ExecutionType
    {
        PerTurn,
        OnEnd
    }

    public abstract class BaseEffectComponent : ParentAwareComponentBase<MagiEntity>
    {
        private bool _isActive;

        public long TickToRemove { get; set; }
        public long TickApplied { get; set; }
        public string? EffectMessage { get; set; }
        public string? RemoveMessage { get; set; }
        public string Tag { get; set; }
        public bool FreezesTurn { get; set; }
        public ExecutionType Execution { get; set; }

        protected BaseEffectComponent(long tickApplied, long tickToRemove, string? effectMessage, string tag, bool customTurnTimer = false, bool freezesTurn = false, string? removeMessage = null, ExecutionType execution = ExecutionType.PerTurn)
        {
            TickToRemove = tickToRemove;
            TickApplied = tickApplied;
            EffectMessage = effectMessage;
            Tag = tag;
            if (!customTurnTimer)
                ConfigureTurnTimer();
            FreezesTurn = freezesTurn;
            RemoveMessage = removeMessage;
            Execution = execution;
        }

        public void ConfigureTurnTimer()
        {
            if (Find.Universe is not null)
                Find.Universe.Time.TurnPassed += GetTime_TurnPassed;
        }

        protected virtual void GetTime_TurnPassed(object? sender, TimeDefSpan e)
        {
            if (e.Ticks <= TickApplied)
                return;
            if (e.Ticks >= TickToRemove && _isActive)
            {
                Parent?.RemoveComponent(this);
                if (!RemoveMessage.IsNullOrEmpty())
                    Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new(RemoveMessage!, Parent == Find.Universe.Player));
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
                _isActive = false;
                if (Execution == ExecutionType.OnEnd)
                    ExecuteEffect();
                return;
            }
            _isActive = true;
            // let me think better...
            if (Execution == ExecutionType.PerTurn)
                ExecuteEffect();
        }

        public abstract void ExecuteEffect();

        ~BaseEffectComponent()
        {
            Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
        }
    }
}

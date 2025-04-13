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

    public readonly struct BaseEffectConfig
    {
        public long TickToRemove { get; }
        public long TickApplied { get; }
        public string? EffectMessage { get; }
        public string? RemoveMessage { get; }
        public string Tag { get; }
        public bool FreezesTurn { get; }
        public ExecutionType Execution { get; } = ExecutionType.PerTurn;

        public BaseEffectConfig(long tickApplied, long tickToRemove, string? effectMessage, string tag, bool freezesTurn = false, string? removeMessage = null, ExecutionType execution = ExecutionType.PerTurn)
        {
            TickToRemove = tickToRemove;
            TickApplied = tickApplied;
            EffectMessage = effectMessage;
            RemoveMessage = removeMessage;
            Tag = tag;
            FreezesTurn = freezesTurn;
            Execution = execution;
        }
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

        protected BaseEffectComponent(BaseEffectConfig config, bool customTurnTimer = false)
        {
            TickToRemove = config.TickToRemove;
            TickApplied = config.TickApplied;
            EffectMessage = config.EffectMessage;
            Tag = config.Tag;
            if (!customTurnTimer)
                ConfigureTurnTimer();
            FreezesTurn = config.FreezesTurn;
            RemoveMessage = config.RemoveMessage;
            Execution = config.Execution;
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
                if (Execution == ExecutionType.OnEnd)
                    ExecuteEffect();
                Parent?.RemoveComponent(this);
                if (!RemoveMessage.IsNullOrEmpty())
                    Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new(RemoveMessage!, Parent == Find.Universe.Player));
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
                _isActive = false;
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

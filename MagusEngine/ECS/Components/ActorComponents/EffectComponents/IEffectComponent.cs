namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public interface IEffectComponent
    {
        public int TurnToRemove { get; set; }
        public int TurnApplied { get; set; }

        void ConfigureTurnTimer();
    }
}
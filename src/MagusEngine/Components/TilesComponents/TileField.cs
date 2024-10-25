namespace MagusEngine.Components.TilesComponents;

public class BaseTileField : BaseEffectComponent
{
    public int BasePower { get; set; }

    public BaseTileField(
        long tickApplied,
        long tickToRemove,
        string effectMessage,
        string tag,
        int basePower,
        bool customTurnTimer = false,
        bool freezesTurn = false,
        string removeMessage = null
    )
        : base(
            tickApplied,
            tickToRemove,
            effectMessage,
            tag,
            customTurnTimer,
            freezesTurn,
            removeMessage
        ) { }

    /// <summary>
    /// Executes the field effect's per-turn behavior.
    /// </summary>
    /// <remarks>
    /// Derived classes must implement this method to define how the field effect
    /// behaves each turn, typically using the BasePower property to determine
    /// the strength of the effect.
    /// </remarks>
    public abstract override void ExecutePerTurn();
}

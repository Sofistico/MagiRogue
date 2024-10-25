namespace MagusEngine.Components.TilesComponents;

/// <summary>
/// Base class for tile-based field effects that can be applied to the game terrain.
/// Provides core functionality for managing field effects with customizable power levels.
/// </summary>
public abstract class BaseTileField : BaseEffectComponent
{
    public int BasePower { get; set; }

    /// <summary>
    /// Initializes a new instance of the BaseTileField class.
    /// </summary>
    /// <param name="tickApplied">The tick when the effect is applied.</param>
    /// <param name="tickToRemove">The tick when the effect should be removed.</param>
    /// <param name="effectMessage">The message to display when the effect is applied.</param>
    /// <param name="tag">The unique identifier for this field effect.</param>
    /// <param name="basePower">The base power level of the field effect.</param>
    /// <param name="customTurnTimer">Whether to use custom turn timing.</param>
    /// <param name="freezesTurn">Whether the effect freezes turns.</param>
    /// <param name="removeMessage">Optional message to display when the effect is removed.</param>
    /// <exception cref="ArgumentException">Thrown when basePower is negative or required strings are null/empty.</exception>
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
        )
    {
        if (string.IsNullOrEmpty(effectMessage))
            throw new ArgumentException("Effect message cannot be null or empty", nameof(effectMessage));
        if (string.IsNullOrEmpty(tag))
            throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
        if (basePower < 0)
            throw new ArgumentException("Base power cannot be negative", nameof(basePower));
            
        BasePower = basePower;
    }

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

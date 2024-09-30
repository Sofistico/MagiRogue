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

    public override void ExecutePerTurn()
    {
    }
}

using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public class UncontrolledMovementComponent : BaseEffectComponent
    {
        public int TilesToMovePerTurn { get; set; }
        public Direction Direction { get; set; }

        public UncontrolledMovementComponent(int tilesToMove, Direction dir, int turnApplied, int turnToRemove, string effectMessage)
            : base(turnApplied, turnToRemove, effectMessage, "uncont_mov")
        {
            TilesToMovePerTurn = tilesToMove;
            Direction = dir;
        }
    }
}

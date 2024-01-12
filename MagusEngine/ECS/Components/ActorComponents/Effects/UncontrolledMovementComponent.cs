using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.ActorComponents.Effects
{
    public class UncontrolledMovementComponent : BaseEffectComponent
    {
        public int TilesToMovePerTurn { get; set; }
        public Direction Direction { get; set; }

        public UncontrolledMovementComponent(int tilesToMove, Direction dir, int turnApplied, int turnToRemove, string effectMessage)
            : base(turnApplied, turnToRemove, effectMessage, "uncont_mov", freezesTurn: true)
        {
            TilesToMovePerTurn = tilesToMove;
            Direction = dir;
        }

        public override void ExecutePerTurn()
        {
            throw new System.NotImplementedException();
        }
    }
}

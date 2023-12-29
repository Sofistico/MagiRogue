using MagusEngine.Systems.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.ECS.Components.ActorComponents.EffectComponents
{
    public class UncontrolledMovementComponent : BaseEffectComponent
    {
        public int TilesToMove { get; set; }

        public UncontrolledMovementComponent(int tilesToMove, int turnApplied, int turnToRemove, string effectMessage) 
            : base(turnApplied, turnToRemove, effectMessage, "uncont_mov")
        {
            TilesToMove = tilesToMove;
        }
    }
}

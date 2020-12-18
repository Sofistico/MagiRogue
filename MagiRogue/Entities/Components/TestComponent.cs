using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GoRogue.GameFramework;

namespace MagiRogue.Entities.Components
{
    public class TestComponent : ComponentBase
    {
        //public TestComponent : base(GameO)
        public TestComponent(IGameObject parent) : base(parent)
        {
            GameLoop.UIManager.MessageLog.Add($"It worked, here are your x and y value: {GameLoop.World.Player.Position.X} {GameLoop.World.Player.Position.Y}");
        }
    }
}
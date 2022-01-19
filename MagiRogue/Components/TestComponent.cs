using GoRogue.Components.ParentAware;

namespace MagiRogue.Components
{
    public class TestComponent : GoRogue.Components.ParentAware.IParentAwareComponent
    {
        public IObjectWithComponents Parent { get; set; }

        public TestComponent(IObjectWithComponents parent)
        {
            GameLoop.UIManager.MessageLog.Add($"It worked, here are your x and y value: {GameLoop.Universe.Player.Position.X} {GameLoop.Universe.Player.Position.Y}");
        }
    }
}
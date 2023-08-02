namespace MagusEngine.ECS.Components
{
    public class TestComponent
    {
        public TestComponent()
        {
            GameLoop.AddMessageLog($"It worked, here are your x and y value: {GameLoop.Universe.Player.Position.X} {GameLoop.Universe.Player.Position.Y}");
        }
    }
}
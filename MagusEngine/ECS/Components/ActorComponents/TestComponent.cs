namespace MagusEngine.ECS.Components.ActorComponents
{
    public class TestComponent
    {
        public TestComponent()
        {
            GameLoop.AddMessageLog($"It worked, here are your x and y value: {Find.Universe.Player.Position.X} {Find.Universe.Player.Position.Y}");
        }
    }
}
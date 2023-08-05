namespace MagusEngine.ECS.Components.TilesComponents
{
    public class DoorComponent
    {
        public bool IsOpen { get; set; }
        public bool Locked { get; set; }
        public char OpenGlyph { get; set; }
        public char ClosedGlyph { get; set; }
    }
}

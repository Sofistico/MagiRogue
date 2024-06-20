namespace MagusEngine.Exceptions
{
    public class ColorNotFoundException : System.Exception
    {
        public ColorNotFoundException()
        {
        }

        public ColorNotFoundException(string colorName) : base($"Color {colorName} not found in ColorExtensions2.ColorMappings")
        {
        }

        public ColorNotFoundException(string? colorName, System.Exception? innerException) : base($"Color {colorName} not found in ColorExtensions2.ColorMappings", innerException)
        {
        }
    }
}

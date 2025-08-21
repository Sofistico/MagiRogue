using Arquimedes.Enumerators;

namespace Arquimedes.Settings
{
    public class InputSetting
    {
        public required string[] Key { get; set; }
        public required KeymapAction Action { get; set; }
        public string Category { get; set; } = "Misc";
    }
}

using Arquimedes.Enumerators;

namespace Arquimedes.Settings
{
    public class InputSetting : IEquatable<InputSetting>
    {
        public required KeymapAction Action { get; set; }
        public required string[] Keys { get; set; }
        public string Category { get; set; } = "Misc";
        public string[]? Modifier { get; set; }

        public bool Equals(InputSetting? other)
        {
            if (Action == other?.Action && Category.Equals(other.Category))
                return true;

            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            return Equals(obj as InputSetting);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Action);
        }
    }
}

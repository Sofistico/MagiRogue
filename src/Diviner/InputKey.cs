using System.Diagnostics.CodeAnalysis;
using SadConsole.Input;

namespace Diviner
{
    public readonly struct InputKey(Keys key, string[] modifiers) : IEquatable<InputKey>
    {
        public Keys Key { get; } = key;
        public string[] Modifiers { get; } = modifiers ?? [];

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, string.Join(",", Modifiers));
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
                return false;
            return Equals((InputKey)obj);
        }

        public bool Equals(InputKey o)
        {
            if (o.Key == Key && Modifiers.SequenceEqual(o.Modifiers))
                return true;
            return false;
        }

        public static bool operator ==(InputKey left, InputKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InputKey left, InputKey right)
        {
            return !(left == right);
        }
    }
}

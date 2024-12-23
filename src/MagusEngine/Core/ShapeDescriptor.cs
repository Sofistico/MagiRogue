using Arquimedes.Interfaces;

namespace MagusEngine.Core
{
    public class ShapeDescriptor : IJsonKey
    {
        public required string Id { get; set; }
        public required string[] Name { get; set; }
        public required string[] Adjectives { get; set; }
        public required string Tile { get; set; }
    }
}

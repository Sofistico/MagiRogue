using Arquimedes.Interfaces;

namespace MagusEngine.Core
{
    public class ShapeDescriptor : IJsonKey
    {
        public string Id { get; set; }
        public string[] Name { get; set; }
        public string[] Adjectives { get; set; }
        public string Tile { get; set; }
    }
}

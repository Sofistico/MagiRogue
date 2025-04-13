using Newtonsoft.Json;

namespace Arquimedes.DataStructures
{
    public abstract class BasicTreeStructure<T>
    {
        public List<T>? Nodes { get; set; }
    }

    public abstract class BasicTreeNode<T>
    {
        public List<T>? Parents { get; set; }
        public List<T>? Children { get; set; }

        [JsonIgnore]
        public bool? IsRoot => Parents?.Count <= 0;
    }
}

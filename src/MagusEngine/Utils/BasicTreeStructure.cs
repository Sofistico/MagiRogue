using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Utils
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
        public bool? IsRoot { get => Parents?.Count <= 0; }
    }
}

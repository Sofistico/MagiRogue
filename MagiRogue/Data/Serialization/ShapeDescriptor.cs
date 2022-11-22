using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class ShapeDescriptor
    {
        public string Id { get; set; }
        public string[] Name { get; set; }
        public string[] Adjectives { get; set; }
        public string Tile { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.Entities
{
    public class ItemBlueprint
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double Lenght { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }
        public double Volume => Lenght * Height * Thickness;
    }
}

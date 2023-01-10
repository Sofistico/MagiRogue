using System.Collections.Generic;

namespace MagiRogue.Entities
{
    public interface IStat
    {
        Dictionary<string, int> Stats { get; set; }
    }
}
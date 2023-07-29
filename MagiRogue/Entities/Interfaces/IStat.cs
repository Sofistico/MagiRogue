using System.Collections.Generic;

namespace MagiRogue.Entities.Interfaces
{
    public interface IStat
    {
        Dictionary<string, int> Stats { get; set; }
    }
}
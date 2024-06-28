using System.Collections.Generic;

namespace MagusEngine.Core.Entities.Interfaces
{
    public interface IStat
    {
        Dictionary<string, int> Stats { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    // TODO: Check if Json will serialize!
    [Flags]
    public enum SiteRelationTypes
    {
        None = 1 << 1,
        LivesThere = 1 << 2,
        WantsDestroyed = 1 << 3,
        WantsToRule = 1 << 4,
        BornThere = 1 << 5,
        DiedThere = 1 << 6,

        Rules = 1 << 7,
        Ruled = 1 << 8,
    }
}
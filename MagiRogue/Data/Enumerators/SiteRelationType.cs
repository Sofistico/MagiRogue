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
        None = 1 << 0,
        LivesThere = 1 << 1,
        WantsDestroyed = 1 << 2,
        WantsToRule = 1 << 3,
        BornThere = 1 << 4,
        DiedThere = 1 << 5,

        Rules = 1 << 6,
        Ruled = 1 << 7,
    }
}
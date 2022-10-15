using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    public enum MythWho
    {
        None,
        God,
        Egg,
        Chaos,
        Chance,
        Science,
        Magic,
        Titan,
        Precursors,
        Demons,
        Angels,
        Forces
    }

    public enum MythAction
    {
        Created,
        Destroyed,
        Modified,
        Antagonized,
        Killed,
        Gave,
        Ascended,
        Descended,
        OpenPortal,
        ClosedPortal,
        Cursed,
        Blessed,
    }

    [Flags]
    public enum MythWhat
    {
        Race = 0,
        OriginMagic = 1 << 0,
        CostMagic = 1 << 1,
        Magic = 1 << 2,
        Land = 1 << 3,
        Region = 1 << 4,
        World = 1 << 5,
        God = 1 << 6,
        Item = 1 << 7,
        Reagent = 1 << 8,
        Afterlife = 1 << 9,
        OuterRealm = 1 << 10,
        Space = 1 << 11,
        Death = 1 << 12,
        Demons = 1 << 13,
        Angels = 1 << 14,
        Spirits = 1 << 15,
        Forces = 1 << 16,
        Individual = 1 << 17
    }
}
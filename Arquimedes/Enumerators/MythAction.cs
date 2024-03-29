﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquimedes.Enumerators
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
        Precursor,
        Demon,
        Angel,
        Force
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
        Demon = 1 << 13,
        Angel = 1 << 14,
        Spirit = 1 << 15,
        Force = 1 << 16,
        Individual = 1 << 17
    }
}
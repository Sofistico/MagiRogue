using System;

namespace MagiRogue.Data.Enumerators
{
    [Flags]
    public enum Food
    {
        None = 0,
        Carnivore = 1 << 0,
        Herbivore = 1 << 1,
        Omnivere = Carnivore | Herbivore,
    }
}

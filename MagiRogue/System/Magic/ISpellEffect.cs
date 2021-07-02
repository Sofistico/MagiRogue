using MagiRogue.System.Time;
using MagiRogue.Utils;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadRogue.Primitives;

namespace MagiRogue.System.Magic
{
    public interface ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public void ApplyEffect(Point target, Stat casterStats);
    }
}
using MagiRogue.System.Time;
using MagiRogue.Utils;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;

namespace MagiRogue.System.Magic
{
    public interface ISpellEffect
    {
        public SpellTypeEnum SpellEffect { get; set; }
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public void ApplyEffect(Coord target, Stat casterStats);
    }
}
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
    public class SpellEffects
    {
        public SpellTypeEnum SpellEffect { get; set; }

        public SpellAreaEffect AreaOfEffect { get; set; }

        public DamageType SpellDamageType { get; set; }

        public TimeDefSpan Duration { get; set; }

        public Stat StatsChange { get; set; }

        public SpellEffects(SpellTypeEnum spellEffect,
            SpellAreaEffect areaOfEffect,
            DamageType spellDamageType, TimeDefSpan spellDuration)
        {
            SpellEffect = spellEffect;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            Duration = spellDuration;
        }

        public void ApplyEffect(Coord action)
        {
        }
    }
}
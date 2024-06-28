using Arquimedes.Enumerators;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Utils;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagusEngine.Core.MapStuff
{
    public static class TargetHelper
    {
        public static List<Point>? SpellAreaHelper(ISpell spell, Point origin, Point targetPoint, Radius radius)
        {
            if (GetSpellAreaEffect(spell, SpellAreaEffect.Ball, out ISpellEffect eff))
            {
                RadiusLocationContext radiusLocation = new(origin, eff.Radius);
                return radius.PositionsInRadius(radiusLocation).ToList();
            }
            else if (GetSpellAreaEffect(spell, SpellAreaEffect.Cone, out eff))
            {
                return origin.Cone(eff.Radius, targetPoint, eff.ConeCircleSpan).Points.ToList();
            }
            return [];
        }

        public static bool GetSpellAreaEffect(ISpell? spell, SpellAreaEffect areaEffect, [NotNullWhen(true)] out ISpellEffect effect)
        {
            effect = spell?.Effects?.Find(e => e.AreaOfEffect == areaEffect)!;
            return effect != null;
        }
    }
}

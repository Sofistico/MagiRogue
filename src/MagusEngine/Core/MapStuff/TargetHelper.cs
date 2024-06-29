﻿using Arquimedes.Enumerators;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagusEngine.Core.MapStuff
{
    public static class TargetHelper
    {
        public static List<Point> SpellAreaHelper(ISpell spell, Point origin, Point targetPoint)
        {
            if (GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Ball, out ISpellEffect eff)
                || GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Cone, out eff)
                || GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Beam, out eff))
            {
                return SpellAreaHelper(eff, origin, targetPoint);
            }
            return [];
        }

        public static List<Point> SpellAreaHelper(ISpellEffect eff, Point origin, Point targetPoint)
        {
            if (eff.AreaOfEffect == SpellAreaEffect.Ball)
            {
                RadiusLocationContext radiusLocation = new(origin, eff.Radius);
                return Radius.Circle.PositionsInRadius(radiusLocation).ToList();
            }
            else if (eff.AreaOfEffect == SpellAreaEffect.Cone)
            {
                return origin.Cone(eff.Radius, targetPoint, eff.ConeCircleSpan).Points.ToList();
            }
            else if (eff.AreaOfEffect == SpellAreaEffect.Beam)
            {
                return origin.Beam(targetPoint, eff.Radius).Points.ToList();
            }
            else if (eff.AreaOfEffect == SpellAreaEffect.Level)
            {
                return Find.CurrentMap is null ? [] : [.. Find.CurrentMap.Positions()];
            }
            return [];
        }

        public static bool GetIfSpellHasSpecificAreaEffect(ISpell? spell, SpellAreaEffect areaEffect, [NotNullWhen(true)] out ISpellEffect effect)
        {
            effect = spell?.Effects?.Find(e => e.AreaOfEffect == areaEffect)!;
            return effect != null;
        }
    }
}

using Arquimedes.Enumerators;
using GoRogue.FOV;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MagusEngine.Core.MapStuff
{
    public static class TargetHelper
    {
        public static IEnumerable<Point> SpellAreaHelper(ISpell spell, Point origin, Point targetPoint)
        {
            if (GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Ball, out ISpellEffect eff)
                || GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Cone, out eff)
                || GetIfSpellHasSpecificAreaEffect(spell, SpellAreaEffect.Beam, out eff))
            {
                return SpellAreaHelper(eff, origin, targetPoint);
            }
            return [];
        }

        public static IEnumerable<Point> SpellAreaHelper(ISpellEffect eff, Point origin, Point targetPoint)
        {
            if (eff.AreaOfEffect == SpellAreaEffect.Ball)
            {
                if (eff.IgnoresWall)
                {
                    RadiusLocationContext radiusLocation = new(origin, eff.Radius);
                    return Radius.Circle.PositionsInRadius(radiusLocation);
                }
                else
                {
                    // move this to another class for ease of use
                    RecursiveShadowcastingBooleanBasedFOV fov = new(Find.CurrentMap!.TransparencyView);
                    fov.Calculate(origin, eff.Radius, Distance.Euclidean);
                    return fov.CurrentFOV;
                }
            }
            else if (eff.AreaOfEffect == SpellAreaEffect.Cone)
            {
                return origin.Cone(eff.Radius, targetPoint, eff.ConeCircleSpan).Points;
            }
            else if (eff.AreaOfEffect == SpellAreaEffect.Beam)
            {
                return origin.Beam(targetPoint, eff.Radius).Points;
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

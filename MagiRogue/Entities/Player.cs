using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Magic.Effects;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(Color foreground, Color background, Point position,
             int layer = (int)MapLayer.PLAYER) :
            base("Magus", foreground, background, '@', position, layer)
        {
            // sets the most fundamental stats, needs to set the godly flag up top, because it superseeds GodPower if it is
            // below.
            Stats.SetAttributes(
                viewRadius: 7,
                health: 10,
                baseHpRegen: 0.1f,
                bodyStat: 1,
                mindStat: 1,
                soulStat: 1,
                attack: 10,
                attackChance: 40,
                defense: 5,
                defenseChance: 20,
                speed: 1.0f,
                _baseManaRegen: 0.1f,
                personalMana: 12
                );

            Anatomy.Limbs = Data.LimbTemplate.BasicHumanoidBody(this);

            Magic.ShapingSkills = 8;

            SpellBase missile = new SpellBase("magic_missile",
                 "Magic Missile",
                new List<ISpellEffect>(), MagicSchool.Projection, 5, manaCost: 1.0f)
            { Proficency = 1 };

            missile.Effects.Add(
            new DamageEffect(missile.Power,
            SpellAreaEffect.Target,
            Utils.DamageType.Force));

            SpellBase cure = new SpellBase("cure_test", "Cure Test", new List<ISpellEffect>(),
                MagicSchool.MedicalMagic, 0, 1, 1)
            { Proficency = 1 };

            cure.Effects.Add(new DamageEffect(cure.Power,
                SpellAreaEffect.Self, Utils.DamageType.Force));

            SpellBase haste = new SpellBase("haste_self", "Haste", new List<ISpellEffect>(),
                MagicSchool.Dimensionalism, 0, 1, 1)
            { Proficency = 1 };
            haste.Effects.Add(new HasteEffect(SpellAreaEffect.Self, 2, 5));

            SpellBase mageSight = new SpellBase("mage_sight", "Mage Sight", new List<ISpellEffect>(),
                MagicSchool.Divination, 0, 1, 1)
            { Proficency = 1 };
            mageSight.Effects.Add(new MageSightEffect(5));

            SpellBase fireRay = new SpellBase("fire_ray", "Fire Ray", new List<ISpellEffect>(),
                MagicSchool.Projection, 5, 1, 1);
            fireRay.Effects.Add(new DamageEffect(fireRay.Power,
                SpellAreaEffect.Beam, Utils.DamageType.Fire));

            List<SpellBase> testSpells = new List<SpellBase>()
            {
                missile,
                cure,
                haste,
                mageSight,
                fireRay
            };

            Magic.KnowSpells.AddRange(testSpells);
        }
    }
}
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

            Magic.ShapingSkills = 5;

            SpellBase missile = new SpellBase("magic_missile",
                 "Magic Missile",
                new List<ISpellEffect>(), MagicSchool.Projection, 5, manaCost: 1.0);

            missile.Effects.Add(
            new DamageEffect(Magic.CalculateSpellDamage(Stats, missile),
            SpellAreaEffect.Target,
            Utils.DamageType.Force));

            SpellBase cure = new SpellBase("cure_test", "Cure Test", new List<ISpellEffect>(),
                MagicSchool.MedicalMagic, 0, 1, 1);

            cure.Effects.Add(new DamageEffect(Magic.CalculateSpellDamage(Stats, cure),
                SpellAreaEffect.Self, Utils.DamageType.Force));

            List<SpellBase> testSpells = new List<SpellBase>()
            {
                missile,
                cure
            };

            Magic.KnowSpells.AddRange(testSpells);
        }
    }
}
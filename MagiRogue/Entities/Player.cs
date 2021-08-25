using MagiRogue.Data;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Magic.Effects;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Linq;
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
                baseAttack: 10,
                attackChance: 40,
                protection: 5,
                defenseChance: 20,
                speed: 1.0f,
                _baseManaRegen: 0.1f,
                personalMana: 12
                );
            Stats.Precision = 3;

            Anatomy.Limbs = LimbTemplate.BasicHumanoidBody(this);

            Magic.ShapingSkill = 9;

            SpellBase missile = DataManager.QuerySpellInData("magic_missile");
            missile.Proficiency = 1;

            SpellBase cure = DataManager.QuerySpellInData("minor_cure");
            cure.Proficiency = 1;

            SpellBase haste = DataManager.QuerySpellInData("haste_self");
            haste.Proficiency = 1;

            SpellBase mageSight = DataManager.QuerySpellInData("mage_sight");
            mageSight.Proficiency = 1;

            SpellBase fireRay = DataManager.QuerySpellInData("fire_ray");
            fireRay.Proficiency = 1;

            SpellBase fireBall = DataManager.QuerySpellInData("fire_ball");
            fireBall.Proficiency = 1;

            SpellBase severWhip = DataManager.QuerySpellInData("sever_whip");
            severWhip.Proficiency = 1;

            SpellBase teleport = DataManager.QuerySpellInData("teleport");
            teleport.Proficiency = 1;

            SpellBase coneOfCold = DataManager.QuerySpellInData("cone_cold");
            coneOfCold.Proficiency = 1;
            SpellBase fingerOfDeath = DataManager.QuerySpellInData("finger_death");

            List<SpellBase> testSpells = new List<SpellBase>()
            {
                missile,
                cure,
                haste,
                mageSight,
                fireRay,
                fireBall,
                severWhip,
                teleport,
                coneOfCold,
                fingerOfDeath
            };

            Magic.KnowSpells.AddRange(testSpells);
        }
    }
}
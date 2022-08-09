﻿using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Magic;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(string name, Color foreground, Color background, Point position,
             int layer = (int)MapLayer.PLAYER) :
            base(name, foreground, background, '@', position, layer)
        {
            GetAnatomy().Limbs = EntityFactory.BasicHumanoidBody();
            GetAnatomy().Organs = EntityFactory.BasicHumanoidOrgans();
            Weight = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(50, 95);
            Volume = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(155, 200);
            //GetAnatomy().CalculateBlood(Weight);
            GetAnatomy().SetRandomLifespanByRace();
            GetAnatomy().SetCurrentAgeWithingAdulthood();
        }

        public static Player TestPlayer()
        {
            Player player = new Player("Magus", Color.White, Color.Black, Point.None);
            //player.Stats.SetAttributes(
            //    viewRadius: 7,
            //    health: 10,
            //    baseHpRegen: 0.1f,
            //    bodyStat: 1,
            //    mindStat: 1,
            //    soulStat: 1,
            //    baseAttack: 10,
            //    attackChance: 40,
            //    protection: 5,
            //    defenseChance: 20,
            //    speed: 1.0f,
            //    _baseManaRegen: 0.1f,
            //    personalMana: 12
            //    );
            player.Mind.Precision = 3;

            player.GetAnatomy().Limbs = EntityFactory.BasicHumanoidBody();
            player.GetAnatomy().Organs = EntityFactory.BasicHumanoidOrgans();

            player.Magic.ShapingSkill = 9;

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

            player.Magic.KnowSpells.AddRange(testSpells);

            return player;
        }

        public static Player ReturnPlayerFromActor(Actor actor)
        {
            if (actor is null)
                return null;

            Player player = new Player(actor.Name,
                actor.Appearance.Foreground,
                actor.Appearance.Background,
                actor.Position)
            {
                Inventory = actor.Inventory,
                Magic = actor.Magic,
                //Stats = actor.Stats,
                Volume = actor.Volume,
                Weight = actor.Weight,
                Material = actor.Material,
                XP = actor.XP,
                Mind = actor.Mind,
                Soul = actor.Soul
            };

            return player;
        }
    }
}
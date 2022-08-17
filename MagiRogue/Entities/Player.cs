using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
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
        }

        public static Player TestPlayer()
        {
            Player player = EntityFactory.PlayerCreatorFromZero(new Point(), "human", "Playa", 25,
                MagiRogue.Data.Enumerators.Sex.Female, "new_wiz");

            player.Magic.ShapingSkill = 9;

            player.Magic.KnowSpells[0].Proficiency = 1;

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
                Volume = actor.Volume,
                Weight = actor.Weight,
                XP = actor.XP,
                Body = actor.Body,
                Mind = actor.Mind,
                Soul = actor.Soul,
                CanBeKilled = actor.CanBeKilled,
                Description = actor.Description,
                IgnoresWalls = actor.IgnoresWalls,
                IsPlayer = true,
                Height = actor.Height,
                Broadness = actor.Broadness,
                Length = actor.Length
            };

            return player;
        }
    }
}
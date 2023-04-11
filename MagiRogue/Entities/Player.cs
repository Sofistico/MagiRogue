using MagiRogue.Data;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(string name, Color foreground, Color background, Point position) :
            base(name, foreground, background, '@', position)
        {
        }

        public static Player TestPlayer()
        {
            Player player = EntityFactory.PlayerCreatorFromZeroForTest(new Point(), "human", "Playa", 25,
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
            int co = testSpells.Count;
            for (int i = 0; i < co; i++)
            {
                player.Magic.AddToSpellList(testSpells[i]);
            }

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

        public string GetStaminaStatus()
        {
            double percent = MathMagi.GetPercentageBasedOnMax(Body.Stamina, Body.MaxStamina);

            if (percent > 50)
                return "Fine";
            if (percent <= 50 && percent > 25)
                return "Tired";
            if (percent <= 25 && percent > 1)
                return "Winded";
            if (percent <= 1)
                return "Spent";

            return "Should not see this!";
        }

        public string GetManaStatus()
        {
            double percent = MathMagi.GetPercentageBasedOnMax(Soul.CurrentMana, Soul.MaxMana);
            if (percent >= 95)
                return "Full";
            if (percent >= 60)
                return "Fine";
            if (percent <= 60 && percent >= 50)
                return "Half";
            if (percent <= 50 && percent > 25)
                return "Low";
            if (percent <= 25 && percent > 9)
                return "Spent";
            if (percent <= 9 && percent > 1)
                return "Fumes";
            if (percent <= 1)
                return "Empty";

            return "Should not see this!";
        }
    }
}
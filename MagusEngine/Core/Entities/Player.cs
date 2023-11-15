using MagusEngine.Core.Magic;
using MagusEngine.Factory;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Core.Entities
{
    // Creates a new player Default glyph is @
    public class Player : Actor
    {
        public Player(string name, Color foreground, Color background, Point position) :
            base(name, foreground, background, '@', position)
        {
        }

        public static Player TestPlayer()
        {
            Player player = EntityFactory.PlayerCreatorFromZeroForTest(new Point(), "human", "Playa", 25,
                Arquimedes.Enumerators.Sex.Female, "new_wiz");

            player.Magic.ShapingSkill = 25;

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

            List<SpellBase> testSpells = new()
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

        public static Player? ReturnPlayerFromActor(Actor actor)
        {
            if (actor is null)
                return null;

            return new Player(actor.Name,
                actor.SadCell.AppearanceSingle!.Appearance.Foreground,
                actor.SadCell.AppearanceSingle.Appearance.Background,
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
        }

        public string GetStaminaStatus()
        {
            double percent;
            if (Body.Stamina == Body.MaxStamina)
                percent = 100;
            else
                percent = MathMagi.GetPercentageBasedOnMax(Body.Stamina, Body.MaxStamina);

            if (percent > 50)
                return "[c:r f:Green]Fine";
            if (percent <= 50 && percent > 25)
                return "[c:r f:Yellow]Tired";
            if (percent <= 25 && percent > 1)
                return "[c:r f:Yellow][c:b]Winded";
            if (percent <= 1)
                return "[c:r f:Red][c:b]Spent";
            return "Should not see this!";
        }

        public string GetManaStatus()
        {
            double percent;
            if (Soul.CurrentMana == Soul.MaxMana)
                percent = 100;
            else
                percent = MathMagi.GetPercentageBasedOnMax(Soul.CurrentMana, Soul.MaxMana);

            if (percent >= 95)
                return "[c:r f:DarkBlue]Full";
            if (percent >= 60)
                return "[c:r f:MediumBlue]Fine";
            if (percent <= 60 && percent >= 50)
                return "[c:r f:MediumBlue]Half";
            if (percent <= 50 && percent > 25)
                return "[c:r f:SkyBlue]Low";
            if (percent <= 25 && percent > 9)
                return "[c:r f:LightBlue]Spent";
            if (percent <= 9 && percent > 1)
                return "[c:r f:220,239,247]Fumes";
            if (percent <= 1)
                return "[c:r b:220,239,247]Empty";
            return "Should not see this!";
        }
    }
}

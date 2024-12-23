using MagusEngine.Core.Magic;
using MagusEngine.Services.Factory;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    // Creates a new player Default glyph is @
    public class Player : Actor
    {
        public Player(string name, Color foreground, Color background, Point position) :
            base(name, foreground, background, '@', position)
        {
            Description = "Here is you, you are beautiful";
        }

        public static Player TestPlayer()
        {
            Player player = EntityFactory.PlayerCreatorFromZeroForTest(new Point(), "human", "Playa", 25,
                Arquimedes.Enumerators.Sex.Male, "new_wiz");

            var magic = player.GetComponent<Magic.Magic>();
            var abb = player.Mind.GetAbility(magic.GetMagicShapingAbility());
            abb.Score = 55;
            magic.KnowSpells[0].Proficiency = 1;
            if (!magic.KnowSpells.Exists(x => x.Id == "magic_missile"))
            {
                Spell missile = DataManager.QuerySpellInData("magic_missile", 2)!;
                magic.AddToSpellList(missile);
            }
            Spell cure = DataManager.QuerySpellInData("minor_cure", 1)!;

            Spell haste = DataManager.QuerySpellInData("haste_self", 1)!;

            Spell mageSight = DataManager.QuerySpellInData("mage_sight", 1)!;

            Spell fireRay = DataManager.QuerySpellInData("fire_ray", 1)!;

            Spell fireBall = DataManager.QuerySpellInData("fire_ball", 2)!;

            Spell severWhip = DataManager.QuerySpellInData("sever_whip", 1)!;

            Spell teleport = DataManager.QuerySpellInData("teleport", 1)!;

            Spell coneOfCold = DataManager.QuerySpellInData("cone_cold", 1)!;
            Spell fingerOfDeath = DataManager.QuerySpellInData("finger_death", 1)!;

            Spell knockBack = DataManager.QuerySpellInData("push", 1)!;

            magic.AddToSpellList([
                cure,
                haste,
                mageSight,
                fireRay,
                fireBall,
                severWhip,
                teleport,
                coneOfCold,
                fingerOfDeath,
                knockBack
            ]);

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
                Length = actor.Length,
                GoRogueComponents = actor.GoRogueComponents,
                AlwaySeen = actor.AlwaySeen,
                CanBeAttacked = actor.CanBeAttacked,
                CanInteract = actor.CanInteract,
                HistoryId = actor.HistoryId,
                IsTransparent = actor.IsTransparent,
                IsWalkable = actor.IsWalkable,
                LeavesGhost = actor.LeavesGhost,
                SituationalFlags = actor.SituationalFlags,
                State = actor.State,
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

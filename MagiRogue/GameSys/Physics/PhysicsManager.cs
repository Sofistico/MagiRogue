using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.Utils;

namespace MagiRogue.GameSys.Physics
{
    // For now it just deals with the list material, but in the future all interactions will go here
    public class PhysicsManager
    {
        protected PhysicsManager()
        {
        }

        /// <summary>
        /// Sets the material of the object by the id of the material
        /// </summary>
        /// <param name="id">Id of the material you want, must consult the json file</param>
        /// <returns></returns>
        public static MaterialTemplate SetMaterial(string id) =>
            DataManager.QueryMaterial(id);

        public static int CalculateStrikeForce(double weight, int actorStrikeForce)
        {
            return (int)MathMagi.Round(weight * actorStrikeForce);
        }

        public static double GetAttackVelocity(Actor actor, Attack attack)
        {
            var itemHeld = actor.WieldedItem();
            int ability;
            double finalSpeed;
            if (itemHeld is not null)
            {
                var speed = ((itemHeld.SpeedOfAttack + (itemHeld.Weight * itemHeld.Volume))
                    / actor.GetActorBaseSpeed());
                ability = actor.GetRelevantAttackAbility(itemHeld.WeaponType);
                finalSpeed = ability != 0 ? speed / ability : speed;
                return finalSpeed * attack.VelocityMultiplier * 100;
            }
            ability = actor.GetRelevantAbility(Data.Enumerators.AbilityName.Unarmed);
            finalSpeed = ability != 0 ?
                actor.GetActorBaseSpeed() / ability
                : actor.GetActorBaseSpeed();
            return finalSpeed * attack.VelocityMultiplier * 100;
        }
    }
}
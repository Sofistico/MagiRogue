using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.Utils;
using System;

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

        public static double GetAttackVelocity(Actor actor)
        {
            var itemHeld = actor.WieldedItem();
            if (itemHeld is not null)
            {
                var speed = ((itemHeld.SpeedOfAttack + itemHeld.Weight * itemHeld.Volume)
                    / actor.GetActorBaseSpeed());
                var ability = actor.GetRelevantAttackAbility(itemHeld.WeaponType);
                var finalSpeed = ability != 0 ? speed / ability : speed;
                return finalSpeed;
            }
            return actor.GetActorBaseSpeed();
        }
    }
}
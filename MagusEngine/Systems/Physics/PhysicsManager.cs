using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Utils;

namespace MagusEngine.Systems.Physics
{
    // For now it just deals with the list material, but in the future all interactions will go here
    public static class PhysicsManager
    {
        /// <summary>
        /// Sets the material of the object by the id of the material
        /// </summary>
        /// <param name="id">Id of the material you want, must consult the json file</param>
        /// <returns></returns>
        public static Material SetMaterial(string id) =>
            DataManager.QueryMaterial(id);

        public static int CalculateStrikeForce(double weight, int actorStrikeForce)
        {
            return (int)MathMagi.Round(weight * actorStrikeForce);
        }

        public static double CalculateStrikeForce(double weight, double actorStrikeForce)
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
                var speed = (itemHeld.SpeedOfAttack + (itemHeld.Weight * itemHeld.Volume))
                    / actor.GetActorSpeed();
                ability = actor.GetRelevantAbility(attack.AttackAbility);
                finalSpeed = ability != 0 ? speed / ability : speed;
                return finalSpeed * attack.VelocityMultiplier * 100;
            }
            ability = actor.GetRelevantAbility(attack.AttackAbility);
            finalSpeed = ability != 0 ?
                actor.GetActorSpeed() / ability
                : actor.GetActorSpeed();
            return finalSpeed * attack.VelocityMultiplier * 100;
        }

        public static double CalculateNewton2Law(double weight, double pushForceInMPS)
        {
            return weight * pushForceInMPS;
        }

        public static double CalculateNewton2LawReturnAcceleration(double weight, double force)
        {
            return force > 0 && weight > 0 ? force / weight : 0;
        }

        public static double CalculateFrictionToMovement(double frictionCoeficient, double force)
        {
            return force * frictionCoeficient;
        }
    }
}

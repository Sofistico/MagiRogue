using MagusEngine.Commands;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;
using System;

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

        public static void DealWithPushes(MagiEntity entity,
            double pushForce,
            int baseDamage,
            Direction directionToBeFlung,
            DamageType damageType)
        {
            if (entity is null && pushForce == 0)
                return;
            // calculate on force necessary to push entity if it's enough
            var force = CalculateNewton2Law(entity.Weight, pushForce);

            // then add friction
            var forceAfterFriction = CalculateFrictionToMovement(0.15, force);
            var accelerationNecessaryToMoveEntity = CalculateNewton2LawReturnAcceleration(entity.Weight, forceAfterFriction);

            if (accelerationNecessaryToMoveEntity >= pushForce)
                return; // not enough punch in the spell to move the entity

            // then calculate damage as base damage + forceAfterFriction(energy not lost to friction)
            var damage = CalculateStrikeForce(entity.Weight, forceAfterFriction) + baseDamage;

            // the acceleration isn't the same, and the meters is more the velocity of the object, since the formula would be:
            // V =  a * t which t is time, and spell resolution happens in a second or less after casting, then this simplification should logicaly work!
            int meters = (int)pushForce;
            BodyPart? bp = null;
            bool run = true;
            for (int i = 0; i < meters || run; i++)
            {
                // is this enough?
                var tile = (entity?.MagiMap?.GetTileAt(entity.Position + directionToBeFlung + i)) 
                    ?? throw new ApplicationException("The tile was null can't push!");
                if (entity is Actor actor)
                    bp = actor.GetAnatomy().Limbs.GetRandomItemFromList();
                var dmgThisTile = damage;
                if (tile?.IsWalkable == false)
                {
                    dmgThisTile *= tile.Material.Density ?? 1; // massive damage by hitting a wall, multiplied by something i dunno
                    run = false;
                }
                ActionManager.MoveActorTo(entity!, tile!.Position);
                CombatUtils.DealDamage(dmgThisTile, entity!, damageType, limbAttacked: bp);
            }
        }
    }
}

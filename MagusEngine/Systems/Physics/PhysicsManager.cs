﻿using MagusEngine.Commands;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

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
            var damage = CalculateStrikeForce(entity.Weight, forceAfterFriction);

            // the acceleration isn't the same, and the meters is more the velocity of the object, since the formula would be:
            // V =  a * t which t is time, and spell resolution happens in a second or less after casting,
            // then this simplification should logicaly work!
            int meters = (int)pushForce;
            List<BodyPart?> bps = [];
            Tile? tile = null;
            for (int i = 0; i < meters; i++)
            {
                var currentTile = (entity?.MagiMap?.GetTileAt(tile is null ? entity.Position + directionToBeFlung : tile.Position + directionToBeFlung))
                    ?? throw new ApplicationException("The tile was null can't push!");
                // is this enough?

                if (entity is Actor actor)
                    bps.Add(actor.GetAnatomy().Limbs.GetRandomItemFromList());
                damage += damage;
                if (tile?.IsWalkable == false)
                {
                    damage *= tile.Material.Density ?? 1; // massive damage by hitting a wall, multiplied by something i dunno
                    break;
                }
                tile = currentTile;
            }
            // only move the actor effective position to the last tile, maybe there will be a need to redo this, but for now with instanteneous movement,
            // this is good enough
            ActionManager.MoveActorTo(entity!, tile!.Position);

            foreach (var bp in bps)
            {
                CombatUtils.DealDamage(damage, entity!, damageType, tile?.Material, tile?.ReturnAttack(), limbAttacked: bp);
            }
        }
    }
}

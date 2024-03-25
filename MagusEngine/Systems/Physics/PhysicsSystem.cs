using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.ECS.Components.ActorComponents.Effects;
using MagusEngine.Services;
using MagusEngine.Utils;
using SadRogue.Primitives;
using System;
using System.Linq;

namespace MagusEngine.Systems.Physics
{
    // For now it just deals with the list material, but in the future all interactions will go here
    public static class PhysicsSystem
    {
        public static int CalculateMomentum(double weight, int force)
        {
            return (int)MathMagi.Round(weight * force);
        }

        public static double CalculateMomentum(double weight, double force)
        {
            return (int)MathMagi.Round(weight * force);
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
            finalSpeed = ability != 0 ? actor.GetActorSpeed() / ability : actor.GetActorSpeed();
            return finalSpeed * attack.VelocityMultiplier * 100;
        }

        public static double CalculateNewton2Law(double mass, double acceleration)
        {
            return mass * acceleration;
        }

        public static double CalculateNewton2LawReturnAcceleration(double mass, double force)
        {
            return force > 0 && mass > 0 ? force / mass : 0;
        }

        public static double CalculateFrictionToMovement(double frictionCoeficient, double force)
        {
            return force * frictionCoeficient;
        }

        public static double CalculateInitialVelocityFromMassAndForce(double mass, double force)
        {
            return Math.Sqrt(2 * force / mass);
        }

        public static double CalculateProjectileRange(double v0, double angle, double g)
        {
            return ((Math.Pow(v0, 2)) * (Math.Sin(2 * angle))) / g;
        }

        public static double CalculateProjectileTime(double v0, int angle, double g)
        {
            return (2 * (v0 * Math.Sin(angle))) / g;
        }

        // create a CalculateProjectileHeight
        public static double CalculateProjectileHeight(double v0, double angle, double g)
        {
            return Math.Pow(v0, 2) * Math.Pow(Math.Sin(angle), 2) / (2 * g);
        }

        /// <summary>
        /// displacement based on the acceleration
        /// </summary>
        /// <param name="t">Time that is accelareted s</param>
        /// <param name="u">Initial velocity m/s</param>
        /// <param name="a">Acceleration m/s^2</param>
        /// <returns>s - distance in meters (tiles)</returns>
        public static double CalculateDisplacementWithAcceleration(double t, double u, double a) => (u * t) + ((0.5 * a) * Math.Pow(t, 2));

        /// <summary>
        /// The total displacement given by velocity * time
        /// </summary>
        /// <returns>s - distance in meters (tiles)</returns>
        public static double CalculateSimpleDisplacement(double v, double t) => v * t;

        /// <summary>
        /// Time taken to get to a certain distance from a given acceleration
        /// </summary>
        /// <param name="a">acceleration m/s^2</param>
        /// <param name="s">displacement m</param>
        /// <returns>t - time in seconds</returns>
        public static double CalculateTimeToTravelDistanceFromAcceleration(double a, double s) =>
            Math.Sqrt(2 * s / a);

        //public static double CalculateCollusionDontKnowType(double? m1, double? m2, double? u1, double? u2, double? v1, double? v2)
        //{
        //    var firstEquation = m1 * u1 + m2 * u2;
        //    var secondEquation = m1 * v1 + m2 * v2;
        //    return;
        //}

        public static void DealWithPushes(MagiEntity? entity,
            double pushForce,
            Direction directionToBeFlung)
        {
            if (entity is null && pushForce == 0)
                return;
            string effectMessage = $"The {entity?.Name} is sent flying!";

            var acceleration = CalculateNewton2LawReturnAcceleration(entity!.Weight, pushForce);
            var tiles = CalculateDisplacementWithAcceleration(1, 0, acceleration);
            if (tiles == 0)
            {
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"The {entity?.Name} resists being flung!", Find.CurrentMap?.PlayerFOV.CurrentFOV.Contains(entity.Position) ?? false));
                return;
            }
            // the force is applied exactly once
            // and since it only accelerates for the point of contact, the target has only time to accelarate minimally
            UncontrolledMovementComponent comp = new(pushForce,
                (int)Math.Round(tiles, MidpointRounding.ToZero),
                directionToBeFlung,
                Find.Time.Tick,
                Find.Time.Tick + 1, // should run exactly once
                effectMessage);
            entity?.AddComponent(comp, comp.Tag);
        }
    }
}

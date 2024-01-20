﻿using MagusEngine.Commands;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.ActorComponents.Effects
{
    public class UncontrolledMovementComponent : BaseEffectComponent
    {
        public int TilesToMovePerTurn { get; }
        public double Force { get; }
        public Direction Direction { get; }

        public UncontrolledMovementComponent(double force, int tilesToMove, Direction dir, long turnApplied, long turnToRemove, string effectMessage)
            : base(turnApplied, turnToRemove, effectMessage, "uncont_mov", freezesTurn: true)
        {
            TilesToMovePerTurn = tilesToMove;
            Direction = dir;
            Force = force;
        }

        public override void ExecutePerTurn()
        {
            if (Parent is null)
                return;
            Point pointDir = new Point(Direction.DeltaX * TilesToMovePerTurn, Direction.DeltaY * TilesToMovePerTurn);
            Point pointToGo = Parent.Position + pointDir;
            Point finalPoint = Point.None;
            int distance = (int)Parent.Position.GetDistance(pointToGo);
            BodyPart? bp = null;
            double damage = Force;
            for (int i = 0; i < distance; i++)
            {
                var currentTile = (Parent?.MagiMap?.GetTileAt(finalPoint == Point.None ? Parent.Position + Direction : finalPoint + Direction))
                    ?? throw new ApplicationException("The tile was null can't push!");
                // is this enough?
                if (Parent is Actor actor)
                    bp = actor.GetAnatomy().Limbs.GetRandomItemFromList();
                if (currentTile?.IsWalkable == false)
                {
                    damage *= currentTile.Material.Density ?? 1; // massive damage by hitting a wall, multiplied by something i dunno
                    CombatSystem.DealDamage(PhysicsSystem.CalculateMomentum(Parent.Weight, damage),
                        Parent,
                        DataManager.QueryDamageInData("blunt"),
                        currentTile?.Material,
                        currentTile?.ReturnAttack(),
                        limbAttacked: bp);
                    break;
                }
                else
                {
                    CombatSystem.DealDamage(PhysicsSystem.CalculateMomentum(Parent.Weight, damage),
                        Parent,
                        DataManager.QueryDamageInData("blunt"),
                        currentTile?.Material,
                        currentTile?.ReturnAttack(),
                        limbAttacked: bp);

                    finalPoint = currentTile!.Position;
                }
            }
            // only move the actor effective position to the last tile, maybe there will be a need to redo this, but for now with instanteneous movement,
            // this is good enough
            ActionManager.MoveActorTo(Parent!, finalPoint);
        }
    }
}
using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagusEngine.Bus;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Factory;
using MagusEngine.Generators;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadConsole;
using System.Collections.Generic;
using System.Text;
using MagiMap = MagusEngine.Core.MapStuff.MagiMap;

namespace MagusEngine.Commands
{
    /// <summary>
    /// Contains all generic actions performed on entities and tiles including combat, movement, and
    /// so on.
    /// </summary>
    public static class ActionManager
    {
        private const int maxTries = 10;

        /// <summary>
        /// Move the actor BY +/- X&Y coordinates returns true if the move was successful
        /// and false if unable to move there
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="positionChange"></param>
        public static bool MoveActorBy(MagiEntity entity, Point positionChange)
        {
            return entity.MoveBy(positionChange);
        }

        /// <summary>
        /// Moves the actor To a position, by means of teleport returns true if the move was
        /// successful and false if unable to move
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool MoveActorTo(MagiEntity entity, Point position)
        {
            return entity.MoveTo(position);
        }

        // TODO: An df inspired menu with body parts located.
        /// <summary>
        /// Executes an attack from an attacking actor on a defending actor, and then describes the
        /// outcome of the attack in the Message Log
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        public static int MeleeAttack(Actor attacker,
            Actor defender,
            Attack? attack = null,
            Limb? limbChoosen = null)
        {
            if (!defender.CanBeAttacked)
                return 0;

            if (!attacker.GetAnatomy().HasAnyRaceAttack)
            {
                attack = Attack.PushAttack();
            }
            attack ??= attacker.GetRaceAttacks().GetRandomItemFromList();
            bool isPlayer = attacker is Player;
            var showMessage = isPlayer || Find.Universe.Player.CanSee(attacker.Position);

            if (attacker.Body.Stamina <= 0)
            {
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"You are too tired to attack|{attacker.Name} is too tired to attack!",
                    showMessage,
                    isPlayer ? PointOfView.First : PointOfView.Third));
                return TimeHelper.Wait;
            }

            // Create two messages that describe the outcome of the attack and defense
            StringBuilder attackMessage = new();
            StringBuilder defenseMessage = new();

            (bool hit, BodyPart limbAttacked, BodyPart limbAttacking, DamageType dmgType, Item? itemUsed, Material attackMaterial) 
                = CombatSystem.ResolveHit(attacker, defender, attackMessage, attack, isPlayer, limbChoosen);
            double finalMomentum = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker,
                defender,
                hit,
                limbAttacking,
                attack,
                itemUsed);

            // Display the outcome of the attack & defense
            Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new(attackMessage.ToString(), showMessage));
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                showMessage = isPlayer || Find.Universe.Player.CanSee(defender.Position);
                Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new(defenseMessage.ToString(), showMessage));
            }

            // The defender now takes damage
            CombatSystem.ResolveDamage(defender,
                finalMomentum,
                dmgType,
                limbAttacked,
                attackMaterial,
                attack,
                itemUsed,
                limbAttacking);
            var staminaDiscount = attacker.Body.Stamina
                - (attack.PrepareVelocity * 10) + (attacker.Body.Endurance * 0.5);
            // discount stamina from the attacker
            attacker.Body.Stamina = MathMagi.Round(staminaDiscount);

            return TimeHelper.GetAttackTime(attacker, attack);
        }

        /// <summary>
        /// Removes an Actor that has died and displays a message showing the actor that has died,
        /// and the loot they dropped
        /// </summary>
        /// <param name="actor"></param>
        public static void ResolveDeath(Actor actor)
        {
            CombatSystem.ResolveDeath(actor);
        }

        /// <summary>
        /// Gets
        /// </summary>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public static bool DirectAttack(Actor attacker)
        {
            // Lists all monsters that are close and their locations
            List<Actor> monsterClose = new();

            // Saves all Points directions of the attacker.
            Point[] directions = attacker.Position.GetDirectionPoints();

            foreach (Point direction in directions)
            {
                Actor monsterLocation = attacker.MagiMap.GetEntityAt<Actor>(direction);

                if (monsterLocation != null && monsterLocation != attacker)
                {
                    monsterClose.Add(monsterLocation);
                    if (monsterClose.Count > 0)
                    {
                        // add logic for attack here
                        // TODO: make it possible to choose which monster to strike and test what happens when there is more
                        // than one monster nearby.
                        Actor closestMonster = monsterClose[0];
                        MeleeAttack(attacker, closestMonster);
                        return true;
                    }
                }
            }

            // default response
            return false;
        }

        // Tries to pick up an Item and add it to the Actor's inventory list
        public static bool PickUp(Actor actor, Item item)
        {
            // Add the item to the Actor's inventory list and then destroy it
            if (item != null)
            {
                actor.Inventory.Add(item);
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{actor.Name} picked up {item.Name}"));
                item.RemoveFromMap();
                return true;
            }
            else
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There are no itens here"));
                return false;
            }
        }

        /// <summary>
        /// Triggered when an Actor attempts to move into a doorway. A closed door opens when used
        /// by an Actor, it takes a full turn because there can be 2 combination, locked and
        /// unlocked, and im lazy to properly separate the time taken.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="door"></param>
        /// <returns></returns>
        public static bool UseDoor(Actor actor, DoorComponent door, string doorName = "door")
        {
            // Handle a locked door
            if (door.Locked)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("The door is locked!"));
            }

            // Handled an unlocked door that is closed
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{actor.Name} opened a {doorName}"));
                actor.MagiMap.ForceFovCalculation();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes an open door
        /// </summary>
        /// <param name="actor">Actor that closes the door</param>
        /// <param name="door">Door that wil be closed&gt;</param>
        public static bool CloseDoor(Actor actor)
        {
            foreach (Point points in actor.Position.GetDirectionPoints())
            {
                var tile = actor.MagiMap.GetTileAt<DoorComponent>(points);
                var possibleDoor = tile.GetComponent<DoorComponent>();
                if (possibleDoor?.IsOpen == true)
                {
                    possibleDoor.Close();
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{actor.Name} closed a {tile.Name}"));
                    actor.MagiMap.ForceFovCalculation();
                    return true;
                }
            }
            return false;
        }

        public static bool DropItems(Actor inv)
        {
            if (inv.Inventory.Count == 0)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is no item to drop in your inventory"));
                return false;
            }
            else
            {
                Item item = inv.Inventory[0];
                inv.Inventory.Remove(item);
                item.Position = inv.Position;
                inv.MagiMap.AddMagiEntity(item);
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{inv.Name} dropped {item.Name}"));
                return true;
            }
        }

        public static bool NodeDrain(Actor actor)
        {
            Point[] direction = actor.Position.GetDirectionPoints();

            //foreach (Point item in direction)
            //{
            //    if (Find.CurrentMap.Terrain[item] is NodeTile node)
            //    {
            //        node.DrainNode(actor);
            //        return true;
            //    }
            //}
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("No node here to drain"));
            return false;
        }

#if DEBUG

        public static void ToggleFOV()
        {
            if (Find.CurrentMap!.GoRogueComponents.GetFirstOrDefault<FOVHandler>()!.IsEnabled!)
            {
                Find.CurrentMap?.GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.Disable(false);
            }
            else
            {
                Find.CurrentMap?.GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.Enable();
            }
        }

        public static void CreateNewMapForTesting()
        {
            // it's just plain wrong, need to fix later.
            /*GameLoop.World.CurrentMap.RemoveAllEntities();
            GameLoop.World.CurrentMap.RemoveAllTiles();
            GameLoop.World.CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            for (int i = 0; i < GameLoop.World.AllMaps.Count; i++)
            {
                GameLoop.World.AllMaps[i].RemoveAllEntities();
                GameLoop.World.AllMaps[i].RemoveAllTiles();
                GameLoop.World.AllMaps[i].GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            }
            GameLoop.World.Player = null;
            GameLoop.World.AllMaps.Clear();
            GameLoop.World.AllMaps = null;*/
            Find.Universe.WorldMap = new PlanetGenerator().CreatePlanet(500, 500);
        }

        public static (bool, Actor) CreateTestEntity(Point pos, Universe universe)
        {
            Actor found = EntityFactory.ActorCreator(pos,
                 "human", "Test Person", 25, Sex.None);

            universe.AddEntityToCurrentMap(found);

            return (true, found);
        }

#endif

        //public static bool SacrificeLifeEnergyToMana(Actor actor)
        //{
        //    int maxMana = actor.Soul.MaxMana;
        //    if (actor.Soul.CurrentMana != maxMana)
        //    {
        //        //actor.Anatomy.Health -= 1;
        //        actor.GetAnatomy().BloodCount -= 100f;
        //        int roll = Dice.Roll("1d3");
        //        float bloodyManaGained = float.Parse($"0.{roll}", CultureInfo.InvariantCulture.NumberFormat);
        //        actor.Soul.CurrentMana = MathMagi.Round(actor.Soul.CurrentMana + bloodyManaGained);
        //        GameLoop.AddMessageLog($"You ritually wound yourself, channeling your blood into mana, gaining {bloodyManaGained} blood mana");
        //        return true;
        //    }

        //    GameLoop.AddMessageLog("You feel too full for this right now");
        //    return false;
        //}

        public static bool RestTillFull(Actor actor)
        {
            Body bodyStats = actor.Body;
            //Mind mindStats = actor.Mind;
            Soul soulStats = actor.Soul;

            if (bodyStats.Stamina < bodyStats.MaxStamina || soulStats.CurrentMana < soulStats.MaxMana)
            {
                // calculate here the amount of time that it will take in turns to rest to full
                double staminaDif, manaDif;

                staminaDif = MathMagi.Round((bodyStats.MaxStamina - bodyStats.Stamina) / actor.GetStaminaRegen());
                manaDif = MathMagi.Round((soulStats.MaxMana - soulStats.CurrentMana) / actor.GetManaRegen());

                double totalTurnsWait = staminaDif + manaDif;

                bool sus = WaitForNTurns((int)totalTurnsWait, actor);

                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"You have rested for {totalTurnsWait} turns"));

                return sus;
            }

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You have no need to rest"));

            return false;
        }

        /// <summary>
        /// Equips an item
        /// </summary>
        /// <param name="actor">The actor that wil receive the item to be equiped</param>
        /// <param name="item">The item that will be equiped</param>
        /// <returns>Returns true for sucess and false for failure</returns>
        public static bool EquipItem(Actor actor, Item item)
        {
            item.Equip(actor);
            return actor.GetEquipment().ContainsValue(item);
        }

        /// <summary>
        /// Unequips an item, if there is no item return false
        /// </summary>
        /// <param name="actor">The actor that will unequip the item</param>
        /// <param name="item">The item to be unequiped</param>
        /// <returns>Returns false for failure and true for sucess</returns>
        public static bool UnequipItem(Actor actor, Item item)
        {
            item.Unequip(actor);
            return !actor.GetEquipment().ContainsValue(item);
        }

        public static bool EnterDownMovement(Point playerPoint)
        {
            Furniture? possibleStairs = Find.CurrentMap?.GetEntityAt<Furniture>(playerPoint);
            var possibleWorldTileHere = Find.CurrentMap?.GetComponentInTileAt<WorldTile>(playerPoint);
            MagiMap currentMap = Find.CurrentMap;
            if (possibleStairs?.MapIdConnection.HasValue == true)
            {
                MagiMap map = Universe.GetMapById(possibleStairs.MapIdConnection.Value);
                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(map, map.GetRandomWalkableTile(), currentMap));
                Find.Universe.ZLevel -= map.ZAmount;

                return true;
            }
            else if (possibleStairs is null && possibleWorldTileHere is null)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is no way to go down from here!"));
                return false;
            }

            if (possibleWorldTileHere?.Visited == false)
            {
                possibleWorldTileHere.Visited = true;

                RegionChunk chunk = Find.Universe.GenerateChunck(playerPoint);
                Find.Universe.CurrentChunk = chunk;
                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].GetRandomWalkableTile(), currentMap));
                return true;
            }
            else if (possibleWorldTileHere?.Visited == true)
            {
                RegionChunk chunk = Find.Universe.GetChunckByPos(playerPoint);
                Find.Universe.CurrentChunk = chunk;
                // if entering the map again, set to update
                chunk.SetMapsToUpdate();
                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].LastPlayerPosition, currentMap));

                return true;
            }
            else
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is nowhere to go!"));
                return false;
            }
        }

        public static bool EnterUpMovement(Point playerPoint)
        {
            bool possibleChangeMap = Find.Universe.PossibleChangeMap;
            Furniture possibleStairs =
                Find.CurrentMap.GetEntityAt<Furniture>(playerPoint);
            MagiMap currentMap = Find.CurrentMap;

            if (possibleChangeMap)
            {
                if (possibleStairs is not null && !Find.Universe.MapIsWorld()
                    && possibleStairs.MapIdConnection is not null)
                {
                    MagiMap map = Universe.GetMapById(possibleStairs.MapIdConnection.Value);
                    Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(map, map.GetRandomWalkableTile(), currentMap));
                    Find.Universe.ZLevel += map.ZAmount;

                    return true;
                }
                else if (!Find.Universe.MapIsWorld())
                {
                    MagiMap map = Find.Universe.WorldMap.AssocietatedMap;
                    Point playerLastPos = Find.Universe.WorldMap.AssocietatedMap.LastPlayerPosition;
                    Locator.GetService<MessageBusService>().SendMessage<ChangeControlledActorMap>(new(map, playerLastPos, currentMap));
                    Locator.GetService<SavingService>().SaveChunkInPos(Find.Universe.CurrentChunk,
                        Find.Universe.CurrentChunk.ToIndex(map.Width));
                    Find.Universe.CurrentChunk = null!;
                    return true;
                }
                else if (Find.Universe.MapIsWorld())
                {
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can't go to the overworld since you are there!"));
                    return false;
                }
                else if (possibleStairs is null && !Find.Universe.MapIsWorld())
                {
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can't go up here!"));
                    return false;
                }
                else
                {
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can't exit the map!"));
                    return false;
                }
            }
            else
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You can't change the map right now!"));
                return false;
            }
        }

        public static Need Sleep(Actor actor, Need need)
        {
            if (actor.State != ActorState.Sleeping)
            {
                if (actor.Flags.Contains(SpecialFlag.NeedsConfortToSleep))
                {
                    // TODO: define behavior for civilized races here in the future!
                    //map.GetFurnitureTypeClosest()
                }
                actor.State = ActorState.Sleeping;
            }
            else
            {
                if (need.TurnCounter-- == 0)
                {
                    actor.State = ActorState.Normal;
                }
            }
            return need;
        }

        public static bool WaitForNTurns(int turns, Actor actor, bool canBeInterrupted = true)
        {
            if (turns < 0)
                return false;
            for (int i = 0; i < turns; i++)
            {
                foreach (Point p in actor.MagiMap.PlayerFOV.NewlySeen)
                {
                    Actor? possibleActor = actor.MagiMap.GetEntityAt<Actor>(p);
                    if (possibleActor?.Equals(actor) == false && canBeInterrupted)
                    {
                        Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is an enemy in view, stop resting!"));
                        return false;
                    }
                }

                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Wait, true));
            }
            return true;
        }

        public static Need? FindFood(Actor actor, MagiMap map)
        {
            var whatToEat = actor.GetAnatomy().WhatToEat();
            var foodItem = map.FindTypeOfFood(whatToEat, actor);
            Need? commitedToNeed = null;
            if (foodItem is null)
            {
                Wander(actor);
                return null;
            }
            if (foodItem is Actor victim)
            {
                commitedToNeed = new Need($"Kill {victim}", false, 0, Actions.Fight, "Peace", $"eat {victim.ID}")
                {
                    Objective = actor,
                };
            }
            if (foodItem is Item item)
            {
                commitedToNeed = new Need($"Pickup {item.Name}", false, 0, Actions.PickUp, "Greed", $"eat {item.ID}")
                {
                    Objective = foodItem
                };
            }
            if (foodItem is Tile tilePlant && tilePlant.GetComponent<Plant>(out var plant))
            {
                commitedToNeed = new Need($"Eat {plant.Name}", false, 0, Actions.Eat, "Greed", $"eat {tilePlant.ID}")
                {
                    Objective = foodItem
                };
            }
            return commitedToNeed;
        }

        public static int Wander(Actor actor)
        {
            int tries = 0;
            bool tileIsInvalid;
            do
            {
                var posToGo = PointUtils.GetPointNextToWithCardinals();
                tileIsInvalid = !MoveActorBy(actor, posToGo);
                tries++;
            } while (tileIsInvalid && tries <= maxTries);
            return TimeHelper.GetWalkTime(actor, actor.Position);
        }

        public static int Drink(Actor actor,
            Material material,
            int temperature,
            Need? needSatisfied = null)
        {
            if (material.GetState(temperature) == MaterialState.Liquid)
            {
                if (needSatisfied != null)
                    needSatisfied?.Fulfill();
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"The {actor.Name} drank {material.Name}"));
                return TimeHelper.Interact;
            }
            return 0;
        }

        public static int Eat(Actor actor, IGameObject whatToEat, Need? need = null)
        {
            if (!actor.CanInteract)
                return 0;
            if (whatToEat is Item item)
            {
                if (need is not null)
                    need?.Fulfill();
                item.Condition -= 100;
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"The {actor.Name} ate {item.Name}"));
            }
            if (whatToEat is Tile tile && tile.GetComponent<Plant>(out var plant))
            {
                if (need is not null)
                    need?.Fulfill();
                if (--plant!.Bundle == 0)
                {
                    tile.RemoveComponent(plant);
                }
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"The {actor.Name} ate {plant.Name}"));
            }
            return TimeHelper.Interact;
        }

        public static Path FindFleeAction(MagiMap map, Actor actor, Actor? danger)
        {
#if DEBUG
            Locator.GetService<MagiLog>().Log($"{actor.Name} considers {danger?.Name} dangerous!");
#endif

            Point rngPoint = map.GetRandomWalkableTile();
            return map.AStar.ShortestPath(actor.Position, rngPoint)!;
        }

        public static bool SearchForDangerAction(Actor actor, MagiMap map, out Actor? danger)
        {
            danger = null;
            if (actor.Mind.Personality.Anger > 20 || actor.Mind.Personality.Peace <= 15)
                return false;
            foreach (var item in actor.AllThatCanSee())
            {
                if (map.EntityIsThere(item, out danger))
                {
                    if (danger is null)
                        continue;
                    if (danger.ID == actor.ID) // ignore if it's the same actor
                        continue;
                    // ignore if it's the same species
                    // TODO: Implement eater of same species
                    if (danger.GetAnatomy().RaceId.Equals(actor.GetAnatomy().RaceId))
                        continue;
                    if (!actor.CanSee(danger.Position))
                        continue;
                    if (danger.Flags.Contains(SpecialFlag.Sapient)) // will need another method to identify sapient creatures
                        continue;
                    var dis = map.DistanceMeasurement.Calculate(actor.Position, danger.Position);

                    bool considersDangerBasedOnSize = (danger.Flags.Contains(SpecialFlag.Predator)
                        && actor.Volume < danger.Volume * 4)
                        || actor.Volume < (danger.Volume * 2) - (actor.Soul.WillPower * actor.Body.Strength);
                    bool getifDangerOnViewNecessaryToworry = dis <= 15 && dis <= actor.GetViewRadius(); // 15 or view radius, whatever is lower.

                    return considersDangerBasedOnSize && getifDangerOnViewNecessaryToworry;
                }
            }
            return false;
        }

        public static bool FindWater(Actor actor, MagiMap map, out Tile water)
        {
            water = map.GetClosestWaterTile(actor.Body.ViewRadius, actor.Position);

            if (water is null)
            {
                if (actor.GetMemory<Tile>(MemoryType.WaterLoc, out var memory))
                {
                    water = memory?.ObjToRemember!;
                    return true;
                }
                return false;
            }

            if (!actor.CanSee(water.Position))
                return false;

            if (!actor.HasMemory(MemoryType.WaterLoc, water))
                actor.AddMemory(water.Position, MemoryType.WaterLoc, water);
            return true;
        }
    }
}

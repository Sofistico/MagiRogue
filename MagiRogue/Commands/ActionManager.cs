using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagiRogue.Components;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Time;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils;
using MagiRogue.Utils.Extensions;
using SadConsole;
using System.Collections.Generic;
using System.Text;
using Map = MagiRogue.GameSys.Map;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Contains all generic actions performed on entities and tiles
    /// including combat, movement, and so on.
    /// </summary>
    public static class ActionManager
    {
        private const int maxTries = 10;

        /// <summary>
        /// Move the actor BY +/- X&Y coordinates
        /// returns true if the move was successful
        /// and false if unable to move there
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool MoveActorBy(Actor actor, Point position)
        {
            return actor.MoveBy(position);
        }

        /// <summary>
        /// Moves the actor To a position, by means of teleport
        /// returns true if the move was successful and false if unable to move
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool MoveActorTo(Actor actor, Point position)
        {
            return actor.MoveTo(position);
        }

        // TODO: An df inspired menu with body parts located.
        /// <summary>
        /// Executes an attack from an attacking actor
        /// on a defending actor, and then describes
        /// the outcome of the attack in the Message Log
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

            if (attacker.Body.Stamina <= 0)
            {
                GameLoop.AddMessageLog($"{attacker.Name} is far too tired to attack!");
                return TimeHelper.Wait;
            }

            // Create two messages that describe the outcome
            // of the attack and defense
            StringBuilder attackMessage = new();
            StringBuilder defenseMessage = new();
            bool isPlayer = attacker is Player;

            (bool hit, BodyPart limbAttacked, BodyPart limbAttacking, DamageTypes dmgType, Item? itemUsed,
                MaterialTemplate attackMaterial)
                = CombatUtils.ResolveHit(attacker, defender, attackMessage, attack, isPlayer, limbChoosen);
            double finalMomentum = CombatUtils.ResolveDefenseAndGetAttackMomentum(attacker,
                defender,
                hit,
                limbAttacking,
                attack,
                itemUsed);

            // Display the outcome of the attack & defense
            GameLoop.AddMessageLog(attackMessage.ToString(), false);
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                GameLoop.AddMessageLog(defenseMessage.ToString(), false);

            // The defender now takes damage
            CombatUtils.ResolveDamage(defender,
                attacker,
                finalMomentum,
                dmgType,
                limbAttacked,
                attackMaterial,
                attack,
                itemUsed,
                limbAttacking);
            var staminaDiscount = (attacker.Body.Stamina
                - (attack.PrepareVelocity * 10)) + (attacker.Body.Endurance * 0.5);
            // discount stamina from the attacker
            attacker.Body.Stamina = MathMagi.Round(staminaDiscount);

            return TimeHelper.GetAttackTime(attacker, attack);
        }

        /// <summary>
        /// Removes an Actor that has died
        /// and displays a message showing
        /// the actor that has died, and the loot they dropped
        /// </summary>
        /// <param name="actor"></param>
        public static void ResolveDeath(Actor actor)
        {
            CombatUtils.ResolveDeath(actor);
        }

        /// <summary>
        /// Gets
        /// </summary>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public static bool DirectAttack(Actor attacker)
        {
            // Lists all monsters that are close and their locations
            List<Actor> monsterClose = new List<Actor>();

            // Saves all Points directions of the attacker.
            Point[] directions = attacker.Position.GetDirectionPoints();

            foreach (Point direction in directions)
            {
                Actor monsterLocation = GameLoop.GetCurrentMap().GetEntityAt<Actor>(direction);

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

        // Tries to pick up an Item and add it to the Actor's
        // inventory list
        public static bool PickUp(Actor actor, Item item)
        {
            // Add the item to the Actor's inventory list
            // and then destroy it
            if (item != null)
            {
                actor.Inventory.Add(item);
                GameLoop.AddMessageLog($"{actor.Name} picked up {item.Name}");
                item.RemoveFromMap();
                return true;
            }
            else
            {
                GameLoop.AddMessageLog("There are no itens here");
                return false;
            }
        }

        /// <summary>
        /// Triggered when an Actor attempts to move into a doorway.
        /// A closed door opens when used by an Actor, it takes a full turn because there can be 2 combination, locked and
        /// unlocked, and im lazy to properly separate the time taken.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="door"></param>
        /// <returns></returns>
        public static bool UseDoor(Actor actor, TileDoor door)
        {
            // Handle a locked door
            if (door.Locked)
            {
                GameLoop.AddMessageLog("The door is locked!");
            }

            // Handled an unlocked door that is closed
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.AddMessageLog($"{actor.Name} opened a {door.Name}");
                GameLoop.GetCurrentMap().ForceFovCalculation();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes an open door
        /// </summary>
        /// <param name="actor">Actor that closes the door</param>
        /// <param name="door">Door that wil be closed></param>
        public static bool CloseDoor(Actor actor)
        {
            foreach (Point points in actor.Position.GetDirectionPoints())
            {
                TileDoor possibleDoor = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(points);
                if (possibleDoor?.IsOpen == true)
                {
                    possibleDoor.Close();
                    GameLoop.AddMessageLog($"{actor.Name} closed a {possibleDoor.Name}");
                    GameLoop.GetCurrentMap().ForceFovCalculation();
                    return true;
                }
            }
            return false;
        }

        public static bool DropItems(Actor inv)
        {
            if (inv.Inventory.Count == 0)
            {
                GameLoop.AddMessageLog("There is no item to drop in your inventory");
                return false;
            }
            else
            {
                Item item = inv.Inventory[0];
                inv.Inventory.Remove(item);
                item.Position = inv.Position;
                GameLoop.GetCurrentMap().AddMagiEntity(item);
                GameLoop.AddMessageLog($"{inv.Name} dropped {item.Name}");
                return true;
            }
        }

        public static bool NodeDrain(Actor actor)
        {
            Point[] direction = actor.Position.GetDirectionPoints();

            foreach (Point item in direction)
            {
                if (GameLoop.GetCurrentMap().Tiles[item.ToIndex(GameLoop.GetCurrentMap().Width)] is NodeTile node)
                {
                    node.DrainNode(actor);
                    return true;
                }
            }
            GameLoop.AddMessageLog("No node here to drain");
            return false;
        }

#if DEBUG

        public static void ToggleFOV()
        {
            if (GameLoop.GetCurrentMap().GoRogueComponents.GetFirstOrDefault<FOVHandler>().IsEnabled)
            {
                GameLoop.GetCurrentMap().GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.Disable(false);
            }
            else
            {
                GameLoop.GetCurrentMap().GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.Enable();
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

            GameLoop.Universe.WorldMap = new PlanetGenerator().CreatePlanet(500, 500);
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

                GameLoop.AddMessageLog($"You have rested for {totalTurnsWait} turns");

                return sus;
            }

            GameLoop.AddMessageLog("You have no need to rest");

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
            Furniture possibleStairs =
                GameLoop.GetCurrentMap().GetEntityAt<Furniture>(playerPoint);
            WorldTile? possibleWorldTileHere =
                GameLoop.GetCurrentMap().GetTileAt<WorldTile>(playerPoint);
            Map currentMap = GameLoop.GetCurrentMap();
            if (possibleStairs?.MapIdConnection.HasValue == true)
            {
                Map map = Universe.GetMapById(possibleStairs.MapIdConnection.Value);
                // TODO: For now it's just a test, need to work out a better way to do it.
                GameLoop.Universe.ChangePlayerMap(map, map.GetRandomWalkableTile(), currentMap);
                GameLoop.Universe.ZLevel -= map.ZAmount;

                return true;
            }
            else if (possibleStairs is null && possibleWorldTileHere is null)
            {
                GameLoop.AddMessageLog("There is no way to go down from here!");
                return false;
            }

            if (possibleWorldTileHere?.Visited == false)
            {
                possibleWorldTileHere.Visited = true;

                RegionChunk chunk = GameLoop.Universe.GenerateChunck(playerPoint);
                GameLoop.Universe.CurrentChunk = chunk;
                GameLoop.Universe.ChangePlayerMap(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].GetRandomWalkableTile(), currentMap);
                return true;
            }
            else if (possibleWorldTileHere?.Visited == true)
            {
                RegionChunk chunk = GameLoop.Universe.GetChunckByPos(playerPoint);
                GameLoop.Universe.CurrentChunk = chunk;
                // if entering the map again, set to update
                chunk.SetMapsToUpdate();
                GameLoop.Universe.ChangePlayerMap(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].LastPlayerPosition, currentMap);

                return true;
            }
            else
            {
                GameLoop.AddMessageLog("There is nowhere to go!");
                return false;
            }
        }

        public static bool EnterUpMovement(Point playerPoint)
        {
            bool possibleChangeMap = GameLoop.Universe.PossibleChangeMap;
            Furniture possibleStairs =
                GameLoop.GetCurrentMap().GetEntityAt<Furniture>(playerPoint);
            Map currentMap = GameLoop.GetCurrentMap();

            if (possibleChangeMap)
            {
                if (possibleStairs is not null && !GameLoop.Universe.MapIsWorld()
                    && possibleStairs.MapIdConnection.HasValue)
                {
                    Map map = Universe.GetMapById(possibleStairs.MapIdConnection.Value);
                    // TODO: For now it's just a test, need to work out a better way to do it.
                    GameLoop.Universe.ChangePlayerMap(map, map.GetRandomWalkableTile(), currentMap);
                    GameLoop.Universe.ZLevel += map.ZAmount;

                    return true;
                }
                else if (!GameLoop.Universe.MapIsWorld())
                {
                    Map map = GameLoop.Universe.WorldMap.AssocietatedMap;
                    Point playerLastPos = GameLoop.Universe.WorldMap.AssocietatedMap.LastPlayerPosition;
                    GameLoop.Universe.ChangePlayerMap(map, playerLastPos, currentMap);
                    GameLoop.Universe.SaveAndLoad.SaveChunkInPos(GameLoop.Universe.CurrentChunk,
                        GameLoop.Universe.CurrentChunk.ToIndex(map.Width));
                    GameLoop.Universe.CurrentChunk = null!;
                    return true;
                }
                else if (GameLoop.Universe.MapIsWorld())
                {
                    GameLoop.AddMessageLog("Can't go to the overworld since you are there!");
                    return false;
                }
                else if (possibleStairs is null && !GameLoop.Universe.MapIsWorld())
                {
                    GameLoop.AddMessageLog("Can't go up here!");
                    return false;
                }
                else
                {
                    GameLoop.AddMessageLog("Can't exit the map!");
                    return false;
                }
            }
            else
            {
                GameLoop.AddMessageLog("You can't change the map right now!");
                return false;
            }
        }

        public static Need Sleep(Actor actor, Need need)
        {
            if (actor.State != ActorState.Sleeping)
            {
                if (actor.Flags.Contains(SpecialFlag.NeedsConfortToSleep))
                {
                    // define behavior for civilized races here in the future!
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
                foreach (Point p in GameLoop.GetCurrentMap().PlayerFOV.NewlySeen)
                {
                    Actor possibleActor = GameLoop.GetCurrentMap().GetEntityAt<Actor>(p);
                    if (possibleActor?.Equals(actor) == false && canBeInterrupted)
                    {
                        GameLoop.AddMessageLog("There is an enemy in view, stop resting!");
                        return false;
                    }
                }

                GameLoop.Universe.ProcessTurn(TimeHelper.Wait, true);
            }
            return true;
        }

        public static Need? FindFood(Actor actor, Map map)
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
                commitedToNeed = new Need($"Kill {victim}", false, 0, Data.Enumerators.Actions.Fight, "Peace", $"eat {victim.ID}")
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
            if (foodItem is Plant plant)
            {
                commitedToNeed = new Need($"Eat {plant.Name}", false, 0, Actions.Eat, "Greed", $"eat {plant.ID}")
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
            MaterialTemplate material,
            int temperature,
            Need? needSatisfied = null)
        {
            if (material.GetState(temperature) == MaterialState.Liquid)
            {
                if (needSatisfied != null)
                    needSatisfied?.Fulfill();
                GameLoop.AddMessageLog($"The {actor.Name} drank {material.Name}");
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
                GameLoop.AddMessageLog($"The {actor.Name} ate {item.Name}");
            }
            if (whatToEat is Plant plant)
            {
                if (need is not null)
                    need?.Fulfill();
                plant.RemoveThisFromMap();
                GameLoop.AddMessageLog($"The {actor.Name} ate {plant.Name}");
            }
            return TimeHelper.Interact;
        }

        public static Path FindFleeAction(Map map, Actor actor, Actor? danger)
        {
#if DEBUG
            GameLoop.AddMessageLog($"{actor.Name} considers {danger.Name} dangerous!");
#endif

            Point rngPoint = map.GetRandomWalkableTile();
            return map.AStar.ShortestPath(actor.Position, rngPoint);
        }

        public static bool SearchForDangerAction(Actor actor, Map map, out Actor? danger)
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

        public static bool FindWater(Actor actor, Map map, out WaterTile water)
        {
            water = map.GetClosestWaterTile(actor.Body.ViewRadius, actor.Position);

            if (water is null)
            {
                if (actor.GetMemory<WaterTile>(MemoryType.WaterLoc, out var memory))
                {
                    water = memory.ObjToRemember!;
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
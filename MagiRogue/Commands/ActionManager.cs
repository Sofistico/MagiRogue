using GoRogue.DiceNotation;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Time;
using MagiRogue.Utils;
using SadConsole;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Contains all generic actions performed on entities and tiles
    /// including combat, movement, and so on.
    /// </summary>
    public sealed class ActionManager
    {
        private const int staminaAttackAction = 100;

        private ActionManager()
        {
            // makes sure that it's never instantiated
        }

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
        public static void MeleeAttack(Actor attacker, Actor defender)
        {
            if (!defender.CanBeAttacked)
                return;

            if (!attacker.GetAnatomy().HasEnoughArms)
            {
                GameLoop.AddMessageLog
                    ($"The {attacker.Name} doesn't have enough arms to hit {defender.Name}");
                return;
            }

            if (attacker.Body.Stamina <= 0)
            {
                GameLoop.AddMessageLog($"{attacker.Name} is far too tired to attack!");
                return;
            }

            // Create two messages that describe the outcome
            // of the attack and defense
            StringBuilder attackMessage = new();
            StringBuilder defenseMessage = new();

            // Count up the amount of attacking damage done
            // and the number of successful blocks
            (bool hit, Limb limbAttacked, Limb limbAttacking, DamageTypes dmgType)
                = CombatUtils.ResolveHit(attacker, defender, attackMessage);
            double damage = CombatUtils.ResolveDefense(attacker,
                defender, hit, attackMessage, defenseMessage, limbAttacked, dmgType, limbAttacking);

            // Display the outcome of the attack & defense
            GameLoop.AddMessageLog(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                GameLoop.AddMessageLog(defenseMessage.ToString());

            // The defender now takes damage
            CombatUtils.ResolveDamage(defender, damage, dmgType, limbAttacking, limbAttacked);

            // discount stamina from the attacker
            attacker.Body.Stamina =
                MathMagi.Round((attacker.Body.Stamina - staminaAttackAction) * (attacker.Body.Endurance / 100));
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
            Point[] directions = SadConsole.PointExtensions.GetDirectionPoints(attacker.Position);

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
                        Actor closestMonster = monsterClose.First();
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
            Point[] allDirections = SadConsole.PointExtensions.GetDirectionPoints(actor.Position);
            foreach (Point points in allDirections)
            {
                TileDoor possibleDoor = GameLoop.GetCurrentMap().GetTileAt<TileDoor>(points);
                if (possibleDoor != null)
                {
                    if (possibleDoor.IsOpen)
                    {
                        possibleDoor.Close();
                        GameLoop.AddMessageLog($"{actor.Name} closed a {possibleDoor.Name}");
                        GameLoop.GetCurrentMap().ForceFovCalculation();
                        return true;
                    }
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
                Item item = inv.Inventory.First();
                inv.Inventory.Remove(item);
                item.Position = inv.Position;
                GameLoop.GetCurrentMap().Add(item);
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
                GameLoop.GetCurrentMap().GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable(false);
            }
            else
                GameLoop.GetCurrentMap().GoRogueComponents.GetFirstOrDefault<FOVHandler>().Enable();
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
            Actor found = EntityFactory.ActorCreatorFirstStep(pos,
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
            Mind mindStats = actor.Mind;
            Soul soulStats = actor.Soul;

            if ((bodyStats.Stamina < bodyStats.MaxStamina || soulStats.CurrentMana < soulStats.MaxMana))
            {
                // calculate here the amount of time that it will take in turns to rest to full
                double staminaDif, manaDif;

                staminaDif = MathMagi.Round((bodyStats.MaxStamina - bodyStats.Stamina) / actor.GetStaminaRegen());
                manaDif = MathMagi.Round((soulStats.MaxMana - soulStats.CurrentMana) / actor.GetManaRegen());

                double totalTurnsWait = staminaDif + manaDif;

                for (int i = 0; i < totalTurnsWait + 1; i++)
                {
                    foreach (Point p in GameLoop.GetCurrentMap().PlayerFOV.CurrentFOV)
                    {
                        Actor possibleActor = GameLoop.GetCurrentMap().GetEntityAt<Actor>(p);
                        if (possibleActor is not null && !possibleActor.Equals(actor))
                        {
                            GameLoop.AddMessageLog("There is an enemy in view, stop resting!");
                            return false;
                        }
                    }

                    GameLoop.Universe.ProcessTurn(TimeHelper.Wait, true);
                }

                GameLoop.AddMessageLog($"You have rested for {totalTurnsWait} turns");

                return true;
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
            if (actor.GetEquipment().ContainsValue(item))
                return true;
            else
                return false;
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
            if (!actor.GetEquipment().ContainsValue(item))
                return true;
            else
                return false;
        }

        public static bool EnterDownMovement(Point playerPoint)
        {
            Furniture possibleStairs =
                GameLoop.GetCurrentMap().GetEntityAt<Furniture>(playerPoint);
            WorldTile? possibleWorldTileHere =
                GameLoop.GetCurrentMap().GetTileAt<WorldTile>(playerPoint);
            Map currentMap = GameLoop.GetCurrentMap();
            if (possibleStairs is not null
                && possibleStairs.MapIdConnection.HasValue)
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

            if (possibleWorldTileHere is not null && !possibleWorldTileHere.Visited)
            {
                possibleWorldTileHere.Visited = true;

                RegionChunk chunk = GameLoop.Universe.GenerateChunck(playerPoint);
                GameLoop.Universe.CurrentChunk = chunk;
                GameLoop.Universe.ChangePlayerMap(chunk.LocalMaps[0],
                    chunk.LocalMaps[0].GetRandomWalkableTile(), currentMap);
                return true;
            }
            else if (possibleWorldTileHere.Visited)
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
                    GameLoop.Universe.CurrentChunk = null;
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
    }
}
using GoRogue.DiceNotation;
using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Time;
using MagiRogue.Utils;
using SadConsole;
using SadRogue.Primitives;
using System;
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
    public sealed class CommandManager
    {
        private CommandManager()
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
        public static void Attack(Actor attacker, Actor defender)
        {
            if (!defender.CanBeAttacked)
                return;

            if (!attacker.Anatomy.HasEnoughArms)
            {
                GameLoop.UIManager.MessageLog.Add
                    ($"The {attacker.Name} doesn't have enough arms to hit {defender.Name}");
                return;
            }

            // Create two messages that describe the outcome
            // of the attack and defense
            StringBuilder attackMessage = new();
            StringBuilder defenseMessage = new();

            // Count up the amount of attacking damage done
            // and the number of successful blocks
            int hits = ResolveHit(attacker, defender, attackMessage);
            int damage = ResolveDefense(attacker, defender, hits, attackMessage, defenseMessage);

            // Display the outcome of the attack & defense
            GameLoop.UIManager.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                GameLoop.UIManager.MessageLog.Add(defenseMessage.ToString());

            // The defender now takes damage
            ResolveDamage(defender, damage, DamageType.None);
        }

        /// <summary>
        /// Calculates the outcome of an attacker's attempt
        /// at scoring a hit on a defender, using the attacker's
        /// AttackChance and a random d100 roll as the basis.
        /// Modifies a StringBuilder message that will be displayed
        /// in the MessageLog
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="attackMessage"></param>
        /// <returns></returns>
        private static int ResolveHit(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            // Create a string that expresses the attacker and defender's names
            int hits = 0;
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);

            // The attacker's AttackSpeed value determines the number of attacks dice rolled
            for (int nmrAttack = 0; nmrAttack < attacker.Stats.AttackSpeed; nmrAttack++)
            {
                //Resolve the dicing outcome and register a hit, governed by the
                //attacker's BaseAttack value.
                //TODO: Adds the fatigue atribute and any penalties.
                if (attacker.Stats.BaseAttack + Mrn.Exploding2D6Dice > defender.Stats.Defense + Mrn.Exploding2D6Dice)
                    hits++;
            }

            return hits;
        }

        /// <summary>
        /// Calculates the outcome of a defender's attempt
        /// at blocking incoming hits.
        /// Modifies a StringBuilder messages that will be displayed
        /// in the MessageLog, expressing the number of hits blocked.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="hits"></param>
        /// <param name="attackMessage"></param>
        /// <param name="defenseMessage"></param>
        /// <returns></returns>
        private static int ResolveDefense(Actor attacker, Actor defender, int hits, StringBuilder attackMessage,
            StringBuilder defenseMessage)
        {
            int totalDamage = 0;

            if (hits > 0)
            {
                // Create a string that displays the defender's name and outcomes
                attackMessage.AppendFormat(" scoring {0} hits.", hits);

                for (int i = 0; i < hits; i++)
                {
                    int loopDamage;
                    Item wieldedItem = attacker.WieldedItem();
                    // TODO: adds a way to get the attack of the weapon or fist or something else
                    if (wieldedItem is null)
                        loopDamage = attacker.Stats.Strength + Mrn.Exploding2D6Dice;
                    else
                        loopDamage = attacker.Stats.Strength + wieldedItem.BaseDmg + Mrn.Exploding2D6Dice;

                    loopDamage -= defender.Stats.Protection + Mrn.Exploding2D6Dice;

                    defenseMessage.AppendFormat("   Hit {0}: {1} was hit for {2} damage", hits, defender.Name, loopDamage);
                    totalDamage += loopDamage;
                }
            }
            else
                attackMessage.Append(" and misses completely!");

            return totalDamage;
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        private static void ResolveDamage(Actor defender, int damage, DamageType dmgType)
        {
            if (damage > 0)
            {
                CombatUtils.DealDamage(damage, defender, dmgType);
            }
            else
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} blocked all damage!");
        }

        /// <summary>
        /// Removes an Actor that has died
        /// and displays a message showing
        /// the actor that has died, and the loot they dropped
        /// </summary>
        /// <param name="defender"></param>
        public static void ResolveDeath(Actor defender)
        {
            // if the defender can't be killed, do nothing.
            if (!defender.CanBeKilled)
                return;

            // Set up a customized death message
            StringBuilder deathMessage = new StringBuilder();

            // dump the dead actor's inventory (if any)
            // at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                deathMessage.AppendFormat("{0} died and dropped", defender.Name);

                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    GameLoop.GetCurrentMap().Add(item);

                    // Append the item to the deathMessage
                    deathMessage.Append(", " + item.Name);
                }

                // Clear the actor's inventory. Not strictly
                // necessary, but makes for good coding habits!
                defender.Inventory.Clear();
            }
            else
            {
                // The monster carries no loot, so don't show any loot dropped
                deathMessage.Append('.');
            }

            // actor goes bye-bye
            GameLoop.GetCurrentMap().Remove(defender);

            if (defender is Player)
            {
                GameLoop.UIManager.MessageLog.Add($" {defender.Name} was killed.");
            }

            // Now show the deathMessage in the messagelog
            GameLoop.UIManager.MessageLog.Add(deathMessage.ToString());
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
                        Attack(attacker, closestMonster);
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
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} picked up {item.Name}");
                item.RemoveFromMap();
                return true;
            }
            else
            {
                GameLoop.UIManager.MessageLog.Add("There is no item here");
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
                GameLoop.UIManager.MessageLog.Add("The door is locked!");
            }

            // Handled an unlocked door that is closed
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} opened a {door.Name}");
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
                        GameLoop.UIManager.MessageLog.Add($"{actor.Name} closed a {possibleDoor.Name}");
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
                GameLoop.UIManager.MessageLog.Add("There is no item to drop in your inventory");
                return false;
            }
            else
            {
                Item item = inv.Inventory.First();
                inv.Inventory.Remove(item);
                item.Position = inv.Position;
                GameLoop.GetCurrentMap().Add(item);
                GameLoop.UIManager.MessageLog.Add($"{inv.Name} dropped {item.Name}");
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
            GameLoop.UIManager.MessageLog.Add("No node here to drain");
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

        public static bool CreateTestEntity(Point pos, Map map)
        {
            Actor found = EntityFactory.ActorCreator(pos,
                 DataManager.QueryActorInData("test_troll"));
            map.Add(found);
            return true;
        }

#endif

        public static bool SacrificeLifeEnergyToMana(Actor actor)
        {
            int maxMana = actor.Stats.BodyStat + actor.Stats.SoulStat + actor.Stats.MindStat;
            if (actor.Stats.PersonalMana != maxMana)
            {
                actor.Stats.Health -= 1;
                actor.Anatomy.BloodCount -= 100f;
                int roll = Dice.Roll("1d3");
                float bloodyManaGained = float.Parse($"0.{roll}", CultureInfo.InvariantCulture.NumberFormat);
                actor.Stats.PersonalMana = MathMagi.Round(actor.Stats.PersonalMana + bloodyManaGained);
                GameLoop.UIManager.MessageLog.Add($"You ritually wound yourself, channeling your blood into mana, gaining {bloodyManaGained} blood mana");
                return true;
            }

            GameLoop.UIManager.MessageLog.Add("You feel too full for this right now");
            return false;
        }

        public static bool RestTillFull(Actor actor)
        {
            Stat stats = actor.Stats;

            if ((stats.Health < stats.MaxHealth || stats.PersonalMana < stats.MaxPersonalMana))
            {
                // calculate here the amount of time that it will take in turns to rest to full
                float healDif, manaDif;

                healDif = MathMagi.Round((stats.MaxHealth - stats.Health) / stats.BaseHpRegen);
                manaDif = MathMagi.Round((stats.MaxPersonalMana - stats.PersonalMana) / stats.BaseManaRegen);

                float totalTurnsWait = healDif + manaDif;

                for (int i = 0; i < totalTurnsWait + 1; i++)
                {
                    foreach (Point p in GameLoop.GetCurrentMap().PlayerFOV.CurrentFOV)
                    {
                        Actor possibleActor = GameLoop.GetCurrentMap().GetEntityAt<Actor>(p);
                        if (possibleActor is not null && !possibleActor.Equals(actor))
                        {
                            GameLoop.UIManager.MessageLog.Add("There is an enemy in view, stop resting!");
                            return false;
                        }
                    }

                    GameLoop.Universe.ProcessTurn(TimeHelper.Wait, true);
                }

                GameLoop.UIManager.MessageLog.Add($"You have rested for {totalTurnsWait} turns");

                return true;
            }

            GameLoop.UIManager.MessageLog.Add("You have no need to rest");

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
            if (actor.Equipment.ContainsValue(item))
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
            if (!actor.Equipment.ContainsValue(item))
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
                GameLoop.UIManager.MessageLog.Add("There is no way to go down here!");
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
                GameLoop.UIManager.MessageLog.Add("There is nowhere to go!");
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
                    GameLoop.UIManager.MessageLog.Add("Can't go to the overworld since you are there!");
                    return false;
                }
                else if (possibleStairs is null && !GameLoop.Universe.MapIsWorld())
                {
                    GameLoop.UIManager.MessageLog.Add("Can't go up here!");
                    return false;
                }
                else
                {
                    GameLoop.UIManager.MessageLog.Add("Can't exit the map!");
                    return false;
                }
            }
            else
            {
                GameLoop.UIManager.MessageLog.Add("You can't change the map right now!");
                return false;
            }
        }
    }
}
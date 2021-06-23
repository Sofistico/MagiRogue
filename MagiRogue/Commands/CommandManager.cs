using GoRogue.DiceNotation;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MagiRogue.System;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Contains all generic actions performed on entities and tiles
    /// including combat, movement, and so on.
    /// </summary>
    public class CommandManager
    {
        protected CommandManager()
        {
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

            // Create two messages that describe the outcome
            // of the attack and defense
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            // Count up the amount of attacking damage done
            // and the number of successful blocks
            int hits = ResolveAttack(attacker, defender, attackMessage);
            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            // Display the outcome of the attack & defense
            GameLoop.UIManager.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
                GameLoop.UIManager.MessageLog.Add(defenseMessage.ToString());

            int damage = hits - blocks;

            // The defender now takes damage
            ResolveDamage(defender, damage);
        }

        public static void DealDamage(Actor defender, int damage)
        {
            ResolveDamage(defender, damage);
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
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            // Create a string that expresses the attacker and defender's names
            int hits = 0;
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);

            // The attacker's Attack value determines the number of D100 dice rolled
            for (int dice = 0; dice < attacker.Stats.Attack; dice++)
            {
                //Roll a single D100 and add its results to the attack Message
                int diceOutcome = Dice.Roll("1d100");

                //Resolve the dicing outcome and register a hit, governed by the
                //attacker's AttackChance value.
                if (diceOutcome >= 100 - attacker.Stats.AttackChance)
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
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                // Create a string that displays the defender's name and outcomes
                attackMessage.AppendFormat(" scoring {0} hits.", hits);
                defenseMessage.AppendFormat(" {0} defends and rolls: ", defender.Name);

                //The defender's Defense value determines the number of D100 dice rolled
                for (int dice = 0; dice < defender.Stats.Defense; dice++)
                {
                    //Roll a single D100 and add its results to the defense Message
                    int diceOutcome = Dice.Roll("1d100");

                    //Resolve the dicing outcome and register a block, governed by the
                    //attacker's DefenceChance value.
                    if (diceOutcome >= 100 - defender.Stats.DefenseChance)
                        blocks++;
                }

                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
                attackMessage.Append(" and misses completely!");

            return blocks;
        }

        /// <summary>
        /// Calculates the damage a defender takes after a successful hit
        /// and subtracts it from its Health
        /// Then displays the outcome in the MessageLog.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Stats.Health -= damage;
                GameLoop.UIManager.MessageLog.Add($" {defender.Name} was hit for {damage} damage");
                if (defender.Stats.Health < 0)
                    ResolveDeath(defender);
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
                deathMessage.Append("died and dropped");

                foreach (Item item in defender.Inventory)
                {
                    // move the Item to the place where the actor died
                    item.Position = defender.Position;

                    // Now let the MultiSpatialMap know that the Item is visible
                    GameLoop.World.CurrentMap.Add(item);

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
            GameLoop.World.CurrentMap.Remove(defender);

            if (defender is Player)
                GameLoop.UIManager.MessageLog.Add($" {defender.Name} was killed.");

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
                Actor monsterLocation = GameLoop.World.CurrentMap.GetEntityAt<Actor>(direction);

                if (monsterLocation != null)
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
                item.Destroy();
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
                // TODO: make it possible to unlock a door though magic or magic of lockpicks.
            }

            // Handled an unlocked door that is closed
            else if (!door.Locked && !door.IsOpen)
            {
                door.Open();
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} opened a {door.Name}");
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
                TileDoor possibleDoor = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(points);
                if (possibleDoor != null)
                {
                    if (possibleDoor.IsOpen)
                    {
                        possibleDoor.Close();
                        GameLoop.UIManager.MessageLog.Add($"{actor.Name} closed a {possibleDoor.Name}");
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
                GameLoop.World.CurrentMap.Add(item);
                GameLoop.UIManager.MessageLog.Add($"{inv.Name} dropped {item.Name}");
                return true;
            }
        }

#if DEBUG

        public static void ToggleFOV()
        {
            if (GameLoop.World.CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().IsEnabled)
            {
                GameLoop.World.CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable(false);
            }
            else
                GameLoop.World.CurrentMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().Enable();
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
                actor.Stats.PersonalMana = (float)Math.Round(actor.Stats.PersonalMana + bloodyManaGained, 1);
                GameLoop.UIManager.MessageLog.Add($"You ritually wound yourself, channeling your blood into mana, gaining {bloodyManaGained} blood mana");
                return true;
            }

            GameLoop.UIManager.MessageLog.Add("You feel too full for this right now");
            return false;
        }
    }
}
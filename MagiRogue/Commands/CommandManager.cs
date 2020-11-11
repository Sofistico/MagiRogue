using GoRogue.DiceNotation;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiRogue.Commands
{
    // Contains all generic actions performed on entities and tiles
    // including combat, movement, and so on.
    public class CommandManager
    {
        // Pathfinding field
        public Pathfinding Pathfinding { get; set; }

        public CommandManager()
        {
            // Instantietes the pathfinding field so that all actors can acess it
            //pathfinding = new Pathfinding();
        }

        // Move the actor BY +/- X&Y coordinates
        // returns true if the move was successful
        // and false if unable to move there
        public bool MoveActorBy(Actor actor, Point position)
        {
            return actor.MoveBy(position);
        }

        // Moves the actor To a position, by means of teleport
        // returns true if the move was successful and false if unable to move
        public bool MoveActorTo(Actor actor, Point position)
        {
            return actor.MoveTo(position);
        }

        public bool FollowPlayer(Actor actor)
        {
            if (GameLoop.World.Player != null) // Implement a way for an actor to check if the player is near, so that it can follow the player
            {
                return Pathfinding.WalkPath(GameLoop.World.Player.Position, actor);
            }
            else
            {
                return false;
            }
        }

        // An method that when a button is pressed, attacks the monster.
        // TODO: An df inspired menu with body parts located.

        // Executes an attack from an attacking actor
        // on a defending actor, and then describes
        // the outcome of the attack in the Message Log
        public void Attack(Actor attacker, Actor defender)
        {
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

        // Calculates the outcome of an attacker's attempt
        // at scoring a hit on a defender, using the attacker's
        // AttackChance and a random d100 roll as the basis.
        // Modifies a StringBuilder message that will be displayed
        // in the MessageLog
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            // Create a string that expresses the attacker and defender's names
            int hits = 0;
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);

            // The attacker's Attack value determines the number of D100 dice rolled
            for (int dice = 0; dice < attacker.Attack; dice++)
            {
                //Roll a single D100 and add its results to the attack Message
                int diceOutcome = Dice.Roll("1d100");

                //Resolve the dicing outcome and register a hit, governed by the
                //attacker's AttackChance value.
                if (diceOutcome >= 100 - attacker.AttackChance)
                    hits++;
            }

            return hits;
        }

        // Calculates the outcome of a defender's attempt
        // at blocking incoming hits.
        // Modifies a StringBuilder messages that will be displayed
        // in the MessageLog, expressing the number of hits blocked.
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                // Create a string that displays the defender's name and outcomes
                attackMessage.AppendFormat(" scoring {0} hits.", hits);
                defenseMessage.AppendFormat(" {0} defends and rolls: ", defender.Name);

                //The defender's Defense value determines the number of D100 dice rolled
                for (int dice = 0; dice < defender.Defense; dice++)
                {
                    //Roll a single D100 and add its results to the defense Message
                    int diceOutcome = Dice.Roll("1d100");

                    //Resolve the dicing outcome and register a block, governed by the
                    //attacker's DefenceChance value.
                    if (diceOutcome >= 100 - defender.DefenseChance)
                        blocks++;
                }

                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
                attackMessage.Append("and misses completely!");

            return blocks;
        }

        // Calculates the damage a defender takes after a successful hit
        // and subtracts it from its Health
        // Then displays the outcome in the MessageLog.
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health -= damage;
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} was hit for {damage} damage");
                if (defender.Health < 0)
                    ResolveDeath(defender);
            }
            else
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} blocked all damage!");
        }

        // Removes an Actor that has died
        // and displays a message showing
        // the actor that has died, and they loot they dropped
        private static void ResolveDeath(Actor defender)
        {
            // Set up a customized death message
            StringBuilder deathMessage = new StringBuilder();

            // dump the dead actor's inventory (if any)
            // at the map position where it died
            if (defender.Inventory.Count > 0)
            {
                deathMessage.Append(" and dropped");

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
                deathMessage.Append(".");
            }

            // actor goes bye-bye
            GameLoop.World.CurrentMap.Remove(defender);

            if (defender is Player)
                GameLoop.UIManager.MessageLog.Add($" {defender.Name} was killed.");

            // Now show the deathMessage in the messagelog
            GameLoop.UIManager.MessageLog.Add(deathMessage.ToString());
        }

        public bool DirectAttack(Actor attacker)
        {
            // Lists all monsters that are close and their locations
            List<Monster> monsterClose = new List<Monster>();

            // Saves all Points directions of the attacker.
            Point[] directions = Directions.GetDirectionPoints(attacker.Position);

            foreach (Point direction in directions)
            {
                Monster monsterLocation = GameLoop.World.CurrentMap.GetEntityAt<Monster>(direction);

                if (monsterLocation != null)
                {
                    monsterClose.Add(monsterLocation);
                    if (monsterClose.Count > 0)
                    {
                        // add logic for attack here
                        // TODO: make it possible to choose which monster to strike and test what happens when there is more
                        // than one monster nearby.
                        Monster closestMonster = monsterClose.First<Monster>();
                        GameLoop.CommandManager.Attack(attacker, closestMonster);
                        return true;
                    }
                }
            }
            // default response
            return false;
        }

        // Tries to pick up an Item and add it to the Actor's
        // inventory list
        public void PickUp(Actor actor, Item item)
        {
            // Add the item to the Actor's inventory list
            // and then destroy it
            if (item != null)
            {
                actor.Inventory.Add(item);
                GameLoop.UIManager.MessageLog.Add($"{actor.Name} picked up {item.Name}");
                item.Destroy();
            }
            else
            {
                GameLoop.UIManager.MessageLog.Add("There is no item here");
            }
        }

        // Triggered when an Actor attempts to move into a doorway.
        // A closed door opens when used by an Actor.
        public void UseDoor(Actor actor, TileDoor door)
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
            }
        }

        public void DropItems(Actor inv)
        {
            try
            {
                Item item = inv.Inventory.First();
                inv.Inventory.Remove(item);
                item.Position = inv.Position;
                GameLoop.World.CurrentMap.Add(item);
            }
            catch (Exception)
            {
                GameLoop.UIManager.MessageLog.Add("There is no item to drop in your inventory");
            }
        }
    }
}
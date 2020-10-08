using MagiRogue.System.Tiles;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MagiRogue.Entities.Materials;

namespace MagiRogue.Entities
{
    public abstract class Actor : Entity
    {
        private int _health;
        private int _maxHealth;
        private int _attack;
        private int _attackChance;
        private int _defense;
        private int _defenseChance;
        private int _bodyStat;
        private int _mindStat;
        private int _soulStat;

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        } // current health

        public int MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        } // maximum health

        public int Attack
        {
            get { return _attack; }
            set
            {
                _attack += BodyStat;
                _attack = value;
            }
        } // attack strength

        public int AttackChance
        {
            get { return _attackChance; }
            set
            {
                _attackChance += BodyStat;
                _attackChance = value;
            }
        } // percent chance of successful hit

        public int Defense
        {
            get { return _defense; }
            set { _defense = value; }
        } // defensive strength

        public int DefenseChance
        {
            get { return _defenseChance; }
            set { _defenseChance = value; }
        } // percent chance of successfully blocking a hit

        public int BodyStat
        {
            get { return _bodyStat; }
            set { _bodyStat = value; }
        }   // The body stat of the actor

        public int MindStat
        {
            get { return _mindStat; }
            set { _mindStat = value; }
        } // The mind stat of the actor

        public int SoulStat
        {
            get { return _soulStat; }
            set { _soulStat = value; }
        } // The soul stat of the actor

        public List<Item> Inventory = new List<Item>(); // the inventory of the actor;

        protected Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(foreground, background,
            glyph, width, height)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        // Moves the Actor BY positionChange tiles in any X/Y direction
        // returns true if actor was able to move, false if failed to move
        // Checks for Monsters, and allows the Actor to commit
        // an action if one is present.
        // TODO: an autopickup feature for items
        public bool MoveBy(Point positionChange)
        {
            // Check the current map if we can move to this new position
            if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                // if there's a monster here,
                // do a bump attack
                Monster monster = GameLoop.World.CurrentMap.GetEntityAt<Monster>(Position + positionChange);
                //Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(Position + positionChange);

                if (monster != null)
                {
                    GameLoop.CommandManager.Attack(this, monster);
                    return true;
                }

                // if there's an item here,
                // try to pick it up
                // implement this like an option
                // if settings allow auto pickup for example
                /*else if (item != null)
                {
                    GameLoop.CommandManager.PickUp(this, item);
                    return true;
                }*/

                Position += positionChange;
                return true;
            }
            // Handle situations where there are non-walkable tiles that CAN be used
            else
            {
                // Check for the presence of a door
                TileDoor door = GameLoop.World.CurrentMap.GetTileAt<TileDoor>(Position + positionChange);

                // if there's a door here,
                // try to use it
                if (door != null)
                {
                    GameLoop.CommandManager.UseDoor(this, door);
                    return true;
                }
                return false;
            }
        }

        // Moves the Actor TO newPosition location
        // returns true if actor was able to move, false if failed to move
        public bool MoveTo(Point newPosition)
        {
            Position = newPosition;
            return true;
        }
    }
}
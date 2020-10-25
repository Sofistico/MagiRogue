using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    public abstract class Actor : Entity
    {
        private int _health; // Will remove in favor of a limb like system
        private int _maxHealth; // Will remove in favor of a limb like system
        private int _mana; // Is to be earned, the limit is soul + mind + body, a more potent form than natural mana
        private int _attack;
        private int _attackChance;
        private int _defense;
        private int _defenseChance;
        private int _bodyStat;
        private int _mindStat;
        private int _soulStat;
        private int _godPower;
        private bool _godly;

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

        public int AmbientMana
        {
            get { return _mana; }
            set
            {
                if (_mana >= _bodyStat + _mindStat + _soulStat)
                    _mana = value;
                else
                    _mana = _bodyStat + _mindStat + _soulStat;
            }
        }

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

        public int GodPower
        {
            get { return _godPower; }
            set
            {
                if (this._godly)
                {
                    _godPower = value;
                }
                else
                {
                    _godPower = 0;
                }
            }
        } //The god stat of the actor, checks if the actor is a god as well

        public bool Godly
        {
            get { return _godly; }
            set { _godly = value; }
        }

        // Formula is soul + (mind*2) + body, this is pure mana made inside the body, it difers from just mana because it's
        // easier to work with and all beings can easily use it, if they train to do it.
        public int NaturalMana
        {
            get { return _bodyStat + (_mindStat * 2) + _soulStat; }
        }

        public List<Item> Inventory = new List<Item>(); // the inventory of the actor;

        protected Actor(Color foreground, Color background, int glyph, int layer, int width = 1, int height = 1) : base(foreground, background,
            glyph, layer, width, height)
        {
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
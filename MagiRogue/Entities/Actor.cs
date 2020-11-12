using GoRogue;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    public abstract class Actor : Entity
    {
        #region Stats

        private int health; // Will remove in favor of a limb like system
        private int maxHealth; // Will remove in favor of a limb like system
        private int mana; // Is to be earned, the limit is soul + mind + body, a more potent form than natural mana
        private int attack;
        private int attackChance;
        private int defense;
        private int defenseChance;
        private int bodyStat;
        private int mindStat;
        private int soulStat;
        private int godPower;
        private bool godly;

        public int Health
        {
            get { return health; }
            set { health = value; }
        } // current health

        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        } // maximum health

        public int AmbientMana
        {
            get { return mana; }
            set
            {
                if (mana >= bodyStat + mindStat + soulStat)
                    mana = value;
                else
                    mana = bodyStat + mindStat + soulStat;
            }
        }

        public int Attack
        {
            get { return attack; }
            set
            {
                attack += BodyStat;
                attack = value;
            }
        } // attack strength

        public int AttackChance
        {
            get { return attackChance; }
            set
            {
                attackChance += BodyStat;
                attackChance = value;
            }
        } // percent chance of successful hit

        public int Defense
        {
            get { return defense; }
            set { defense = value; }
        } // defensive strength

        public int DefenseChance
        {
            get { return defenseChance; }
            set { defenseChance = value; }
        } // percent chance of successfully blocking a hit

        public int BodyStat
        {
            get { return bodyStat; }
            set { bodyStat = value; }
        }   // The body stat of the actor

        public int MindStat
        {
            get { return mindStat; }
            set { mindStat = value; }
        } // The mind stat of the actor

        public int SoulStat
        {
            get { return soulStat; }
            set { soulStat = value; }
        } // The soul stat of the actor

        public int GodPower
        {
            get { return godPower; }
            set
            {
                if (this.godly)
                {
                    godPower = value;
                }
                else
                {
                    godPower = 0;
                }
            }
        } //The god stat of the actor, checks if the actor is a god as well

        public bool Godly
        {
            get { return godly; }
            set { godly = value; }
        }

        // Formula is soul + (mind*2) + body, this is pure mana made inside the body, it difers from just mana because it's
        // easier to work with and all beings can easily use it, if they train to do it.
        public int NaturalMana
        {
            get { return bodyStat + (mindStat * 2) + soulStat; }
        }

        public int ViewRadius { get; set; }

        public FOVSystem FieldOfViewSystem { get; private set; }

        public List<Item> Inventory = new List<Item>(); // the inventory of the actor;

        #endregion Stats

        protected Actor(Color foreground, Color background, int glyph, int layer, int width = 1, int height = 1) : base(foreground, background,
            glyph, layer, width, height)
        {
            FieldOfViewSystem = new FOVSystem(this, Distance.CHEBYSHEV);
        }

        #region HelpCode

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
                if (this is Player)
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
            if (GameLoop.World.CurrentMap.IsTileWalkable(newPosition))
            {
                Position = newPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion HelpCode
    }
}
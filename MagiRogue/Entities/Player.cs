using MagiRogue.System;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(Color foreground, Color background, Point position, int layer = (int)MapLayer.PLAYER) :
            base(foreground, background, '@', layer, position)
        {
            // sets the most fundamental stats, needs to set the godly flag up top, because it superseeds GodPower if it is
            // below.

            Stats.SetAttributes(
                this,
                name: "Magus",
                viewRadius: 5,
                health: 10,
                maxHealth: 10,
                baseHpRegen: 0.1f,
                bodyStat: 1,
                mindStat: 1,
                soulStat: 1,
                attack: 10,
                attackChance: 40,
                defense: 5,
                defenseChance: 20,
                godly: true,
                godPower: 1,
                size: 165,
                weight: 55,
                speed: 100);
        }
    }
}
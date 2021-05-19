using MagiRogue.System;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(Color foreground, Color background, Point position,
             int layer = (int)MapLayer.PLAYER) :
            base("Magus", foreground, background, '@', position, layer)
        {
            // sets the most fundamental stats, needs to set the godly flag up top, because it superseeds GodPower if it is
            // below.
            Stats.SetAttributes(
                viewRadius: 5,
                health: 10,
                baseHpRegen: 0.1f,
                bodyStat: 1,
                mindStat: 1,
                soulStat: 1,
                attack: 10,
                attackChance: 40,
                defense: 5,
                defenseChance: 20,
                speed: 1.0f,
                _baseManaRegen: 0.1f,
                personalMana: 12
                );

            Anatomy.Limbs = Data.LimbTemplate.BasicHumanoidBody(this);
        }
    }
}
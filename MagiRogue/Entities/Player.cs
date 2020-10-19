using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(Color foreground, Color background, int layer = (int)MapLayer.PLAYER) :
            base(foreground, background, '@', layer)
        {
            // sets the most fundamental stats
            BodyStat = 1;
            MindStat = 1;
            SoulStat = 1;

            Attack = 10;
            AttackChance = 40;
            Defense = 5;
            DefenseChance = 20;
            Name = "Adventurer";
        }
    }
}
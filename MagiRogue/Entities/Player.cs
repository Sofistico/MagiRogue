﻿using MagiRogue.System;
using Microsoft.Xna.Framework;

namespace MagiRogue.Entities
{
    // Creates a new player
    // Default glyph is @
    public class Player : Actor
    {
        public Player(Color foreground, Color background, int layer = (int)MapLayer.ACTORS) :
            base(foreground, background, '@', layer)
        {
            // sets the most fundamental stats, needs to set the godly flag up top, because it superseeds GodPower if it is
            // below.
            Godly = true;
            BodyStat = 1;
            MindStat = 1;
            SoulStat = 1;
            GodPower = 1;
            Health = 10;
            MaxHealth = 10;

            Attack = 10;
            AttackChance = 40;
            Defense = 5;
            DefenseChance = 20;
            Name = "Magi";
            ViewRadius = 8;

            Weight = 50;
            Size = 165;
        }
    }
}
﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    //A generic monster capable of
    //combat and interaction
    //yields treasure upon death
    public class Monster : Actor
    {
        private readonly Random rndNum = new Random();

        public Monster(Color foreground, Color background) : base(foreground, background, 'M')
        {
            //number of loot to spawn for monster
            int lootNum = rndNum.Next(1, 4);

            for (int i = 0; i < lootNum; i++)
            {
                // monsters are made out of spork, obvs.
                Item newItem = new Item(Color.Red, Color.Black, "Debug Remains", '%', 1.5);
                newItem.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
                Inventory.Add(newItem);
            }
        }
    }
}
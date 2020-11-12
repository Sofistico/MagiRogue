using MagiRogue.System;
using Microsoft.Xna.Framework;
using System;

namespace MagiRogue.Entities
{
    //A generic monster capable of
    //combat and interaction
    //yields treasure upon death
    // todo: tranform this class in a json class, so that i can define monsters via json.
    public class Monster : Actor
    {
        private readonly Random rndNum = new Random();

        public Monster(Color foreground, Color background, int layer = (int)MapLayer.MONSTER) :
            base(foreground, background, 'M', layer)
        {
            //number of loot to spawn for monster
            int lootNum = rndNum.Next(1, 4);

            for (int i = 0; i < lootNum; i++)
            {
                // monsters are made out of spork, obvs.
                Item newItem = new Item(Color.Red, Color.Black, "Debug Remains", '%', 1.5);
                //newItem.Components.Add(new SadConsole.Components.EntityViewSyncComponent());
                Inventory.Add(newItem);
            }
        }

        // implement a way for the monster to follow, the error was caused because the World wasn't instantieted properly
        private void BasicAi()
        {
            GameLoop.CommandManager.FollowPlayer(this);
        }
    }
}
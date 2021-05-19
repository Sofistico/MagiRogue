using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SadConsole;
using Game = SadConsole.Game;
using MagiRogue.Entities.Data;
using MagiRogue.Entities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MagiRogue.Test.Entities
{
    public class EntitySerializationTest : IDisposable
    {
        public void Dispose()
        {
            Game.Instance.Exit();
        }

        public EntitySerializationTest()
        {
            Game.Create(1, 1);

            Game.Instance.RunOneFrame();
        }

        [Fact]
        public void ItemSerializingTest()
        {
            const string name = "Serialization Test";

            Item item = new Item(Color.Red, Color.Transparent, name, 'T', GoRogue.Coord.NONE, 100);

            string serialized = JsonConvert.SerializeObject(item);
            Item deserialized = JsonConvert.DeserializeObject<Item>(serialized);

            Assert.Equal(name, deserialized.Name);
        }

        [Fact]
        public void ActorSerializingTest()
        {
            Game.Create(1, 1);

            Game.Instance.RunOneFrame();

            const string name = "Actor Serialization Test";

            Actor actor = new Actor(name, Color.White, Color.Black, '@', GoRogue.Coord.NONE);

            string serialized = JsonConvert.SerializeObject(actor);
            Actor deserialized = JsonConvert.DeserializeObject<Actor>(serialized);

            Assert.Equal(name, deserialized.Name);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using MagiRogue.Entities.Data;
using MagiRogue.Entities;
using SadRogue.Primitives;
using Newtonsoft.Json;
using System.IO;

namespace MagiRogue.Test.Entities
{
    public class EntitySerializationTest
    {
        [Fact]
        public void ItemSerializingTest()
        {
            const string name = "Serialization Test";

            Item item = new Item(Color.Red, Color.Transparent, name, 'T', Point.None, 100);

            string serialized = JsonConvert.SerializeObject(item);
            Item deserialized = JsonConvert.DeserializeObject<Item>(serialized);

            Assert.Equal(name, deserialized.Name);
        }

        [Fact]
        public void ActorSerializingTest()
        {
            const string name = "Actor Serialization Test";

            ActorTemplate actor = new ActorTemplate(new Actor(name, Color.White, Color.Black, '@', Point.None));
            string serialized = JsonConvert.SerializeObject(actor);
            var t = SadConsole.Serializer.Serialize(new Actor("tst", Color.AliceBlue, Color.AliceBlue, '2', Point.None));
            Actor deserialized = JsonConvert.DeserializeObject<Actor>(serialized);

            Assert.Equal(name, deserialized.Name);
        }

        [Fact]
        public void ActorDeserializationFromFile()
        {
            List<ActorTemplate> deserialized = Utils.JsonUtils.JsonDeseralize<List<ActorTemplate>>
                (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Entities", "Actors", "Humanoids.json"));
            Actor found = EntityFactory.ActorCreator(Point.None, deserialized.FirstOrDefault(i => i.Id == "test_troll"));
            Assert.Equal(found.Name, deserialized.FirstOrDefault(i => i.Id == "test_troll").Name);
        }
    }
}
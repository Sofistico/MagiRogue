using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Services.Factory;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using Newtonsoft.Json;
using SadRogue.Primitives;
using Xunit;

namespace MagiRogue.Test.Entities
{
    public class EntitySerializationTest
    {
        [Fact]
        public void ItemSerializingTest()
        {
            const string name = "Serialization Test";
            string expectedName = DataManager.QueryMaterial("wood").ReturnNameFromMaterial(name);

            Item item = new(Color.Red, Color.Transparent, name, 'T', Point.None, 100, materialId: "wood");

            string serialized = JsonConvert.SerializeObject(item);
            Item deserialized = JsonConvert.DeserializeObject<Item>(serialized);

            Assert.Equal(expectedName, deserialized.Name);
        }

        [Fact]
        public void ActorSerializingTest()
        {
            const string name = "Actor Serialization Test";

            ActorTemplate actor = new ActorTemplate(EntityFactory.ActorCreator(Point.None,
                "test_race",
                Sex.None,
                AgeGroup.Adult,
                name));
            string serialized = JsonConvert.SerializeObject(actor);
            Actor deserialized = JsonConvert.DeserializeObject<Actor>(serialized);

            Assert.Equal(name, deserialized.Name);
        }

        [Fact]
        public void ActorSerializeBody()
        {
            var playa = EntityFactory.PlayerCreatorFromZero(new Point(0, 0),
                "human",
                "Test",
                Sex.None,
                "new_wiz");

            var json = JsonConvert.SerializeObject(playa);
            var deserialized = JsonConvert.DeserializeObject<Actor>(json);

            Assert.Equal(playa.Body.Stamina, deserialized.Body.Stamina);
            Assert.Equal(playa.Body.StaminaRegen, deserialized.Body.StaminaRegen);
        }

        [Fact]
        public void ActorSerializeToSeeIfCanSeeFalse()
        {
            var playa = EntityFactory.PlayerCreatorFromZero(new Point(0, 0),
                "human",
                "Test",
                Sex.None,
                "new_wiz");
            var eyes = playa.ActorAnatomy.Organs.FindAll(i => i.OrganType is OrganType.Visual);
            foreach (var item in eyes)
            {
                item.Working = false;
            }
            var json = JsonConvert.SerializeObject(playa);
            var deserialized = JsonConvert.DeserializeObject<Actor>(json);

            Assert.False(deserialized.ActorAnatomy.CanSee);
        }

        [Fact]
        public void ActorSerializeToSeeIfCanSeeTrue()
        {
            var playa = EntityFactory.PlayerCreatorFromZero(new Point(0, 0),
                "human",
                "Test",
                Sex.None,
                "new_wiz");
            var json = JsonConvert.SerializeObject(playa);
            var deserialized = JsonConvert.DeserializeObject<Actor>(json);

            Assert.True(deserialized.ActorAnatomy.CanSee);
        }
    }
}

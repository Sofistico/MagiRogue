using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Factory;
using MagusEngine.Generators;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Systems;
using Newtonsoft.Json;
using SadRogue.Primitives;
using Xunit;

namespace MagiRogue.Test.System
{
    public class MapTests
    {
        private readonly MagiMap map = new("Test", 20, 20);

        public MapTests()
        {
            //Game.Create(20, 20);
        }

        [Fact]
        public void MapTest()
        {
            Assert.True(map.Terrain.Count == 20 * 20);
            Actor actor = new("Test", Color.Black, Color.Black, '3', new Point(1, 1));
            map.AddMagiEntity(actor);
            Assert.True(map.Entities.Contains(actor));
        }

        [Fact]
        public void ChangeEntityCurrentMap()
        {
            var actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(1, 1));
            map.AddMagiEntity(actor);
            var newMap = new MagiMap("MapTest", 1, 1);
            newMap.SetTerrain(new Tile(Color.Beige, Color.Beige, '.', true, true, new Point(0, 0)));
            Universe.ChangeActorMap(actor, newMap, new Point(0, 0), map);
            Assert.True(actor.CurrentMap.Equals(newMap));
        }

        [Fact]
        public void MapSerialization()
        {
            map.SetTerrain(new Tile(Color.Beige, Color.Beige, '.', true, true, new Point(0, 0)));
            Actor actor = new("Test", Color.Black, Color.Black, '@', new Point(0, 0));
            map.AddMagiEntity(actor);
            var json = JsonConvert.SerializeObject((MapTemplate)map);

            Assert.Contains(actor.Name, json);
        }

        [Fact]
        public void WorldSerialization()
        {
            PlanetMap planet = new PlanetGenerator().CreatePlanet(150, 150, 30);
            Player playa = EntityFactory.PlayerCreatorFromZeroForTest(new Point(), "human", "Playa", 25,
                Sex.Female, "new_wiz");
            planet.AssocietatedMap.AddMagiEntity(playa);

            string json = JsonConvert.SerializeObject(planet, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            Assert.True(json.StartsWith('{'));
        }

        [Fact]
        public void MapDeserialization()
        {
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                map.SetTerrain(new Tile(Color.Beige, Color.Beige, '.', true, true, Point.FromIndex(i, map.Width)));
            }
            ActorTemplate actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(0, 0))
            {
                Description = "Test Desc"
            };
            ItemTemplate item = new Item(Color.Black, Color.Black,
                "Test Item", '@', Point.None, 100)
            {
                ItemId = "test",
                EquipType = EquipType.Head
            };
            Player player = Player.TestPlayer();
            player.AddToEquipment(item);

            map.AddMagiEntity(actor);
            map.AddMagiEntity(item);
            map.AddMagiEntity(player);
            var json = JsonConvert.SerializeObject((MapTemplate)map, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            MapTemplate mapDeJsonified = JsonConvert.DeserializeObject<MagiMap>(json);

            Assert.True(mapDeJsonified.MapName == map.MapName);
        }

        [Fact]
        public void TestIdSerializationMap()
        {
            map.SetId(2);
            string json = JsonConvert.SerializeObject(map);
            MagiMap newMap = JsonConvert.DeserializeObject<MagiMap>(json);

            Assert.Equal(map.MapId, newMap.MapId);
        }

        [Fact]
        public void TestSpawnMultipleThingSamePos()
        {
            var fur = DataManager.QueryFurnitureInData("wood_table");
            fur.Position = new Point();
            map.AddMagiEntity(fur);
            var item = DataManager.QueryItemInData("coal_item");
            item.Position = new Point();
            Assert.True(map.CanAddEntity(item));
        }
    }
}

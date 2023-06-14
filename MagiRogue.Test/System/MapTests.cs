using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MagiRogue.Test.System
{
    public class MapTests
    {
        private readonly Map map = new("Test", 20, 20);

        public MapTests()
        {
            //Game.Create(20, 20);
        }

        [Fact]
        public void MapTest()
        {
            Assert.True(map.Tiles.Length == 20 * 20);
            Actor actor = new Actor("Test", Color.Black, Color.Black, '3', new Point(1, 1));
            map.AddMagiEntity(actor);
            Assert.True(map.Entities.Contains(actor));
        }

        [Fact]
        public void ChangeEntityCurrentMap()
        {
            var actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(1, 1));
            map.AddMagiEntity(actor);
            var newMap = new Map("MapTest", 1, 1);
            newMap.Tiles[0] = new TileFloor(new Point(0, 0));
            Universe.ChangeActorMap(actor, newMap, new Point(0, 0), map);
            Assert.True(actor.CurrentMap.Equals(newMap));
        }

        /*[Fact]
        public void CheckIfCanGoBeyondBoundsEntity()
        {
            var actor = new Actor("Name", Color.Black, Color.Black, '@', new Point(21, 21));
            Action testCode = delegate () { map.Add(actor); };
            Assert.Throws<NullReferenceException>(testCode);
        }*/

        [Fact]
        public void MapSerialization()
        {
            map.SetTerrain(new TileFloor(new Point(0, 0)));
            Actor actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(0, 0));
            map.AddMagiEntity(actor);
            var json = JsonConvert.SerializeObject((MapTemplate)map);

            Assert.Contains(actor.Name, json);
        }

        [Fact]
        public void WorldSerialization()
        {
            PlanetMap planet = new PlanetGenerator().CreatePlanet(150, 150, 30);
            Player playa = EntityFactory.PlayerCreatorFromZeroForTest(new Point(), "human", "Playa", 25,
                MagiRogue.Data.Enumerators.Sex.Female, "new_wiz");
            planet.AssocietatedMap.AddMagiEntity(playa);

            PlanetMapTemplate planetMapTemplate = planet;

            string json = JsonConvert.SerializeObject(planetMapTemplate, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            Assert.True(json.StartsWith('{'));
        }

        [Fact]
        public void MapDeserialization()
        {
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                map.SetTerrain(new TileFloor(Point.FromIndex(i, map.Width)));
            }
            ActorTemplate actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(0, 0));
            actor.Description = "Test Desc";
            ItemTemplate item = new Item(Color.Black, Color.Black,
                "Test Item", '@', Point.None, 100)
            {
                ItemId = "test",
                EquipType = MagiRogue.Data.Enumerators.EquipType.Head
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
            MapTemplate mapDeJsonified = JsonConvert.DeserializeObject<Map>(json);

            Assert.True(mapDeJsonified.MapName == map.MapName);
        }

        [Fact]
        public void TestIdSerializationMap()
        {
            map.SetId(2);
            string json = JsonConvert.SerializeObject(map);
            Map newMap = JsonConvert.DeserializeObject<Map>(json);

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
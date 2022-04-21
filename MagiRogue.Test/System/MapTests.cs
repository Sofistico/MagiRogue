﻿using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using System;
using Xunit;

namespace MagiRogue.Test.System
{
    public class MapTests
    {
        private readonly Map map = new("Test", 20, 20);

        public MapTests()
        {
            Game.Create(20, 20);
        }

        [Fact]
        public void MapTest()
        {
            Assert.True(map.Tiles.Length == 20 * 20);
            Actor actor = new Actor("Test", Color.Black, Color.Black, '3', new Point(1, 1));
            map.Add(actor);
            Assert.True(map.Entities.Contains(actor));
        }

        [Fact]
        public void ChangeEntityCurrentMap()
        {
            var actor = new Actor("Test", Color.Black, Color.Black, '@', new Point(1, 1));
            Universe world = new(null);
            map.Add(actor);
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
            map.Add(actor);
            var json = JsonConvert.SerializeObject((MapTemplate)map);

            Assert.Contains(actor.Name, json);
        }

        [Fact]
        public void WorldSerialization()
        {
            PlanetMap planet = new PlanetGenerator().CreatePlanet(150, 150, 30);

            planet.AssocietatedMap.Add(new Player("pla", Color.Black,
                Color.White, new Point(0, 0)));

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
                "Test Item", '@', Point.None, 100);
            Player player = new Player("Test", Color.Black, Color.Black, new Point(0, 0));
            player.Equipment.Add(EntityFactory.BasicHumanoidBody()[0], item);
            map.Add(actor);
            map.Add(item);
            map.Add(player);
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
    }
}
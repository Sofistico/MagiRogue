using Arquimedes;
using Arquimedes.Enumerators;
using MagusEngine;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Generators;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using Xunit;

namespace MagiRogue.Test.System
{
    public class UniverseTests
    {
        private readonly Universe uni;

        public UniverseTests()
        {
            MagiPalette.AddToColorDictionary();
            var chunck = new RegionChunk(new Point(0, 0));
            uni = new(new PlanetMap(50, 50), null, null,
                new TimeSystem(10), true,
                SeasonType.Spring, chunck);
            Locator.InitializeSingletonServices();
            PrepareForChunkTest();
        }

        [Fact]
        public void SerializeUniverse()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni, Formatting.Indented);

            Assert.Contains(uni.Time.TimePassed.Ticks.ToString(), json);
        }

        private static void FillTilesWithRandomShit(MagiMap map)
        {
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                var p = Point.FromIndex(i, map.Width);
                int z = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, 3);
                if (z == 1)
                    map.SetTerrain(new Tile(Color.Black, Color.Black, '#', false, false, p));
                else
                    map.SetTerrain(new Tile(Color.Black, Color.Black, '.', true, true, p));
            }
        }

        [Fact]
        public void DeserializeUniverse()
        {
            uni.ForceChangeCurrentMap(new MagiMap("Test"));
            uni.WorldMap.AssocietatedMap.SetTerrain
                (new Tile(Color.Black, Color.Black, '.', true, true, Point.Zero));
            Player player = Player.TestPlayer();
            player.Position = new SadRogue.Primitives.Point(0, 0);
            uni.WorldMap.AssocietatedMap.AddMagiEntity(player);
            uni.Player = player;
            var json = JsonConvert.SerializeObject(uni, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            // make it so that the object only reference other files
            UniverseTemplate obj = JsonConvert.DeserializeObject<Universe>(json);

            Assert.True(obj.PossibleChangeMap);
        }

        //// CODE DEAD END, MAYBE KEEP FOR A FUTURE PROJECT
        //// SINCE IT SEEMS THAT FOR LARGE AMOUNT OF DATA THIS IS FASTER
        //[Fact]
        //public void ChunkSerializeJWriterTest()
        //{
        //    PrepareForChunkTest();

        //    using MemoryStream ms = new MemoryStream();
        //    using StreamWriter wr = new StreamWriter(ms);
        //    using JsonTextWriter jw = new JsonTextWriter(wr);
        //    jw.WriteStartArray();
        //    var json = JsonConvert.SerializeObject(uni.CurrentChunk);
        //    jw.WriteValue(json);
        //    jw.WriteEndArray();
        //    /*JsonTextReader reader = new JsonTextReader(new StreamReader(ms));
        //    Newtonsoft.Json.Linq.JObject j = Newtonsoft.Json.Linq.JObject.Load(reader);*/
        //    jw.Flush();
        //    wr.Flush();
        //}

        private void PrepareForChunkTest()
        {
            uni.WorldMap = new PlanetGenerator().CreatePlanet(20,
                    20,
                    3);

            //uni.AllChunks[i] = chunk;
            uni.CurrentChunk = uni.GenerateChunck(new Point(0, 0));

            foreach (var map in uni.CurrentChunk.LocalMaps)
            {
                if (map is null) continue;
                FillTilesWithRandomShit(map);
            }
        }

        [Fact]
        public void SerializeChunkDirectTest()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni.CurrentChunk);

            Assert.Contains("X", json);
        }

        [Fact]
        public void DeserializeChunck()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni.CurrentChunk);
            RegionChunk chunks = JsonConvert.DeserializeObject<RegionChunk>(json);

            Assert.True(uni.CurrentChunk.LocalMaps[0].MapId == chunks.LocalMaps[0].MapId);
        }

        /*[Fact]
        public void DeserializeSpecificChunck()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni.CurrentChunk);

            JArray jObj = JArray.Parse(json);

            var chunck = jObj[0].ToObject<RegionChunk>();

            Assert.True(chunck.X == 0 && chunck.Y == 0);
        }*/

        [Fact]
        public void TestIdSerializationUniverse()
        {
            PrepareForChunkTest();
            string json = JsonConvert.SerializeObject(uni);
            Universe newUni = JsonConvert.DeserializeObject<Universe>(json);

            Assert.Equal(uni.CurrentChunk.LocalMaps[0].MapId, newUni.CurrentChunk.LocalMaps[0].MapId);
        }
    }
}

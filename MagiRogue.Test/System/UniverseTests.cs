using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newtonsoft.Json.Linq;

namespace MagiRogue.Test.System
{
    [MemoryDiagnoser]
    public class UniverseTests
    {
        private Universe uni;

        public UniverseTests()
        {
            var chunck = new RegionChunk(new Point(0, 0));
            uni = new(new PlanetMap(50, 50), null, null,
                new MagiRogue.System.Time.TimeSystem(500), true,
                MagiRogue.System.Time.SeasonType.Spring, new(), chunck);
            Palette.AddToColorDictionary();
        }

        [Fact]
        public void SerializeUniverse()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni, Formatting.Indented);

            Assert.Contains(uni.Time.TimePassed.Ticks.ToString(), json);
        }

        private static void FillTilesWithRandomShit(Map map)
        {
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                var p = SadRogue.Primitives.Point.FromIndex(i, map.Width);
                int z = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, 3);
                if (z == 1)
                    map.SetTerrain(new TileFloor(p));
                else
                    map.SetTerrain(new TileWall(p));
            }
        }

        [Fact]
        public void DeserializeUniverse()
        {
            uni.ForceChangeCurrentMap(new Map("Test"));
            uni.WorldMap.AssocietatedMap.SetTerrain
                (new TileFloor(new SadRogue.Primitives.Point(0, 0)));
            Player player = Player.TestPlayer();
            player.Position = new SadRogue.Primitives.Point(0, 0);
            uni.WorldMap.AssocietatedMap.Add(player);
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

        // CODE DEAD END, MAYBE KEEP FOR A FUTURE PROJECT
        // SINCE IT SEEMS THAT FOR LARGE AMOUNT OF DATA THIS IS FASTER
        [Fact]
        public void ChunkSerializeJWriterTest()
        {
            PrepareForChunkTest();

            using MemoryStream ms = new MemoryStream();
            using StreamWriter wr = new StreamWriter(ms);
            using JsonTextWriter jw = new JsonTextWriter(wr);
            jw.WriteStartArray();
            var json = JsonConvert.SerializeObject(uni.CurrentChunk);
            jw.WriteValue(json);
            jw.WriteEndArray();
            /*JsonTextReader reader = new JsonTextReader(new StreamReader(ms));
            Newtonsoft.Json.Linq.JObject j = Newtonsoft.Json.Linq.JObject.Load(reader);*/
            jw.Flush();
            wr.Flush();
        }

        private void PrepareForChunkTest()
        {
            uni.WorldMap = new PlanetGenerator().CreatePlanet(20,
                    20,
                    3);

            RegionChunk chunk = uni.GenerateChunck(new Point(0, 0));
            //uni.AllChunks[i] = chunk;
            uni.CurrentChunk = chunk;

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

        [Fact]
        public void DeserializeSpecificChunck()
        {
            PrepareForChunkTest();

            var json = JsonConvert.SerializeObject(uni.CurrentChunk);

            JArray jObj = JArray.Parse(json);

            var chunck = jObj[0].ToObject<RegionChunk>();

            Assert.True(chunck.X == 0 && chunck.Y == 0);
        }

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
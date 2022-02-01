using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Planet;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MagiRogue.Test.System
{
    public class UniverseTests
    {
        private Universe uni;

        public UniverseTests()
        {
            var chunck = new RegionChunkTemplate[50 * 50];
            uni = new(new PlanetMap(50, 50), null, null,
                new MagiRogue.System.Time.TimeSystem(500), true,
                MagiRogue.System.Time.SeasonType.Spring, chunck, new());
        }

        [Fact]
        public void SerializeUniverse()
        {
            var json = JsonConvert.SerializeObject(uni, Formatting.Indented);

            Assert.Contains(uni.Time.TimePassed.Ticks.ToString(), json);
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
    }
}
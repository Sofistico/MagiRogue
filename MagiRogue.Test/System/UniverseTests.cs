using MagiRogue.Data.Serialization;
using MagiRogue.System;
using MagiRogue.System.Planet;
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
                MagiRogue.System.Time.SeasonType.Spring, chunck);
        }

        [Fact]
        public void SerializeUniverse()
        {
            var json = JsonConvert.SerializeObject(uni);

            Assert.Contains(uni.Time.TimePassed.Ticks.ToString(), json);
        }

        /*[Fact]
        public void DeserializeUniverse()
        {
            uni.ForceChangeCurrentMap(new Map("Test"));
            var json = JsonConvert.SerializeObject(uni, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            // make it so that the object only reference other files
            var obj = JsonConvert.DeserializeObject<UniverseTemplate>(json);

            Assert.True(obj.PossibleChangeMap);
        }*/
    }
}
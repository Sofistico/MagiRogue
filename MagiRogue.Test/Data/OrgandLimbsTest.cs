using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MagiRogue.Data;
using MagiRogue.Utils;
using Newtonsoft.Json;
using MagiRogue.Entities;
using MagiRogue.Data.Serialization;
using Newtonsoft.Json.Linq;

namespace MagiRogue.Test.Data
{
    public class OrgandLimbsTest
    {
        public OrgandLimbsTest()
        {
        }

        [Fact]
        public void DeserializeOrgans()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            List<Organ> organs = JsonUtils.JsonDeseralize<List<Organ>>
                (path + @"\Data\Other\organs.json");

            var otg = DataManager.QueryOrganInData("brain");

            Assert.Equal(organs[0].Id, otg.Id);
        }

        [Fact]
        public void DeserializeLimbs()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            List<LimbTemplate> limb = MagiRogue.Utils.JsonUtils.JsonDeseralize<List<LimbTemplate>>
                (path + @"\Data\Other\body_parts.json");

            var otherLimb = DataManager.QueryLimbInData("humanoid_torso");

            Assert.Equal(limb[0].Id, otherLimb.Id);
        }

        [Fact]
        public void SerializeOrgans()
        {
            Organ organ = new Organ("organ_test", "Test", null,
                Limb.LimbOrientation.Center,
                OrganType.Misc,
                15,
                "null",
                1.5f);

            string json = JsonConvert.SerializeObject(organ);
            JObject jOrgan = JObject.Parse(json);

            Assert.Equal(jOrgan["Id"].ToString(), organ.Id);
        }

        [Fact]
        public void SerializeLimbs()
        {
            Limb limb = new Limb("head_test", TypeOfLimb.Head,
                12,
                12,
                2.5,
                "Test",
                Limb.LimbOrientation.Center,
                "humanoid_torso");

            string json = JsonConvert.SerializeObject(limb);
            JObject obj = JObject.Parse(json);

            Assert.Equal(limb.Id, obj["Id"].ToString());
        }
    }
}
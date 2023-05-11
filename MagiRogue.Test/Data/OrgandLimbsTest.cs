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
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Enumerators;
using SadRogue.Primitives;

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
                (path + @"\Data\Bodies\organs_std.json");

            var otg = DataManager.QueryOrganInData("brain");

            Assert.Equal(organs[0].Id, otg.Id);
        }

        [Fact]
        public void DeserializeLimbs()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            List<Limb> limb = MagiRogue.Utils.JsonUtils.JsonDeseralize<List<Limb>>
                (path + @"\Data\Bodies\limbs_std.json");

            var otherLimb = DataManager.QueryLimbInData("upper_body");

            Assert.Equal(limb[0].Id, otherLimb.Id);
        }

        [Fact]
        public void SerializeOrgans()
        {
            Organ organ = new Organ("organ_test", "Test", null,
                BodyPartOrientation.Center,
                OrganType.Misc,
                "null");

            string json = JsonConvert.SerializeObject(organ);
            JObject jOrgan = JObject.Parse(json);

            Assert.Equal(jOrgan["Id"].ToString(), organ.Id);
        }

        [Fact]
        public void SerializeLimbs()
        {
            Limb limb = new Limb("head_test", LimbType.Head,
                "Test",
                BodyPartOrientation.Center,
                "humanoid_torso");

            string json = JsonConvert.SerializeObject(limb);
            JObject obj = JObject.Parse(json);

            Assert.Equal(limb.Id, obj["Id"].ToString());
        }

        [Fact]
        public void GetConnecetLimb()
        {
            var bp = DataManager.QueryBpPlanInData("humanoid_limbs");
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5fingers").BodyParts);
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5toes").BodyParts);
            List<Limb> basicHuman = bp.ReturnBodyParts().Where(i => i is Limb).Cast<Limb>().ToList();
            Anatomy ana = new Anatomy();
            ana.Limbs = basicHuman;
            List<Limb> test = ana.GetAllConnectedLimb(basicHuman.Find(f => f.LimbType is LimbType.Arm));
            Assert.True(test.Count >= 3);
        }

        [Fact]
        public void FindLimbTillRootParent()
        {
            var bp = DataManager.QueryBpPlanInData("humanoid_limbs");
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5fingers").BodyParts);
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5toes").BodyParts);
            List<Limb> basicHuman = bp.ReturnBodyParts().Where(i => i is Limb).Cast<Limb>().ToList();
            Anatomy ana = new Anatomy();
            ana.Limbs = basicHuman;
            List<Limb> test = ana.GetAllParentConnectionLimb(basicHuman.Find(i => i.LimbType is LimbType.Finger));
            Assert.True(test.Count >= 1);
        }

        [Fact]
        public void RegenActorLostLimb()
        {
            Actor actor = EntityFactory.ActorCreator(Point.None, "test_race", "test dummy", 20, Sex.None);
            //Actor actor = new Actor("Test actor", Color.AliceBlue, Color.AliceBlue, '@',
            //    new Point(0, 0));
            //actor.GetAnatomy().Race = "test_race";
            //var bp = DataManager.QueryBpPlanInData("humanoid_limbs");
            //bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5fingers").BodyParts);
            //bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5toes").BodyParts);
            //actor.GetAnatomy().Limbs = bp.ReturnBodyParts().Where(i => i is Limb).Cast<Limb>().ToList();
            var arms = actor.GetAnatomy().Limbs.FindAll(l => l.LimbType is LimbType.Arm);
            foreach (var arm in arms)
            {
                actor.GetAnatomy().Injury(new Wound(DamageTypes.Sharp, arm.Tissues), arm, actor);
            }
            bool healing = true;
            while (healing)
            {
                actor.ApplyAllRegen();
                if (actor.GetAnatomy().Limbs.All(i => i.Attached))
                    healing = false;
            }

            Assert.True(actor.GetAnatomy().Limbs.All(i => i.Attached));
        }

        [Fact]
        public void TestFingers()
        {
            var bp = DataManager.QueryBpPlanInData("humanoid_limbs");
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5fingers").BodyParts);
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5toes").BodyParts);
            var seeWhatHappens = bp.ReturnBodyParts();
            Assert.DoesNotContain(seeWhatHappens, i => i.BodyPartName.Contains("{0}"));
        }

        [Fact]
        public void CustomSelectorCode()
        {
            var wildMaleDeer = EntityFactory.ActorCreator(Point.Zero, "deer", "Test Deer", 10, Sex.Male);
            Assert.Contains(wildMaleDeer.GetAnatomy().Limbs, i => i.BodyPartName.Contains("Horn"));
        }
    }
}
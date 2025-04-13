using Arquimedes.Enumerators;
using Arquimedes.Utils;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services.Factory;
using MagusEngine.Systems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
                (path + @"/Data/Bodies/organs_std.json");

            var otg = DataManager.QueryOrganInData("brain");

            Assert.Equal(organs[0].Id, otg.Id);
        }

        [Fact]
        public void DeserializeLimbs()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            List<Limb> limb = JsonUtils.JsonDeseralize<List<Limb>>
                (path + @"/Data/Bodies/limbs_std.json");

            var otherLimb = DataManager.QueryLimbInData("upper_body");

            Assert.Equal(limb[0].Id, otherLimb.Id);
        }

        [Fact]
        public void SerializeOrgans()
        {
            Organ organ = new("organ_test", "Test", null,
                BodyPartOrientation.Center,
                OrganType.Auditory);

            string json = JsonConvert.SerializeObject(organ);
            JObject jOrgan = JObject.Parse(json);

            Assert.Equal(jOrgan["Id"].ToString(), organ.Id);
        }

        [Fact]
        public void SerializeLimbs()
        {
            Limb limb = new("head_test", LimbType.Head,
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
            List<Limb> basicHuman = bp.ReturnBodyParts().OfType<Limb>().ToList();
            Anatomy ana = new()
            {
                Limbs = basicHuman
            };
            List<Limb> test = ana.GetAllConnectedLimb(basicHuman.Find(f => f.LimbType is LimbType.Arm));
            Assert.True(test.Count >= 3);
        }

        [Fact]
        public void FindLimbTillRootParent()
        {
            var bp = DataManager.QueryBpPlanInData("humanoid_limbs");
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5fingers").BodyParts);
            bp.BodyParts.AddRange(DataManager.QueryBpPlanInData("5toes").BodyParts);
            List<Limb> basicHuman = bp.ReturnBodyParts().OfType<Limb>().ToList();
            Anatomy ana = new()
            {
                Limbs = basicHuman
            };
            List<Limb> test = ana.GetAllParentConnectionLimb(basicHuman.Find(i => i.LimbType is LimbType.Finger));
            Assert.True(test.Count >= 1);
        }

        [Fact]
        public void RegenActorLostLimb()
        {
            Actor actor = EntityFactory.ActorCreator(Point.None, "test_race", "test dummy", 20, Sex.None);
            var arms = actor.ActorAnatomy.Limbs.FindAll(l => l.LimbType is LimbType.Arm);
            foreach (var arm in arms)
            {
                actor.ActorAnatomy.Injury(new Wound(DataManager.QueryDamageInData("sharp"), arm.Tissues), arm, actor);
            }
            bool healing = true;
            while (healing)
            {
                actor.ApplyAllRegen();
                if (actor.ActorAnatomy.Limbs.All(i => i.Attached))
                    healing = false;
            }

            Assert.True(actor.ActorAnatomy.Limbs.All(i => i.Attached));
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
            Assert.Contains(wildMaleDeer.ActorAnatomy.Limbs, i => i.BodyPartName.Contains("Horn"));
        }

        [Fact]
        public void DeerLosingTeeth()
        {
            var wildMaleDeer = EntityFactory.ActorCreator(Point.Zero, "deer", "Test Deer", 10, Sex.Male);
            Assert.Contains(wildMaleDeer.ActorAnatomy.AllBPs, i => i.BodyPartFunction == BodyPartFunction.Teeth);
        }
    }
}

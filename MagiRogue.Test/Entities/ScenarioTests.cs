using MagiRogue.Data;
using MagiRogue.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MagiRogue.Test.Entities
{
    public class ScenarioTests
    {
        [Fact]
        public void TestScenario()
        {
            var scenario = DataManager.QueryScenarioInData("new_wiz");

            Actor test = EntityFactory.ActorCreator(Point.None,
                scenario.RacesAllowed[0],
                "Test",
                25,
                MagiRogue.Data.Enumerators.Sex.None);
            Player player = EntityFactory.PlayerCreatorFromActor(test, scenario,
                MagiRogue.Data.Enumerators.Sex.Male);

            Assert.Equal(scenario.ShapingSkill, player.Magic.ShapingSkill);
        }
    }
}
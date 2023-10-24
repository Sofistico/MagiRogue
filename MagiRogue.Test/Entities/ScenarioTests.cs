using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Factory;
using MagusEngine.Systems;
using SadRogue.Primitives;
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
                Sex.None);
            Player player = EntityFactory.PlayerCreatorFromActor(test, scenario,
                Sex.Male);

            Assert.Equal(scenario.ShapingSkill, player.Magic.ShapingSkill);
        }
    }
}
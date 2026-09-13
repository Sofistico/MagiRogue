using System;
using Arquimedes;
using Arquimedes.Enumerators;
using MagusEngine;
using MagusEngine.Actions;
using MagusEngine.Components.EntityComponents.Ai;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Services.Factory;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Systems.Time.Nodes;
using SadRogue.Primitives;
using Xunit;

namespace MagiRogue.Test.System
{
    public class TurnSystemTests : IDisposable
    {
        private readonly Universe _universe;
        private readonly Player _player;
        private readonly Actor _npc;
        private readonly TimeSystem _time;
        private readonly MessageBusService _originalBus;

        public TurnSystemTests()
        {
            MagiPalette.AddToColorDictionary();

            _originalBus = Locator.GetService<MessageBusService>();
            Locator.AddService(new MessageBusService());

            var map = new MagiMap("TurnTestMap", 10, 10);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    map.SetTerrain(new Tile(Color.White, Color.Black, '.', true, true, new Point(x, y)));
                }
            }

            _player = EntityFactory.PlayerCreatorFromZeroForTest(
                new Point(5, 5), "human", "TestPlayer", 25, Sex.Male, "new_wiz");

            _npc = EntityFactory.ActorCreator(new Point(3, 3), "test_race", "TestNPC", 25, Sex.None)
                .WithComponent(new NeedDrivenAi());

            _time = new TimeSystem();

            var planetMap = new PlanetMap(10, 10);
            var chunk = new RegionChunk(0, 0);
            chunk.LocalMaps[0] = map;

            _universe = new Universe(planetMap, map, _player, _time, true, SeasonType.Spring, chunk);
            Find.Universe = _universe;

            map.AddMagiEntity(_player);
            map.ControlledEntitiy = _player;
            map.AddMagiEntity(_npc);

            _time.RegisterNode(new EntityTimeNode(_npc.ID, 0));
        }

        public void Dispose()
        {
            Locator.AddService(_originalBus);
        }

        [Fact]
        public void MoveAction_ProcessesTurn_AndAdvancesTick()
        {
            long tickBefore = _universe.Time.Tick;

            var moveAction = new MoveAction(new Point(1, 0));
            bool result = moveAction.Execute(_universe);

            Assert.True(result);
            Assert.True(_universe.Time.Tick > tickBefore);
        }

        [Fact]
        public void MoveAction_AllowsNpcToAct()
        {
            long tickBefore = _universe.Time.Tick;

            var moveAction = new MoveAction(new Point(1, 0));
            moveAction.Execute(_universe);

            Assert.True(_universe.Time.Tick > tickBefore);
            bool npcReRegistered = false;
            foreach (var node in _universe.Time.Nodes)
            {
                if (node.Id == _npc.ID)
                {
                    npcReRegistered = true;
                    break;
                }
            }
            Assert.True(npcReRegistered);
        }

        [Fact]
        public void MoveAction_GameStateAdvancesAfterTurn()
        {
            long tickBefore = _universe.Time.Tick;
            Point playerPosBefore = _player.Position;

            var moveAction = new MoveAction(new Point(0, 1));
            moveAction.Execute(_universe);

            Assert.NotEqual(playerPosBefore, _player.Position);
            Assert.True(_universe.Time.Tick > tickBefore);
        }

        [Fact]
        public void WaitAction_ProcessesTurn_AndAdvancesTick()
        {
            long tickBefore = _universe.Time.Tick;

            var waitAction = new WaitAction(TimeHelper.OneSecond);
            waitAction.Execute(_universe);

            Assert.True(_universe.Time.Tick > tickBefore);
        }

        [Fact]
        public void MultipleMoveActions_AdvanceTurnEachTime()
        {
            long tick0 = _universe.Time.Tick;

            new MoveAction(new Point(1, 0)).Execute(_universe);
            long tick1 = _universe.Time.Tick;

            new MoveAction(new Point(1, 0)).Execute(_universe);
            long tick2 = _universe.Time.Tick;

            Assert.True(tick1 > tick0);
            Assert.True(tick2 > tick1);
        }
    }
}

using Arquimedes.Enumerators;
using Diviner.Enums;
using Diviner.Windows;
using MagusEngine;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Commands;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.ECS.Components.ActorComponents.Ai;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole.Input;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;

namespace Diviner
{
    public static class KeyboardHandle
    {
        private static readonly Player? _getPlayer = Find.Universe?.Player;

        private static Target _targetCursor = null!;

        private static readonly Dictionary<Keys, Direction> _movementDirectionMapping = new()
        {
            { Keys.NumPad7, Direction.UpLeft }, { Keys.NumPad8, Direction.Up }, { Keys.NumPad9, Direction.UpRight },
            { Keys.NumPad4, Direction.Left }, { Keys.NumPad6, Direction.Right },
            { Keys.NumPad1, Direction.DownLeft }, { Keys.NumPad2, Direction.Down }, { Keys.NumPad3, Direction.DownRight },
            { Keys.Up, Direction.Up }, { Keys.Down, Direction.Down }, { Keys.Left, Direction.Left }, { Keys.Right, Direction.Right }
        };

        public static bool HandleMapKeys(Keyboard input, UIManager ui, Universe world)
        {
            return HandleActions(input, world, ui);
        }

        public static bool HandleUiKeys(Keyboard info, UIManager ui)
        {
            if (info.IsKeyPressed(Keys.I))
            {
                ui.InventoryScreen.Show();
                return true;
            }

            if (info.IsKeyPressed(Keys.Escape) && ui.NoPopWindow)
            {
                ui.MainMenu.Show();
                ui.MainMenu.IsFocused = true;
                return true;
            }

            return false;
        }

        private static bool HandleMove(Keyboard info, Universe world, UIManager ui)
        {
            #region WorldMovement

            if (CurrentMapIsPlanetView(world))
            {
                var console = ui.MapWindow.MapConsole;

                if (info.IsKeyDown(Keys.Left))
                {
                    console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((-1, 0));
                }

                if (info.IsKeyDown(Keys.Right))
                {
                    console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((1, 0));
                }

                if (info.IsKeyDown(Keys.Up))
                {
                    console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((0, -1));
                }

                if (info.IsKeyDown(Keys.Down))
                {
                    console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((0, +1));
                }
                // Must return false, because there isn't any movement of the actor
                return false;
            }

            #endregion WorldMovement

            foreach (Keys key in _movementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key) && world.CurrentMap is not null)
                {
                    Direction moveDirection = _movementDirectionMapping[key];
                    Point deltaMove = new(moveDirection.DeltaX, moveDirection.DeltaY);
                    var actor = (Actor)world.CurrentMap.ControlledEntitiy;
                    if (world.CurrentMap.ControlledEntitiy is not Player)
                    {
                        if (world.CurrentMap.CheckForIndexOutOfBounds(world.CurrentMap.ControlledEntitiy.Position + deltaMove))
                            return false;

                        int distance = HandleNonPlayerMoveAndReturnDistance(world, deltaMove);

                        return world.CurrentMap.PlayerExplored[world.CurrentMap.ControlledEntitiy.Position + deltaMove]
                            && distance <= _targetCursor?.MaxDistance
                            && actor.MoveBy(deltaMove);
                    }

                    return actor.MoveBy(deltaMove);
                }
            }

            return false;
        }

        private static int HandleNonPlayerMoveAndReturnDistance(Universe world, Point coorToMove)
        {
            int distance = 0;

            if (world.CurrentMap.ControlledEntitiy == _targetCursor?.Cursor)
            {
                if (_targetCursor.TravelPath is not null)
                    distance = _targetCursor.TravelPath.LengthWithStart;
                if (_targetCursor.TravelPath is not null
                    && _targetCursor.TravelPath.LengthWithStart >= _targetCursor.MaxDistance)
                {
                    distance = world.CurrentMap.AStar.ShortestPath(_targetCursor.OriginCoord, world.CurrentMap.ControlledEntitiy.Position + coorToMove).Length;
                }
            }
            return distance;
        }

        private static bool HandleActions(Keyboard info, Universe world, UIManager ui)
        {
            if (_getPlayer == null || world == null)
                return false;

            // Work around for a > symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemPeriod))
            {
                return ActionManager.EnterDownMovement(_getPlayer.Position);
            }
            // Work around for a < symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemComma))
            {
                return ActionManager.EnterUpMovement(_getPlayer.Position);
            }
            if (HandleMove(info, world, ui))
            {
                if (!_getPlayer.Bumped && world?.CurrentMap?.ControlledEntitiy is Player)
                {
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetWalkTime(_getPlayer,
                        world.CurrentMap.GetTileAt<Tile>(_getPlayer.Position)), true));
                }
                else if (world?.CurrentMap?.ControlledEntitiy is Player)
                {
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetAttackTime(_getPlayer,
                        _getPlayer.GetAttacks().GetRandomItemFromList()),
                        true));
                }

                return true;
            }

            if (info.IsKeyPressed(Keys.NumPad5) && info.IsKeyDown(Keys.LeftControl))
            {
                return ActionManager.RestTillFull(_getPlayer);
            }

            if (info.IsKeyPressed(Keys.NumPad5) || info.IsKeyPressed(Keys.OemPeriod))
            {
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Wait, true));
                return true;
            }

            if (info.IsKeyPressed(Keys.OemComma))
            {
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(10, true));
                return true;
            }

            //if (info.IsKeyPressed(Keys.A))
            //{
            //    bool sucess = ActionManager.DirectAttack(world.Player);
            //    world.ProcessTurn(TimeHelper.GetAttackTime(world.Player), sucess);
            //    return sucess;
            //} // some other thing will be in here!
            if (info.IsKeyPressed(Keys.G))
            {
                Item item = world.CurrentMap.GetEntityAt<Item>(world.Player.Position);
                bool sucess = ActionManager.PickUp(world.Player, item);
                ui.InventoryScreen.ShowItems(world.Player);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                return sucess;
            }
            if (info.IsKeyPressed(Keys.D))
            {
                bool sucess = ActionManager.DropTopItemInv(world.Player);
                ui.InventoryScreen.ShowItems(world.Player);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                return sucess;
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = ActionManager.CloseDoor(world.Player);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                ui.MapWindow.MapConsole.IsDirty = true;
            }
            //if (info.IsKeyPressed(Keys.H) && !info.IsKeyDown(Keys.LeftShift))
            //{
            //    bool sucess = ActionManager.SacrificeLifeEnergyToMana(world.Player);
            //    world.ProcessTurn(TimeHelper.MagicalThings, sucess);
            //}
            if (info.IsKeyPressed(Keys.H) && info.IsKeyDown(Keys.LeftShift))
            {
                bool sucess = ActionManager.NodeDrain(_getPlayer);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.MagicalThings, sucess));
            }
            if (info.IsKeyPressed(Keys.L))
            {
                _targetCursor ??= new Target(_getPlayer.Position);

                if (_targetCursor.State == TargetState.LookMode)
                {
                    _targetCursor.EndTargetting();
                }
                else
                {
                    _targetCursor.StartTargetting();
                }

                return true;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.Z))
            {
                SpellSelectWindow spell = new(_getPlayer.Soul.CurrentMana);

                _targetCursor = new Target(_getPlayer.Position);

                spell.Show(_getPlayer.Magic.KnowSpells,
                    selectedSpell => _targetCursor.OnSelectSpell(selectedSpell,
                    (Actor)world.CurrentMap.ControlledEntitiy),
                    _getPlayer.Soul.CurrentMana);

                return true;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.R))
            {
                var wait = ui.GetWindow<WaitWindow>(WindowTag.Wait);
                if (wait is null)
                {
                    wait = new();
                    ui.AddWindowToList(wait);
                }
                wait.Show(true);
                return true;
            }

            if (info.IsKeyPressed(Keys.Enter) && _targetCursor is not null)
            {
                if (_targetCursor.AnyTargeted() && _targetCursor.State == TargetState.LookMode)
                {
                    _targetCursor.LookTarget();
                    return true;
                }
                if (_targetCursor.EntityInTarget() || _targetCursor.SpellTargetsTile())
                {
                    var (sucess, spellCasted) = _targetCursor.EndSpellTargetting();

                    if (sucess)
                    {
                        _targetCursor = null;
                        Locator.GetService<MessageBusService>()?.SendMessage<ProcessTurnEvent>(new(TimeHelper.GetCastingTime(_getPlayer, spellCasted), sucess));
                    }
                    return sucess;
                }
                else
                {
                    Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new("Invalid target!"));
                    return false;
                }
            }

            if (info.IsKeyPressed(Keys.Escape) && (_targetCursor is not null))
            {
                _targetCursor.EndTargetting();

                _targetCursor = null!;

                return true;
            }

#if DEBUG
            if (HandleDebugActions(info, world, ui))
            {
                return true;
            }
#endif

            return false;
        }

#if DEBUG

        private static bool HandleDebugActions(Keyboard info, Universe world, UIManager ui)
        {
            if (info.IsKeyPressed(Keys.F10))
            {
                ActionManager.ToggleFOV();
                ui.MapWindow.MapConsole.IsDirty = true;
                int c = world.CurrentMap.PlayerExplored.Count;
                for (int i = 0; i < c; i++)
                {
                    world.CurrentMap.PlayerExplored[i] = true;
                }
                return false;
            }

            if (info.IsKeyPressed(Keys.F8))
            {
                world.CurrentMap.ControlledEntitiy.AddComponents(new TestComponent());
                return false;
            }

            if (info.IsKeyPressed(Keys.F6))
            {
                if (world.CurrentMap.Rooms?.Count > 0)
                {
                    foreach (Room room in world.CurrentMap.Rooms)
                    {
                        foreach (Point point in room.RoomPoints)
                        {
                            world.CurrentMap.SetTerrain(new Tile(Color.ForestGreen,
                                Color.FloralWhite,
                                '$',
                                true,
                                true,
                                point,
                                "Test Room Tile",
                                "stone"));
                        }
                    }
                }
                return false;
            }

            if (info.IsKeyPressed(Keys.NumPad0))
            {
                LookWindow w = new LookWindow(_getPlayer);
                w.Show();
                return false;
            }

            if (info.IsKeyPressed(Keys.M))
            {
                ActionManager.ShootProjectile(5000, _getPlayer.Position, DataManager.QueryItemInData("test"), Direction.Left);

                return true;
            }

            if (info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyPressed(Keys.O) && _targetCursor is not null)
            {
                var (_, actor) = ActionManager.CreateTestEntity(_targetCursor.Cursor.Position, world);
                actor.AddComponents(new MoveAndAttackAI(actor.GetViewRadius()));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.O) && _targetCursor is not null)
            {
                var (_, entity) = ActionManager.CreateTestEntity(_targetCursor.Cursor.Position, world);
                entity.AddComponents(new BasicAi(entity));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.P) && _targetCursor.EntityInTarget())
            {
                Actor actor = (Actor)_targetCursor.TargetEntity();
                actor.AddComponents(new MoveAndAttackAI(actor.GetViewRadius()));
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"Added attack component to {actor.Name}!"));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyPressed(Keys.P)
                && _targetCursor.EntityInTarget())
            {
                Actor? actor = (Actor?)_targetCursor.TargetEntity();
                actor?.AddComponents(new NeedDrivenAi());
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"Added need component to {actor.Name}!"));
                return false;
            }

            if (info.IsKeyPressed(Keys.Tab))
            {
                ActionManager.CreateNewMapForTesting();
                return false;
            }

            if (info.IsKeyPressed(Keys.OemPlus))
            {
                try
                {
                    // the map is being saved, but it isn't being properly deserialized
                    MagiMap map = (MagiMap)_getPlayer.CurrentMap;
                    map.LastPlayerPosition = _getPlayer.Position;
                    if (Find.Universe.MapIsWorld(map))
                    {
                        string json = JsonConvert.SerializeObject(Find.Universe.WorldMap);

                        Locator.GetService<SavingService>().SaveJsonToSaveFolder(json);
                    }
                    else
                    {
                        string json = map.SaveMapToJson(_getPlayer);

                        // The universe class also isn't being serialized properly, crashing newtonsoft
                        // TODO: Revise this line of code when the time comes to work on the save system.
                        //var gameState = JsonConvert.SerializeObject(new GameState().Universe);
                        MapTemplate mapDeJsonified = JsonConvert.DeserializeObject<MagiMap>(json);
                    }
                }
                catch
                {
                    throw;
                }
                return false;
            }

            if (info.IsKeyPressed(Keys.OemMinus))
            {
                Locator.GetService<SavingService>().SaveGameToFolder(Find.Universe, "TestFile");
                return false;
            }

            if (info.IsKeyPressed(Keys.P) && (_targetCursor?.EntityInTarget()) == true)
            {
                var target = _targetCursor.TargetEntity();
                var needs = target.GetComponent<NeedCollection>();
                for (int i = 0; i < needs.Count; i++)
                {
                    var need = needs[i];
                    need.TurnCounter = need.MaxTurnCounter.HasValue
                        ? need.MaxTurnCounter.Value - 1
                        : (int)((need.Priority + 1) * 1000);
                }
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"{target.Name} is stuck with strange needs!"));
                return false;
            }

            return false;
        }

#endif

        private static bool CurrentMapIsPlanetView(Universe world) =>
            world.WorldMap != null && world.WorldMap.AssocietatedMap == world.CurrentMap && world.Player == null;
    }
}

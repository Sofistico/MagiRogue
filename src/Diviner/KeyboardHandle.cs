using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using Diviner.Extensions;
using Diviner.Windows;
using MagusEngine;
using MagusEngine.Actions;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents;
using MagusEngine.Components.EntityComponents.Ai;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using SadConsole.Input;
using Color = SadRogue.Primitives.Color;

namespace Diviner
{
    public static class KeyboardHandle
    {
        [NotNull]
        private static readonly Player _getPlayer = Find.Universe?.Player!;

        [AllowNull]
        private static readonly Target? _targetCursor;

        private static readonly Dictionary<KeymapAction, Func<IExecuteAction?>> _actionFactory = new()
        {
            [KeymapAction.MoveNorth] = () => new MoveAction((0, -1), _targetCursor),
            [KeymapAction.MoveSouth] = () => new MoveAction((0, 1), _targetCursor),
            [KeymapAction.MoveLeft] = () => new MoveAction((-1, 0), _targetCursor),
            [KeymapAction.MoveRight] = () => new MoveAction((1, 0), _targetCursor),
            [KeymapAction.MoveNorthLeft] = () => new MoveAction((-1, -1), _targetCursor),
            [KeymapAction.MoveNorthRight] = () => new MoveAction((1, -1), _targetCursor),
            [KeymapAction.MoveSouthLeft] = () => new MoveAction((-1, 1), _targetCursor),
            [KeymapAction.MoveSouthRight] = () => new MoveAction((1, 1), _targetCursor),
            [KeymapAction.MoveUp] = () => new UpDownMovementAction(1),
            [KeymapAction.MoveDown] = () => new UpDownMovementAction(-1),
            [KeymapAction.OpenInventory] = () => new OpenInventoryAction(),
            [KeymapAction.ThownItem] = () => new ThrowItemAction(),
            [KeymapAction.EscapeMenu] = () => new EscapeMenuAction(),
            [KeymapAction.WaitScreen] = () => new OpenWaitAction(),
            [KeymapAction.WaitTillRested] = () => new RestAction(_getPlayer),
            [KeymapAction.WaitOneMoment] = () => new WaitAction(TimeHelper.OneMoment),
            [KeymapAction.WaitOneSecond] = () => new WaitAction(TimeHelper.OneSecond),
            [KeymapAction.PickUp] = () => new PickupPlayerAction(_getPlayer),
        };


        public static bool HandleKeys(Keyboard info)
        {
            var action = info.GetActionFromKey();
            if (action is null)
                return false;
#if DEBUG
            if (HandleDebugActions(info, Find.Universe))
            {
                return true;
            }
#endif
            var executeAction = GetExecuteAction(action.Value);
            if (executeAction is null)
                return false;
            return executeAction.Execute(Find.Universe);
        }

        private static IExecuteAction? GetExecuteAction(KeymapAction action)
        {
            if (_actionFactory.TryGetValue(action, out var factory))
                return factory();
            return null;
        }

#if DEBUG

        private static bool HandleDebugActions(Keyboard info, Universe uni)
        {
            if (info.IsKeyPressed(Keys.F10))
            {
                ActionManager.ToggleFOV();
                int c = uni!.CurrentMap!.PlayerExplored.Count;
                for (int i = 0; i < c; i++)
                {
                    uni.CurrentMap.PlayerExplored[i] = true;
                }
                return false;
            }

            if (info.IsKeyPressed(Keys.F2))
            {
                uni!.CurrentMap!.ForceFovCalculation();
                return false;
            }

            if (info.IsKeyPressed(Keys.K) && _targetCursor?.TileInTarget() == true)
            {
                Tile tile = uni!.CurrentMap!.GetTileAt(_targetCursor.Position)!;
                tile!.IsTransparent = !tile.IsTransparent;
                return false;
            }

            if (info.IsKeyPressed(Keys.F8))
            {
                uni!.CurrentMap!.ControlledEntitiy!.AddComponents(new TestComponent());
                return false;
            }

            if (info.IsKeyPressed(Keys.F6))
            {
                if (uni?.CurrentMap?.Rooms?.Count > 0)
                {
                    foreach (Room room in uni.CurrentMap.Rooms)
                    {
                        foreach (Point point in room.RoomPoints)
                        {
                            uni.CurrentMap.SetTerrain(new Tile(Color.ForestGreen,
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
                var w = new LookWindow(_getPlayer);
                w.Show();
                return false;
            }

            if (info.IsKeyPressed(Keys.M))
            {
                Item item = DataManager.QueryItemInData("ingot", DataManager.QueryMaterial("iron")!, _getPlayer.Position)!;
                //ActionManager.ShootProjectile(5000, _getPlayer.Position, item, Direction.Left, _getPlayer);
                Locator.GetService<MessageBusService>().SendMessage<AddEntitiyCurrentMap>(new(item));
                return true;
            }

            if (info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyPressed(Keys.O) && _targetCursor is not null)
            {
                var (_, actor) = ActionManager.CreateTestEntity(_targetCursor.Cursor.Position, uni);
                actor.AddComponents(new MoveAndAttackAI(actor.GetViewRadius()));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.O) && _targetCursor is not null)
            {
                var (_, entity) = ActionManager.CreateTestEntity(_targetCursor.Cursor.Position, uni);
                entity.AddComponents(new BasicAi(entity));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.P) && _targetCursor!.EntityInTarget())
            {
                Actor actor = (Actor)_targetCursor.TargetEntity()!;
                actor.AddComponents(new MoveAndAttackAI(actor.GetViewRadius()));
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"Added attack component to {actor.Name}!"));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyPressed(Keys.P)
                && _targetCursor!.EntityInTarget())
            {
                Actor? actor = (Actor?)_targetCursor.TargetEntity();
                actor?.AddComponents(new NeedDrivenAi());
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new($"Added need component to {actor?.Name}!"));
                return false;
            }

            if (info.IsKeyPressed(Keys.Tab))
            {
                ActionManager.CreateNewMapForTesting();
                return false;
            }

            if (info.IsKeyPressed(Keys.P) && (_targetCursor?.EntityInTarget()) == true)
            {
                var target = _targetCursor.TargetEntity();
                var needs = target!.GetComponent<NeedCollection>();
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
    }
}

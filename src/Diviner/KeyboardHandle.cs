using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using Arquimedes.Settings;
using Diviner.Windows;
using MagusEngine;
using MagusEngine.Actions;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents;
using MagusEngine.Components.EntityComponents.Ai;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils.Extensions;
using SadConsole.Input;
using Color = SadRogue.Primitives.Color;

namespace Diviner
{
    public static class KeyboardHandle
    {
        [NotNull]
        private static readonly Player _getPlayer = Find.Universe?.Player!;

        [AllowNull]
        private static Target? _targetCursor;

        public static bool HandleKeys(Keyboard info)
        {
            var inputSettings = Locator.GetService<Dictionary<KeymapAction, InputSetting>>();

            return false;
        }

        public static bool HandleActions(Keyboard info, Universe uni, UIManager ui)
        {
            if (_getPlayer == null || uni == null)
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
            if (HandleMove(info, uni, ui))
            {
                if (!_getPlayer.Bumped && uni?.CurrentMap?.ControlledEntitiy is Player)
                {
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetWalkTime(_getPlayer,
                        uni.CurrentMap.GetTileAt<Tile>(_getPlayer.Position)!), true));
                }
                else if (uni?.CurrentMap?.ControlledEntitiy is Player)
                {
                    var attack = _getPlayer.GetAttacks().GetRandomItemFromList() ?? throw new NullValueException("Attack was null", null);
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetAttackTime(_getPlayer, attack), true));
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
                Item item = uni.CurrentMap!.GetEntityAt<Item>(uni.CurrentMap.ControlledEntitiy!.Position)!;
                bool sucess = ActionManager.PickUp(uni.Player, item!);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                return sucess;
            }

            if (info.IsKeyPressed(Keys.D))
            {
                //bool sucess = ActionManager.DropTopItemInv(uni.Player);
                ui.InventoryScreen.ShowItems((Actor)uni!.CurrentMap!.ControlledEntitiy!, item =>
                {
                    var sucess = ActionManager.DropItem(item, _getPlayer.Position, uni.CurrentMap);
                    Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                });
                return false;
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = ActionManager.CloseDoor(uni.Player);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                ui.MapWindow.MapConsole.IsDirty = true;
            }
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.H))
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

                _targetCursor ??= new Target(_getPlayer.Position);
                var magic = _getPlayer.GetComponent<Magic>();
                spell.Show(magic.KnowSpells, selectedSpell => _targetCursor.OnSelectSpell(selectedSpell, (Actor)uni.CurrentMap!.ControlledEntitiy!), _getPlayer.Soul.CurrentMana);

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
                if (_targetCursor.AnyTargeted())
                {
                    bool sucess = false;
                    long timeTaken = 0;
                    if (_targetCursor.State == TargetState.LookMode)
                    {
                        _targetCursor.LookTarget();
                        return true;
                    }
                    else if (_targetCursor.State == TargetState.TargetingSpell)
                    {
                        (sucess, var spellCasted) = _targetCursor.EndSpellTargetting();
                        if (sucess)
                            timeTaken = TimeHelper.GetCastingTime(_getPlayer, spellCasted!);
                    }
                    else
                    {
                        (sucess, var item) = _targetCursor.EndItemTargetting();
                        if (sucess)
                            timeTaken = TimeHelper.GetShootingTime(_getPlayer, item!.Mass);
                    }
                    if (sucess)
                    {
                        _targetCursor = null;
                        Locator.GetService<MessageBusService>()?.SendMessage<ProcessTurnEvent>(new(timeTaken, sucess));
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
            if (HandleDebugActions(info, uni, ui))
            {
                return true;
            }
#endif

            return false;
        }

#if DEBUG

        private static bool HandleDebugActions(Keyboard info, Universe uni, UIManager ui)
        {
            if (info.IsKeyPressed(Keys.F10))
            {
                ActionManager.ToggleFOV();
                ui.MapWindow.MapConsole.IsDirty = true;
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

using MagiRogue.Commands;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using MagiRogue.UI.Windows;
using Newtonsoft.Json;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.UI
{
    public static class KeyboardHandle
    {
        private static Player GetPlayer => GameLoop.Universe.Player;

        private static Target targetCursor;

        private static readonly Dictionary<Keys, Direction> MovementDirectionMapping = new Dictionary<Keys, Direction>
        {
            { Keys.NumPad7, Direction.UpLeft }, { Keys.NumPad8, Direction.Up }, { Keys.NumPad9, Direction.UpRight },
            { Keys.NumPad4, Direction.Left }, { Keys.NumPad6, Direction.Right },
            { Keys.NumPad1, Direction.DownLeft }, { Keys.NumPad2, Direction.Down }, { Keys.NumPad3, Direction.DownRight },
            { Keys.Up, Direction.Up }, { Keys.Down, Direction.Down }, { Keys.Left, Direction.Left }, { Keys.Right, Direction.Right }
        };

        public static bool HandleMapKeys(Keyboard input, UIManager ui, Universe world)
        {
            if (HandleActions(input, world, ui))
                return true;

            return false;
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

        private static bool HandleMove(Keyboard info, Universe world)
        {
            #region WorldMovement

            if (CurrentMapIsPlanetView(world))
            {
                var console = GameLoop.UIManager.MapWindow.MapConsole;

                if (info.IsKeyDown(Keys.Left))
                {
                    console.ViewPosition = console.ViewPosition.Translate((-1, 0));
                }

                if (info.IsKeyDown(Keys.Right))
                {
                    console.ViewPosition = console.ViewPosition.Translate((1, 0));
                }

                if (info.IsKeyDown(Keys.Up))
                {
                    console.ViewPosition = console.ViewPosition.Translate((0, -1));
                }

                if (info.IsKeyDown(Keys.Down))
                {
                    console.ViewPosition = console.ViewPosition.Translate((0, +1));
                }
                // Must return false, because there isn't any movement of the actor
                return false;
            }

            #endregion WorldMovement

            foreach (Keys key in MovementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key) && world.CurrentMap is not null)
                {
                    Direction moveDirection = MovementDirectionMapping[key];
                    Point coorToMove = new(moveDirection.DeltaX, moveDirection.DeltaY);

                    if (world.CurrentMap.ControlledEntitiy is not Player)
                    {
                        int distance = 0;
                        if (targetCursor.TravelPath is not null)
                            distance = targetCursor.TravelPath.LengthWithStart;

                        if (world.CurrentMap.CheckForIndexOutOfBounds
                            (world.CurrentMap.ControlledEntitiy.Position + coorToMove))
                            return false;

                        // If there is a need to roll back,
                        // the code here was taking the CurrentFov and Contains(pos + posMove)
                        if (world.CurrentMap.PlayerExplored
                            [world.CurrentMap.ControlledEntitiy.Position + coorToMove]
                            && distance <= targetCursor.MaxDistance)
                        {
                            return CommandManager.MoveActorBy
                                ((Actor)world.CurrentMap.ControlledEntitiy, coorToMove);
                        }
                        else
                            return false;
                    }

                    bool sucess =
                        CommandManager.MoveActorBy((Actor)world.CurrentMap.ControlledEntitiy, coorToMove);
                    return sucess;
                }
            }

            return false;
        }

        private static bool HandleActions(Keyboard info, Universe world, UIManager ui)
        {
            // Work around for a > symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemPeriod))
            {
                return CommandManager.EnterDownMovement(GetPlayer.Position);
            }
            // Work around for a < symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemComma))
            {
                return CommandManager.EnterUpMovement(GetPlayer.Position);
            }
            if (HandleMove(info, world))
            {
                if (!GetPlayer.Bumped && world.CurrentMap.ControlledEntitiy is Player)
                    world.ProcessTurn(TimeHelper.GetWalkTime(GetPlayer,
                        world.CurrentMap.GetTileAt<TileBase>(GetPlayer.Position)), true);
                else if (world.CurrentMap.ControlledEntitiy is Player)
                    world.ProcessTurn(TimeHelper.GetAttackTime(GetPlayer), true);

                return true;
            }

            if (info.IsKeyPressed(Keys.NumPad5) && info.IsKeyDown(Keys.LeftControl))
            {
                return CommandManager.RestTillFull(GetPlayer);
            }

            if (info.IsKeyPressed(Keys.NumPad5) || info.IsKeyPressed(Keys.OemPeriod))
            {
                world.ProcessTurn(TimeHelper.Wait, true);
                return true;
            }

            if (info.IsKeyPressed(Keys.A))
            {
                bool sucess = CommandManager.DirectAttack(world.Player);
                world.ProcessTurn(TimeHelper.GetAttackTime(world.Player), sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.G))
            {
                Item item = world.CurrentMap.GetEntityAt<Item>(world.Player.Position);
                bool sucess = CommandManager.PickUp(world.Player, item);
                ui.InventoryScreen.ShowItems(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.D))
            {
                bool sucess = CommandManager.DropItems(world.Player);
                ui.InventoryScreen.ShowItems(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = CommandManager.CloseDoor(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                ui.MapWindow.MapConsole.IsDirty = true;
            }
            if (info.IsKeyPressed(Keys.H) && !info.IsKeyDown(Keys.LeftShift))
            {
                bool sucess = CommandManager.SacrificeLifeEnergyToMana(world.Player);
                world.ProcessTurn(TimeHelper.MagicalThings, sucess);
            }
            if (info.IsKeyPressed(Keys.H) && info.IsKeyDown(Keys.LeftShift))
            {
                bool sucess = CommandManager.NodeDrain(GetPlayer);
                world.ProcessTurn(TimeHelper.MagicalThings, sucess);
            }
            if (info.IsKeyPressed(Keys.L))
            {
                if (targetCursor is null)
                    targetCursor = new Target(GetPlayer.Position);

                if (world.CurrentMap.ControlledEntitiy is not Player
                    && !targetCursor.EntityInTarget())
                {
                    targetCursor.EndTargetting();
                    return true;
                }

                targetCursor.StartTargetting();
                targetCursor.Cursor.IgnoresWalls = true;

                return true;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.Z))
            {
                SpellSelectWindow spell =
                    new SpellSelectWindow(GetPlayer.Stats.PersonalMana);

                targetCursor = new Target(GetPlayer.Position);

                spell.Show(GetPlayer.Magic.KnowSpells,
                    selectedSpell => targetCursor.OnSelectSpell(selectedSpell,
                    (Actor)world.CurrentMap.ControlledEntitiy),
                    GetPlayer.Stats.PersonalMana);

                return true;
            }

            if (info.IsKeyPressed(Keys.Enter) && targetCursor is not null
                && targetCursor.State == Target.TargetState.Targeting)
            {
                if (targetCursor.EntityInTarget() && targetCursor.SpellSelected is null)
                {
                    targetCursor.LookTarget();
                    return true;
                }
                if (targetCursor.EntityInTarget() || targetCursor.SpellTargetsTile())
                {
                    var (sucess, spellCasted) = targetCursor.EndSpellTargetting();

                    if (sucess)
                    {
                        targetCursor = null;
                        world.ProcessTurn(TimeHelper.GetCastingTime(GetPlayer, spellCasted), sucess);
                    }
                    return sucess;
                }
                else
                {
                    ui.MessageLog.Add("Invalid target!");
                    return false;
                }
            }

            if (info.IsKeyPressed(Keys.Escape) && (targetCursor is not null))
            {
                targetCursor.EndTargetting();

                targetCursor = null;

                return true;
            }

#if DEBUG
            if (info.IsKeyPressed(Keys.F10))
            {
                CommandManager.ToggleFOV();
                ui.MapWindow.MapConsole.IsDirty = true;
            }

            if (info.IsKeyPressed(Keys.F8))
            {
                GetPlayer.AddComponent(new Components.TestComponent(GetPlayer));
            }

            if (info.IsKeyPressed(Keys.NumPad0))
            {
                LookWindow w = new LookWindow(GetPlayer);
                w.Show();
            }

            if (info.IsKeyPressed(Keys.T))
            {
                foreach (NodeTile node in world.CurrentMap.Tiles.OfType<NodeTile>())
                {
                    if (node.TrueAppearence.Matches(node))
                    {
                        break;
                    }
                    node.RestoreOriginalAppearence();
                }
            }

            if (info.IsKeyPressed(Keys.Tab))
            {
                CommandManager.CreateNewMapForTesting();
            }
            if (info.IsKeyPressed(Keys.OemPlus))
            {
                try
                {
                    // the map is being saved, but it isn't being properly deserialized
                    Map map = (Map)GetPlayer.CurrentMap;
                    map.LastPlayerPosition = GetPlayer.Position;
                    if (GameLoop.Universe.MapIsWorld(map))
                    {
                        string json = JsonConvert.SerializeObject(GameLoop.Universe.WorldMap);

                        GameLoop.Universe.SaveAndLoad.SaveJsonToSaveFolder(json);
                    }
                    else
                    {
                        string json = map.SaveMapToJson(GetPlayer);

                        // The universe class also isn't being serialized properly, crashing newtonsoft
                        // TODO: Revise this line of code when the time comes to work on the save system.
                        //var gameState = JsonConvert.SerializeObject(new GameState().Universe);
                        MapTemplate mapDeJsonified = JsonConvert.DeserializeObject<Map>(json);
                    }
                }
                catch (Newtonsoft.Json.JsonSerializationException e)
                {
                    throw e;
                }
            }

            if (info.IsKeyPressed(Keys.OemMinus))
            {
                GameLoop.Universe.SaveAndLoad.SaveGameToFolder(GameLoop.Universe, "TestFile");
            }

#endif

            return false;
        }

        private static bool CurrentMapIsPlanetView(Universe world)
        {
            if (world.WorldMap != null
                && world.WorldMap.AssocietatedMap == world.CurrentMap && world.Player == null)
                return true;
            else
                return false;
        }
    }
}
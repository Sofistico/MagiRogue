using MagiRogue.Components;
using MagiRogue.Components.Ai;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Tiles;
using Diviner.Enums;
using Diviner.Windows;
using Newtonsoft.Json;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using MagusEngine.Commands;
using MagusEngine.Utils.Extensions;
using MagusEngine.ECS.Components;
using MagusEngine.Core.Entities;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.Core;
using MagusEngine.Systems.Time;

namespace Diviner
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

            foreach (Keys key in MovementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key) && world.CurrentMap is not null)
                {
                    Direction moveDirection = MovementDirectionMapping[key];
                    Point coorToMove = new(moveDirection.DeltaX, moveDirection.DeltaY);

                    if (world.CurrentMap.ControlledEntitiy is not Player)
                    {
                        if (world.CurrentMap.CheckForIndexOutOfBounds
                            (world.CurrentMap.ControlledEntitiy.Position + coorToMove))
                            return false;

                        int distance = HandleTargetActionAndReturnDistance(world, coorToMove);

                        // If there is a need to roll back,
                        // the code here was taking the CurrentFov and Contains(pos + posMove)
                        if (world.CurrentMap.PlayerExplored
                            [world.CurrentMap.ControlledEntitiy.Position + coorToMove]
                            && distance <= targetCursor.MaxDistance)
                        {
                            return ActionManager.MoveActorBy
                                ((Actor)world.CurrentMap.ControlledEntitiy, coorToMove);
                        }
                        else
                            return false;
                    }

                    bool sucess =
                        ActionManager.MoveActorBy((Actor)world.CurrentMap.ControlledEntitiy, coorToMove);
                    return sucess;
                }
            }

            return false;
        }

        private static int HandleTargetActionAndReturnDistance(Universe world, Point coorToMove)
        {
            int distance = 0;

            if (world.CurrentMap.ControlledEntitiy == targetCursor.Cursor)
            {
                if (targetCursor.TravelPath is not null)
                    distance = targetCursor.TravelPath.LengthWithStart;
                if (targetCursor.TravelPath is not null
                    && targetCursor.TravelPath.LengthWithStart >= targetCursor.MaxDistance)
                {
                    distance = world.CurrentMap.AStar.ShortestPath(targetCursor.OriginCoord,
                        world.CurrentMap.ControlledEntitiy.Position + coorToMove)
                        .Length;
                }
            }
            return distance;
        }

        private static bool HandleActions(Keyboard info, Universe world, UIManager ui)
        {
            // Work around for a > symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemPeriod))
            {
                return ActionManager.EnterDownMovement(GetPlayer.Position);
            }
            // Work around for a < symbol, must be top to not make the char wait
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.OemComma))
            {
                return ActionManager.EnterUpMovement(GetPlayer.Position);
            }
            if (HandleMove(info, world))
            {
                if (!GetPlayer.Bumped && world.CurrentMap.ControlledEntitiy is Player)
                {
                    world.ProcessTurn(TimeHelper.GetWalkTime(GetPlayer,
                        world.CurrentMap.GetTileAt<Tile>(GetPlayer.Position)), true);
                }
                else if (world.CurrentMap.ControlledEntitiy is Player)
                {
                    world.ProcessTurn(TimeHelper.GetAttackTime(GetPlayer,
                        GetPlayer.GetAttacks().GetRandomItemFromList()),
                        true);
                }

                return true;
            }

            if (info.IsKeyPressed(Keys.NumPad5) && info.IsKeyDown(Keys.LeftControl))
            {
                return ActionManager.RestTillFull(GetPlayer);
            }

            if (info.IsKeyPressed(Keys.NumPad5) || info.IsKeyPressed(Keys.OemPeriod))
            {
                world.ProcessTurn(TimeHelper.Wait, true);
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
                world.ProcessTurn(TimeHelper.Interact, sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.D))
            {
                bool sucess = ActionManager.DropItems(world.Player);
                ui.InventoryScreen.ShowItems(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = ActionManager.CloseDoor(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                ui.MapWindow.MapConsole.IsDirty = true;
            }
            //if (info.IsKeyPressed(Keys.H) && !info.IsKeyDown(Keys.LeftShift))
            //{
            //    bool sucess = ActionManager.SacrificeLifeEnergyToMana(world.Player);
            //    world.ProcessTurn(TimeHelper.MagicalThings, sucess);
            //}
            if (info.IsKeyPressed(Keys.H) && info.IsKeyDown(Keys.LeftShift))
            {
                bool sucess = ActionManager.NodeDrain(GetPlayer);
                world.ProcessTurn(TimeHelper.MagicalThings, sucess);
            }
            if (info.IsKeyPressed(Keys.L))
            {
                targetCursor ??= new Target(GetPlayer.Position);

                if (world.CurrentMap.ControlledEntitiy is not Player
                    && !targetCursor.EntityInTarget())
                {
                    targetCursor.EndTargetting();
                    return true;
                }

                targetCursor.StartTargetting();

                return true;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.Z))
            {
                SpellSelectWindow spell =
                    new SpellSelectWindow(GetPlayer.Soul.CurrentMana);

                targetCursor = new Target(GetPlayer.Position);

                spell.Show(GetPlayer.Magic.KnowSpells,
                    selectedSpell => targetCursor.OnSelectSpell(selectedSpell,
                    (Actor)world.CurrentMap.ControlledEntitiy),
                    GetPlayer.Soul.CurrentMana);

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

            if (info.IsKeyPressed(Keys.Enter) && targetCursor?.State == TargetState.Targeting)
            {
                if ((targetCursor.EntityInTarget() || targetCursor.TileInTarget()) && targetCursor.SpellSelected is null)
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
                    ui.MessageLog.PrintMessage("Invalid target!");
                    return false;
                }
            }

            if (info.IsKeyPressed(Keys.Escape) && (targetCursor is not null))
            {
                targetCursor.EndTargetting();

                targetCursor = null!;

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
                world.CurrentMap.ControlledEntitiy.AddComponent(new TestComponent());
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
                            world.CurrentMap.SetTerrain(new TileFloor("Test Room Tile", point,
                                "stone", '$', Color.ForestGreen, Color.FloralWhite));
                        }
                    }
                }
                return false;
            }

            if (info.IsKeyPressed(Keys.NumPad0))
            {
                LookWindow w = new LookWindow(GetPlayer);
                w.Show();
                return false;
            }

            if (info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyPressed(Keys.O) && targetCursor is not null)
            {
                var (_, actor) = ActionManager.CreateTestEntity(targetCursor.Cursor.Position, world);
                actor.AddComponent(new MoveAndAttackAI(actor.GetViewRadius()));
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.O) && targetCursor is not null)
            {
                var (_, entity) = ActionManager.CreateTestEntity(targetCursor.Cursor.Position, world);
                entity.AddComponent(new BasicAi(entity));
                return false;
            }
            if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.P) && targetCursor.EntityInTarget())
            {
                Actor actor = (Actor)targetCursor.TargetEntity();
                actor.AddComponent(new MoveAndAttackAI(actor.GetViewRadius()));
                GameLoop.AddMessageLog($"Added attack component to {actor.Name}!");
                return false;
            }

            if (info.IsKeyDown(Keys.LeftShift)
                && info.IsKeyDown(Keys.LeftControl)
                && info.IsKeyPressed(Keys.P)
                && targetCursor.EntityInTarget())
            {
                Actor actor = (Actor)targetCursor.TargetEntity();
                actor.AddComponent(new NeedDrivenAi());
                GameLoop.AddMessageLog($"Added need component to {actor.Name}!");
                return false;
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
                return false;
            }

            if (info.IsKeyPressed(Keys.OemMinus))
            {
                GameLoop.Universe.SaveAndLoad.SaveGameToFolder(GameLoop.Universe, "TestFile");
                return false;
            }

            if (info.IsKeyPressed(Keys.P) && (targetCursor?.EntityInTarget()) == true)
            {
                var target = targetCursor.TargetEntity();
                var needs = target.GetComponent<NeedCollection>();
                for (int i = 0; i < needs.Count; i++)
                {
                    var need = needs[i];
                    need.TurnCounter = need.MaxTurnCounter.HasValue
                        ? need.MaxTurnCounter.Value - 1
                        : (int)((need.Priority + 1) * 1000);
                }
                GameLoop.AddMessageLog($"{target.Name} is stuck with strange needs!");
                return false;
            }

            return false;
        }

#endif

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
using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Linq;
using Console = SadConsole.Console;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MagiRogue.UI.Windows
{
    public class MapWindow : MagiBaseWindow
    {
        public Console MapConsole { get; set; }

        private static Player GetPlayer => GameLoop.World.Player;

        private Target targetCursor;

        public MapWindow(int width, int height, string title) : base(width, height, title)
        {
        }

        // centers the viewport camera on an Actor
        public void CenterOnActor(Actor actor)
        {
            MapConsole.SadComponents.Add
                (new SadConsole.Components.SurfaceComponentFollowTarget() { Target = actor });
        }

        public void CreateMapConsole()
        {
            MapConsole = new Console(Width, Height);
        }

        // Adds the entire list of entities found in the
        // World.CurrentMap's Entities SpatialMap to the
        // MapConsole, so they can be seen onscreen
        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            map.ConfigureRender(MapConsole);
        }

        // Loads a Map into the MapConsole
        public void LoadMap(Map map)
        {
            //make console short enough to show the window title
            //and borders, and position it away from borders
            int mapConsoleWidth = Width - 2;
            int mapConsoleHeight = Height - 2;

            Rectangle rec =
                new BoundedRectangle((0, 0, mapConsoleWidth, mapConsoleHeight), (0, 0, map.Width, map.Height)).Area;

            // First load the map's tiles into the console
            MapConsole = new Console(GameLoop.World.CurrentMap.Width,
                GameLoop.World.CurrentMap.Height, GameLoop.World.CurrentMap.Width,
                GameLoop.World.CurrentMap.Width, map.Tiles)
            {
                View = rec,

                //reposition the MapConsole so it doesnt overlap with the left/top window edges
                Position = new Point(1, 1),

                DefaultBackground = Color.Black
            };

            // Adds the console to the children list of the window
            Children.Add(MapConsole);

            // Now Sync all of the map's entities
            SyncMapEntities(map);

            IsDirty = true;
        }

        public bool HandleMapInteraction(Keyboard info, UIManager ui, World world)
        {
            if (HandleMove(info, world))
            {
                if (!GetPlayer.Bumped && world.CurrentMap.ControlledEntitiy is Player)
                    world.ProcessTurn(TimeHelper.GetWalkTime(GetPlayer), true);
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
                Item item = world.CurrentMap.GetEntityAt<Item>(world.Player.Position);
                ui.InventoryScreen.RemoveItemFromConsole(item);
                ui.InventoryScreen.ShowItems(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                return sucess;
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = CommandManager.CloseDoor(world.Player);
                world.ProcessTurn(TimeHelper.Interact, sucess);
                MapConsole.IsDirty = true;
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
                if (!(targetCursor != null))
                    targetCursor = new Target(GetPlayer.Position);

                if (targetCursor.EntityInTarget())
                {
                    if (targetCursor.TargetList != null)
                    {
                        LookWindow w = new LookWindow(targetCursor.TargetList[0]);
                        w.Show();

                        return true;
                    }
                }

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
                    new SpellSelectWindow(GetPlayer.Magic.KnowSpells, GetPlayer.Stats.PersonalMana);

                targetCursor = new Target(GetPlayer.Position);

                spell.Show(GetPlayer.Magic.KnowSpells,
                    selectedSpell => targetCursor.OnSelectSpell(selectedSpell,
                    (Actor)world.CurrentMap.ControlledEntitiy),
                    GetPlayer.Stats.PersonalMana);

                return true;
            }

            if (info.IsKeyPressed(Keys.Enter) && targetCursor is not null && targetCursor.State == Target.TargetState.Targeting)
            {
                if (targetCursor.EntityInTarget())
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
                    ui.MessageLog.Add("There is no one to target there!");
                }
            }
            if (info.IsKeyPressed(Keys.Escape) && (targetCursor is object))
            {
                targetCursor.EndTargetting();

                targetCursor = null;

                return true;
            }

#if DEBUG
            if (info.IsKeyPressed(Keys.F10))
            {
                CommandManager.ToggleFOV();
                MapConsole.IsDirty = true;
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
                foreach (NodeTile node in world.CurrentMap.Tiles.Where(t => t is NodeTile))
                {
                    if (node.TrueAppearence.Matches(node))
                    {
                        break;
                    }
                    node.RestoreOriginalAppearence();
                }
            }

#endif

            return false;
        }

        private bool HandleMove(SadConsole.Input.Keyboard info, World world)
        {
            foreach (Keys key in UIManager.MovementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key))
                {
                    Direction moveDirection = UIManager.MovementDirectionMapping[key];
                    Point coorToMove = new Point(moveDirection.DeltaX, moveDirection.DeltaY);

                    if (world.CurrentMap.ControlledEntitiy is not Player)
                    {
                        int distance = (int)Distance.Chebyshev.Calculate(targetCursor.OriginCoord,
                            world.CurrentMap.ControlledEntitiy.Position + coorToMove);

                        if (world.CurrentMap.PlayerFOV.CurrentFOV.Contains
                            (world.CurrentMap.ControlledEntitiy.Position + coorToMove)
                            &&  distance <= targetCursor.MaxDistance)
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
    }
}
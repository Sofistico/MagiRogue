﻿using GoRogue;
using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Magic.Effects;
using MagiRogue.System.Time;
using MagiRogue.UI.Windows;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System.Linq;
using System.Collections.Generic;
using Color = SadConsole.UI.AdjustableColor;

namespace MagiRogue.UI
{
    // Creates/Holds/Destroys all consoles used in the game
    // and makes consoles easily addressable from a central place.
    public class UIManager : ScreenObject
    {
        #region Managers

        // Here are the managers
        public MapWindow MapWindow { get; set; }

        public MessageLogWindow MessageLog { get; set; }
        public InventoryWindow InventoryScreen { get; set; }
        public StatusWindow StatusConsole { get; set; }
        public MainMenuWindow MainMenu { get; set; }

        public bool NoPopWindow { get; set; } = true;

        #endregion Managers

        #region Field

        private static Player GetPlayer => GameLoop.World.Player;

        public SadConsole.UI.Colors CustomColors;

        private Target target;

        #endregion Field

        #region ConstructorAndInitCode

        public UIManager()
        {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = GameHost.Instance.Screen;
        }

        // Initiates the game by means of going to the menu first
        public void InitMainMenu()
        {
            SetUpCustomColors();

            MainMenu = new MainMenuWindow(GameLoop.GameWidth, GameLoop.GameHeight)
            {
                IsFocused = true
            };
            Children.Add(MainMenu);
            MainMenu.Show();
            MainMenu.Position = new Point(0, 0);
        }

        public void StartGame(bool testGame = false)
        {
            IsFocused = true;

            GameLoop.World = new World(testGame);

            // Hides the main menu, so that it's possible to interact with the other windows.
            MainMenu.Hide();

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);
#if DEBUG
            MessageLog.Add("Test message log works");
#endif
            // Inventory initialization
            InventoryScreen = new InventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Inventory Window");
            Children.Add(InventoryScreen);
            InventoryScreen.Hide();
            InventoryScreen.Position = new Point(GameLoop.GameWidth / 2, 0);

            StatusConsole = new StatusWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Status Window");
            Children.Add(StatusConsole);
            StatusConsole.Position = new Point(GameLoop.GameWidth / 2, 0);
            StatusConsole.Show();

            // Build the Window
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight, "Game Map");

            // Then load the map into the MapConsole
            MapWindow.LoadMap(GameLoop.World.CurrentMap);

            // Start the game with the camera focused on the player
            MapWindow.CenterOnActor(GameLoop.World.Player);
        }

        #endregion ConstructorAndInitCode

        #region Input

        public static readonly Dictionary<Keys, Direction> MovementDirectionMapping = new Dictionary<Keys, Direction>
        {
            { Keys.NumPad7, Direction.UpLeft }, { Keys.NumPad8, Direction.Up }, { Keys.NumPad9, Direction.UpRight },
            { Keys.NumPad4, Direction.Left }, { Keys.NumPad6, Direction.Right },
            { Keys.NumPad1, Direction.DownLeft }, { Keys.NumPad2, Direction.Down }, { Keys.NumPad3, Direction.DownRight },
            { Keys.Up, Direction.Up }, { Keys.Down, Direction.Down }, { Keys.Left, Direction.Left }, { Keys.Right, Direction.Right }
        };

        /// <summary>
        /// Scans the SadConsole's Global KeyboardState and triggers behaviour
        /// based on the button pressed.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (GameLoop.World != null)
            {
                if (HandleMove(info))
                {
                    if (!GetPlayer.Bumped && GameLoop.World.CurrentMap.ControlledEntitiy is Player)
                        GameLoop.World.ProcessTurn(TimeHelper.GetWalkTime(GetPlayer), true);
                    else if (GameLoop.World.CurrentMap.ControlledEntitiy is Player)
                        GameLoop.World.ProcessTurn(TimeHelper.GetAttackTime(GetPlayer), true);

                    return true;
                }

                if (info.IsKeyPressed(Keys.NumPad5) || info.IsKeyPressed(Keys.OemPeriod))
                    GameLoop.World.ProcessTurn(TimeHelper.Wait, true);

                if (info.IsKeyPressed(Keys.A))
                {
                    bool sucess = CommandManager.DirectAttack(GameLoop.World.Player);
                    GameLoop.World.ProcessTurn(TimeHelper.GetAttackTime(GameLoop.World.Player), sucess);
                }

                if (info.IsKeyPressed(Keys.G))
                {
                    Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.Player.Position);
                    bool sucess = CommandManager.PickUp(GameLoop.World.Player, item);
                    InventoryScreen.ShowItems(GameLoop.World.Player);
                    GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
                }
                if (info.IsKeyPressed(Keys.D))
                {
                    bool sucess = CommandManager.DropItems(GameLoop.World.Player);
                    Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.Player.Position);
                    InventoryScreen.RemoveItemFromConsole(item);
                    InventoryScreen.ShowItems(GameLoop.World.Player);
                    GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
                }
                if (info.IsKeyPressed(Keys.C))
                {
                    bool sucess = CommandManager.CloseDoor(GameLoop.World.Player);
                    GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
                    MapWindow.MapConsole.IsDirty = true;
                }
                if (info.IsKeyPressed(Keys.I))
                {
                    InventoryScreen.Show();
                }

                if (info.IsKeyPressed(Keys.H))
                {
                    bool sucess = CommandManager.SacrificeLifeEnergyToMana(GameLoop.World.Player);
                    GameLoop.World.ProcessTurn(TimeHelper.MagicalThings, sucess);
                }

                if (info.IsKeyPressed(Keys.L))
                {
                    if (!(target != null))
                        target = new Target(GetPlayer.Position);

                    if (target.EntityInTarget())
                    {
                        if (target.TargetList != null)
                        {
                            LookWindow w = new LookWindow(target.TargetList[0]);
                            w.Show();

                            return true;
                        }
                    }

                    if (GameLoop.World.CurrentMap.ControlledEntitiy is not Player
                        && !target.EntityInTarget())
                    {
                        GameLoop.World.ChangeControlledEntity(GetPlayer);
                        GameLoop.World.CurrentMap.Remove(target.Cursor);
                        target = null;
                        return true;
                    }

                    GameLoop.World.CurrentMap.Add(target.Cursor);

                    GameLoop.World.ChangeControlledEntity(target.Cursor);

                    return true;
                }

                if (info.IsKeyDown(Keys.LeftShift) && info.IsKeyPressed(Keys.Z))
                {
                    var spellBase = GetPlayer.Magic.QuerySpell("magic_missile");

                    var entity = GameLoop.World.CurrentMap.GetClosestEntity(GetPlayer.Position, spellBase.SpellRange);

                    if (entity != null)
                    {
                        bool sucess = spellBase.CastSpell(
                        entity.Position,
                        GetPlayer);

                        GameLoop.World.ProcessTurn(TimeHelper.MagicalThings, sucess);
                        return true;
                    }
                    else
                    {
                        GameLoop.UIManager.MessageLog.Add("There is no target for the spell!");
                        return false;
                    }
                }

#if DEBUG
                if (info.IsKeyPressed(Keys.F10))
                {
                    CommandManager.ToggleFOV();
                    MapWindow.MapConsole.IsDirty = true;
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

#endif

                if (info.IsKeyPressed(Keys.Escape) && NoPopWindow)
                {
                    //SadConsole.Game.Instance.Exit();
                    MainMenu.Show();
                    MainMenu.IsFocused = true;
                }
            }

            return base.ProcessKeyboard(info);
        }

        public bool HandleMove(SadConsole.Input.Keyboard info)
        {
            foreach (Keys key in MovementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key))
                {
                    Direction moveDirection = MovementDirectionMapping[key];
                    Point coorToMove = new Point(moveDirection.DeltaX, moveDirection.DeltaY);

                    bool sucess = CommandManager.MoveActorBy((Actor)GameLoop.World.CurrentMap.ControlledEntitiy, coorToMove);
                    return sucess;
                }
            }

            return false;
        }

        #endregion Input

        #region HelperMethods

        // Creates a window that encloses a map console
        // of a specified height and width
        // and displays a centered window title
        // make sure it is added as a child of the UIManager
        // so it is updated and drawn
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new MapWindow(width, height, title);

            // The MapWindow becomes a child console of the UIManager
            Children.Add(MapWindow);

            // Add the map console to it
            MapWindow.CreateMapConsole();

            // Without this, the window will never be visible on screen
            MapWindow.Show();
        }

        // Build a new coloured theme based on SC's default theme
        // and then set it as the program's default theme.
        private void SetUpCustomColors()
        {
            // Create a set of default colours that we will modify
            CustomColors = SadConsole.UI.Themes.Library.Default.Colors.Clone();

            // Pick a couple of background colours that we will apply to all consoles.
            Color backgroundColor = new Color(CustomColors.Black, "Black");

            // Set background colour for controls consoles and their controls
            CustomColors.ControlHostBackground = backgroundColor;
            CustomColors.ControlBackgroundNormal = backgroundColor;

            // Generate background colours for dark and light themes based on
            // the default background colour.
            //CustomColors.ControlH = (backgroundColor * 1.3f).FillAlpha();
            //CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();

            // Set a color for currently selected controls. This should always
            // be different from the background colour.
            CustomColors.ControlBackgroundSelected = new Color(CustomColors.GrayDark, "Grey");

            // Rebuild all objects' themes with the custom colours we picked above.
            CustomColors.RebuildAppearances();

            // Now set all of these colours as default for SC's default theme.
            SadConsole.UI.Themes.Library.Default.Colors = CustomColors;
        }

        #endregion HelperMethods
    }
}
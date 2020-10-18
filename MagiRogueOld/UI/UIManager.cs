﻿using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using MagiRogue.Entities;
using Microsoft.Xna.Framework.Input;
using SadConsole.Themes;
using MagiRogue.System;
using GoRogue;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MagiRogue.UI
{
    // Creates/Holds/Destroys all consoles used in the game
    // and makes consoles easily addressable from a central place.
    public class UIManager : ContainerConsole
    {
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public InventoryWindow InventoryScreen;
        public StatusWindow StatusWindow;

        public UIManager()
        {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = Global.CurrentScreen;
        }

        public void Init()
        {
            CreateConsoles();

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(0, GameLoop.GameHeight / 2);

            // Inventory initialization
            InventoryScreen = new InventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Inventory Window");
            Children.Add(InventoryScreen);
            InventoryScreen.Show();
            InventoryScreen.Position = new Point(GameLoop.GameWidth / 2, 0);

            // Load the map into the MapConsole
            LoadMap(GameLoop.World.CurrentMap);

            // Now that the MapConsole is ready, build the Window
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Game Map");
            UseMouse = true;

            // Start the game with the camera focused on the player
            CenterOnActor(GameLoop.World.Player);
        }

        // Creates all child consoles to be managed
        private void CreateConsoles()
        {
            // Temporarily create a console with *no* tile data that will later be replaced with map data
            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
        }

        // centers the viewport camera on an Actor
        public void CenterOnActor(Actor actor)
        {
            MapConsole.CenterViewPortOnPoint(actor.Position);
        }

        // Custom Update method which allows for a vertical scrollbar
        public override void Update(TimeSpan timeElapsed)
        {
            CheckKeyboard();
            base.Update(timeElapsed);
        }

        // Scans the SadConsole's Global KeyboardState and triggers behaviour
        // based on the button pressed.
        private void CheckKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Keys.F5))
                Settings.ToggleFullScreen();
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad8))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, -1));
                CenterOnActor(GameLoop.World.Player);
            }

            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad2))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(0, 1));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad4))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 0));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad6))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 0));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad7))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, -1));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad9))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, -1));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad1))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(-1, 1));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.NumPad3))
            {
                GameLoop.CommandManager.MoveActorBy(GameLoop.World.Player, new Point(1, 1));
                CenterOnActor(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.Escape))
                SadConsole.Game.Instance.Exit();

            if (Global.KeyboardState.IsKeyPressed(Keys.A))
                GameLoop.CommandManager.DirectAttack(GameLoop.World.Player);

            if (Global.KeyboardState.IsKeyPressed(Keys.G))
            {
                Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.Player.Position);
                GameLoop.CommandManager.PickUp(GameLoop.World.Player, item);
                InventoryScreen.ShowItems(GameLoop.World.Player);
            }
            if (Global.KeyboardState.IsKeyPressed(Keys.D))
            {
                GameLoop.CommandManager.DropItems(GameLoop.World.Player);
                Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.Player.Position);
                InventoryScreen.RemoveItemFromConsole(item);
                InventoryScreen.ShowItems(GameLoop.World.Player);
            }
        }

        // Creates a window that encloses a map console
        // of a specified height and width
        // and displays a centered window title
        // make sure it is added as a child of the UIManager
        // so it is updated and drawn
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new Window(width, height)
            {
                CanDrag = true
            };

            //make console short enough to show the window title
            //and borders, and position it away from borders
            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;

            // Resize the Map Console's ViewPort to fit inside of the window's borders snugly
            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);

            //reposition the MapConsole so it doesnt overlap with the left/top window edges
            MapConsole.Position = new Point(1, 1);

            MapConsole.DefaultBackground = Color.Black;

            //close window button
            /*Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(0, 0);
            closeButton.Text = "[X]";

            //Add the close button to the Window's list of UI elements
            MapWindow.Add(closeButton);*/

            // center the title of the console at the top of the window
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth);

            //add the map viewer to the window
            MapWindow.Children.Add(MapConsole);

            // The MapWindow becomes a child console of the UIManager
            Children.Add(MapWindow);

            // Add the player to the MapConsole's render list
            MapConsole.Children.Add(GameLoop.World.Player);

            // Without this, the window will never be visible on screen
            MapWindow.Show();
        }

        // Method helper to print the X and Y of the console.
        // Use only for debugging purposes
#pragma warning disable IDE0051 // Remover membros privados não utilizados

        private static void PrintHeader()
        {
            int counter = 0;
            var startingColor = Color.Black.GetRandomColor(SadConsole.Global.Random);
            var color = startingColor;
            var UIManagerConsoles = Global.CurrentScreen;
            for (int x = 0; x < UIManagerConsoles.Width; x++)
            {
                UIManagerConsoles[x].Glyph = counter.ToString()[0];
                UIManagerConsoles[x].Foreground = color;

                counter++;

                if (counter == 10)
                {
                    counter = 0;
                    color = color.GetRandomColor(SadConsole.Global.Random);
                }
            }

            counter = 0;
            color = startingColor;
            for (int y = 0; y < UIManagerConsoles.Height; y++)
            {
                UIManagerConsoles[0, y].Glyph = counter.ToString()[0];
                UIManagerConsoles[0, y].Foreground = color;

                counter++;

                if (counter == 10)
                {
                    counter = 0;
                    color = color.GetRandomColor(SadConsole.Global.Random);
                }
            }

            // Display console size
            UIManagerConsoles.Print(4, 2, "Console Size");
            UIManagerConsoles.Print(4, 3, "                         ");
            UIManagerConsoles.Print(4, 3, $"{UIManagerConsoles.Width} {UIManagerConsoles.Height}");
        }

#pragma warning disable IDE0051 // Remover membros privados não utilizados

        // Adds the entire list of entities found in the
        // World.CurrentMap's Entities SpatialMap to the
        // MapConsole, so they can be seen onscreen
        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            // Now pull all of the entities into the MapConsole in bulk
            foreach (Entity entity in map.Entities.Items)
            {
                MapConsole.Children.Add(entity);
            }

            // Subscribe to the Entities ItemAdded listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemAdded += OnMapEntityAdded;

            // Subscribe to the Entities ItemRemoved listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        // Remove an Entity from the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityRemoved(object sender, ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Remove(args.Item);
        }

        // Add an Entity to the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityAdded(object sender, ItemEventArgs<Entity> args)
        {
            MapConsole.Children.Add(args.Item);
        }

        // Loads a Map into the MapConsole
        private void LoadMap(Map map)
        {
            // First load the map's tiles into the console
            MapConsole = new ScrollingConsole(GameLoop.World.CurrentMap.Width, GameLoop.World.CurrentMap.Height, Global.FontDefault,
                new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);

            // Now Sync all of the map's entities
            SyncMapEntities(map);
        }
    }
}
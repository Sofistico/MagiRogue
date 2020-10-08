using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiRogue.UI
{
    public class InventoryWindow : Window
    {
        // alocate the inventory memory
        private readonly List<Item> inventory;

        // Create the field
        private readonly ScrollingConsole inventoryConsole;

        // account for the thickness of the window border to prevent UI element spillover
        // check to see if it will be needed.
        private readonly int _windowBorderThickness = 2;

        // This constructor creates a new inventory window and defines the inventory console inside this window
        public InventoryWindow(int width, int height, string title) : base(width, height)
        {
            // Ensure that the window background is the correct colour
            Theme.FillStyle.Background = DefaultBackground;

            // instantiete the inventory of the actor, passing the actor value if and when i implement helpers, to make it
            // possible to see and use their inventory.
            inventory = new List<Item>();

            CanDrag = true;

            Title = title.Align(HorizontalAlignment.Center, width);

            // define the inventory console
            inventoryConsole = new ScrollingConsole(width, height)
            {
                Position = new Point(1, 1)
            };
            inventoryConsole.ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorderThickness);
            inventoryConsole.DefaultBackground = Color.Black;

            UseMouse = true;
            UseKeyboard = true;

            Children.Add(inventoryConsole);
        }

        public void ShowItems(Actor actorInventory)
        {
            int indexInventoryY = 0;
            //inventory.Clear();
            foreach (Item item in actorInventory.Inventory)
            {
                inventory.Add(item);
                inventoryConsole.Print(0, indexInventoryY, item.Name);
                indexInventoryY++;
            }
        }

        public void RemoveItemFromConsole(Item item)
        {
            if (item != null)
            {
                int indexInventoryX = item.Name.Length;
                inventory.Remove(item);
                inventoryConsole.Clear(new Rectangle(0, 0, Width, Height));
            }
        }
    }
}
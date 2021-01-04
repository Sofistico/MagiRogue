using MagiRogue.Entities;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;

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
        private readonly int windowBorderThickness = 2;

        private ScrollBar invScrollBar;

        /// <summary>
        /// This constructor creates a new inventory window and defines the inventory console inside this window
        /// </summary>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="title"></param>
        public InventoryWindow(int width, int heigth, string title) : base(width, heigth)
        {
            // Ensure that the window background is the correct colour
            ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

            // instantiete the inventory of the actor, passing the actor value if and when i implement helpers, to make it
            // possible to see and use their inventory.
            inventory = new List<Item>();

            CanDrag = false;

            Title = title.Align(HorizontalAlignment.Center, width);

            // define the inventory console
            inventoryConsole = new ScrollingConsole(width - windowBorderThickness, heigth - windowBorderThickness)
            {
                Position = new Point(1, 1)
            };
            inventoryConsole.ViewPort = new Rectangle(0, 0, width - 1, heigth - windowBorderThickness);
            inventoryConsole.DefaultBackground = Color.Black;

            UseMouse = true;
            UseKeyboard = true;

            //close window button
            Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(0, 0);
            closeButton.Text = "[X]";

            closeButton.Click += CloseButton_Click;

            invScrollBar = new ScrollBar
                (Orientation.Vertical, heigth - windowBorderThickness)
            {
                Position = new Point(inventoryConsole.Width + 1, inventoryConsole.Position.X)
                //IsEnabled = false
            };
            invScrollBar.ValueChanged += InvScrollBar_ValueChanged; ; ;
            Add(invScrollBar);

            //Add the close button to the Window's list of UI elements
            Add(closeButton);

            Children.Add(inventoryConsole);
        }

        private void InvScrollBar_ValueChanged(object sender, EventArgs e)
        {
            inventoryConsole.ViewPort = new Rectangle(0, invScrollBar.Value + windowBorderThickness,
                    inventoryConsole.Width, inventoryConsole.ViewPort.Height);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Hide();
            //UseKeyboard = false;
            //UseMouse = false;
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
                inventory.Remove(item);
                inventoryConsole.Clear(new Rectangle(0, 0, Width, Height));
            }
        }
    }
}
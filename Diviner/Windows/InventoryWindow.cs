﻿using Diviner.Controls;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Console = SadConsole.Console;
using MagusEngine.Core.Entities;

namespace Diviner.Windows
{
    public class InventoryWindow : PopWindow
    {
        // Create the field
        private readonly Console inventoryConsole;

        private readonly ScrollBar invScrollBar;

        private Dictionary<char, Item> _hotKeys;

        /// <summary>
        /// This constructor creates a new inventory window and defines the inventory console inside this window
        /// </summary>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="title"></param>
        public InventoryWindow(int width, int heigth, string title = "Inventory") : base(title)
        {
            _hotKeys = new Dictionary<char, Item>();
            // define the inventory console
            inventoryConsole = new Console(width - WindowBorderThickness, heigth - WindowBorderThickness)
            {
                Position = new Point(1, WindowBorderThickness)
            };
            inventoryConsole.Surface.View = new Rectangle(0, 0, width - 1, heigth - WindowBorderThickness);

            invScrollBar = new ScrollBar(Orientation.Vertical, heigth - WindowBorderThickness)
            {
                Position = new Point(Width - WindowBorderThickness, inventoryConsole.Position.X)
            };

            invScrollBar.ValueChanged += InvScrollBar_ValueChanged;
            Controls.Add(invScrollBar);

            //Children.Add(inventoryConsole);
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (_hotKeys.TryGetValue(key.Character, out Item item))
                {
                    // Do Something here on base of what the object is
                    // something like item.UseObject()

                    return true;
                }
            }

            return base.ProcessKeyboard(info);
        }

        private void InvScrollBar_ValueChanged(object sender, EventArgs e)
        {
            inventoryConsole.Surface.Surface.View = new Rectangle(0, invScrollBar.Value + WindowBorderThickness,
                    inventoryConsole.Width, inventoryConsole.ViewHeight);
        }

        public void ShowItems(Actor actorInventory)
        {
            SetupSelectionButtons(BuildInventoryButtons(actorInventory.Inventory));
        }

        private void OnItemSelected(Item item)
        {
            DescriptionArea.Clear();
            DescriptionArea.Cursor.Position = new Point(0, 1);
            DescriptionArea.Cursor.Print(item.ToString());
            DescriptionArea.Cursor.Position = new Point(0, 5);
            if (item.Description is object)
                DescriptionArea.Cursor.Print(item.Description);
        }

        private List<MagiButton> BuildInventoryButtons(List<Item> listItems)
        {
            _hotKeys.Clear();

            int yCount = 1;

            var orderedItems = listItems
                .OrderBy(i => i.Name).ToArray();
            var controlList = new List<MagiButton>(orderedItems.Length);
            for (int i = 0; i < orderedItems.Length; i++)
            {
                var hotkeyLetter = (char)(96 + yCount);
                Item item = orderedItems[i];
                _hotKeys.Add(hotkeyLetter, item);

                controlList.Add(new MagiButton(ButtonWidth - 2)
                {
                    Text = $"{hotkeyLetter}. {item.Name}",
                    Position = new Point(1, yCount++),
                    Action = () => OnItemSelected(item)
                });
            }

            for (int i = 1; i < controlList.Count; i++)
            {
                controlList[i - 1].NextSelection = controlList[i];
                controlList[i].PreviousSelection = controlList[i - 1];
            }

            return controlList;
        }
    }
}
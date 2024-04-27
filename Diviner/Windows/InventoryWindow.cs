using MagusEngine.Core.Entities;
using MagusEngine.Utils.Extensions;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class InventoryWindow : PopWindow
    {
        // Create the field
        private readonly Console inventoryConsole;

        private readonly ScrollBar invScrollBar;

        /// <summary>
        /// This constructor creates a new inventory window and defines the inventory console inside this window
        /// </summary>
        public InventoryWindow(int width, int heigth, string title = "Inventory") : base(title)
        {
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
        }

        public override bool ProcessKeyboard(Keyboard info)
        {
            foreach (var key in info.KeysPressed)
            {
                if (_hotKeys.TryGetValue(key.Character, out var item))
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
            inventoryConsole.Surface.Surface.View = new Rectangle(0, invScrollBar.Value + WindowBorderThickness, inventoryConsole.Width, inventoryConsole.ViewHeight);
        }

        public void ShowItems(Actor actorInventory)
        {
            SetupSelectionButtons(BuildHotKeysButtons<Item>(actorInventory.Inventory, OnItemSelected));
        }

        private void OnItemSelected(Item item)
        {
            _descriptionArea.Clear();
            _descriptionArea.Cursor.Position = new Point(0, 1);
            _descriptionArea.Cursor.Print(item.ToString());
            _descriptionArea.Cursor.Position = new Point(0, 5);
            if (!item.Description.IsNullOrEmpty())
                _descriptionArea.Cursor.Print(item.Description);
        }
    }
}

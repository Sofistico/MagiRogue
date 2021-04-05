using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;

namespace MagiRogue.UI
{
    // Will contain relevant custom code here, maybe
    public class MagiBaseWindow : Window
    {
        public MagiBaseWindow(int width, int height, string title) : base(width, height)
        {
            // Ensure that the window background is the correct colour
            ThemeColors = GameLoop.UIManager.CustomColors;
            ThemeColors.ControlBack = Color.Black;
            ThemeColors.TitleText = Color.Red;
            ThemeColors.ModalBackground = Color.Black;
            ThemeColors.ControlHostBack = Color.Black;
            ThemeColors.ControlBackSelected = Color.DarkRed;
            ThemeColors.RebuildAppearances();

            // instantiete the inventory of the actor, passing the actor value if and when i implement helpers, to make it
            // possible to see and use their inventory.

            CanDrag = false;

            Title = title.Align(HorizontalAlignment.Center, width);
        }
    }
}
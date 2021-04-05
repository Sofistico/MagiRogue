using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using Microsoft.Xna.Framework;
using SadConsole;
using MagiRogue.System;
using GoRogue;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MagiRogue.UI
{
    public class MapWindow : MagiBaseWindow
    {
        public ScrollingConsole MapConsole { get; set; }

        public MapWindow(int width, int height, string title) : base(width, height, title)
        {
        }

        // centers the viewport camera on an Actor
        public void CenterOnActor(Actor actor)
        {
            MapConsole.CenterViewPortOnPoint(actor.Position);
        }

        public void CreateMapConsole()
        {
            MapConsole = new ScrollingConsole(Width, Height);
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

            // First load the map's tiles into the console
            MapConsole = new ScrollingConsole(GameLoop.World.CurrentMap.Width,
                GameLoop.World.CurrentMap.Height, Global.FontDefault,
                new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles)
            {
                ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight),

                //reposition the MapConsole so it doesnt overlap with the left/top window edges
                Position = new Point(1, 1),

                DefaultBackground = Color.Black
            };

            // Adds the console to the children list of the window
            Children.Add(MapConsole);

            // Now Sync all of the map's entities
            SyncMapEntities(map);
        }
    }
}
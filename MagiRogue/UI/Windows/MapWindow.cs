﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using SadConsole;
using MagiRogue.System;
using SadRogue.Primitives;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;
using Console = SadConsole.Console;
using MagiRogue.System.Tiles;

namespace MagiRogue.UI.Windows
{
    public class MapWindow : MagiBaseWindow
    {
        public Console MapConsole { get; set; }

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
    }
}
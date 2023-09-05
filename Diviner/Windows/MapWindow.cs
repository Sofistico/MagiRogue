using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace Diviner.Windows
{
    public class MapWindow : MagiBaseWindow
    {
        private Map _mapDisplayed;
        private readonly SadConsole.Components.SurfaceComponentFollowTarget followComponent;
        public ScreenSurface MapConsole { get; set; }

        public MapWindow(int width, int height, string title) : base(width, height, title)
        {
            followComponent = new SadConsole.Components.SurfaceComponentFollowTarget();
            UseMouse = false;
        }


        /// <summary>
        /// centers the viewport camera on an Actor
        /// </summary>
        /// <param name="actor"></param>
        public void CenterOnActor(Actor actor)
        {
            followComponent.Target = actor;
        }

        /// <summary>
        /// centers the viewport camera on an Entity
        /// </summary>
        /// <param name="entity"></param>
        public void CenterOnActor(MagiEntity entity)
        {
            followComponent.Target = entity;
        }

        public void CreateMapConsole()
        {
            MapConsole = new Console(Width, Height);
        }

        // Adds the entire list of entities found in the World.CurrentMap's Entities SpatialMap to
        // the MapConsole, so they can be seen onscreen
        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            map.ConfigureRender(MapConsole);
        }

        /// <summary>
        /// Loads a Map into the MapConsole
        /// </summary>
        /// <param name="map"></param>
        public void LoadMap(Map map)
        {
            //make console short enough to show the window title
            //and borders, and position it away from borders
            int mapConsoleWidth = Width - 2;
            int mapConsoleHeight = Height - 2;
            Children.Remove(MapConsole);
            MapConsole.Dispose();

            Rectangle rec =
                new BoundedRectangle((0, 0, mapConsoleWidth, mapConsoleHeight), (0, 0, map.Width, map.Height)).Area;

            // First load the map's tiles into the console
            MapConsole = new Console(map.Width,
                map.Height, map.Width,
                map.Height, map.GetTilesAppearence())
            {
                //reposition the MapConsole so it doesnt overlap with the left/top window edges
                Position = new Point(1, 1),
            };
            MapConsole.Surface.View = rec;
            MapConsole.Surface.DefaultBackground = Color.Black;

            // Adds the console to the children list of the window
            Children.Add(MapConsole);

            // Now Sync all of the map's entities
            SyncMapEntities(map);

            IsDirty = true;

            _mapDisplayed = map;

            Title = map.MapName;

            MapConsole.SadComponents.Add(followComponent);
        }
    }
}
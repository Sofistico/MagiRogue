﻿using Arquimedes.Enumerators;
using Diviner.Interfaces;
using GoRogue.Messaging;
using MagusEngine;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Services;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace Diviner.Windows
{
    public class MapWindow : MagiBaseWindow,
        ISubscriber<ChangeCenteredActor>,
        ISubscriber<LoadMapMessage>,
        ISubscriber<MapConsoleIsDirty>
    {
        private readonly SadConsole.Components.SurfaceComponentFollowTarget followComponent;
        public ScreenSurface MapConsole { get; set; } = null!;

        public MapWindow(int width, int height, string title) : base(width, height, title)
        {
            followComponent = new SadConsole.Components.SurfaceComponentFollowTarget();
            UseMouse = false;
            Locator.GetService<MessageBusService>().RegisterAllSubscriber(this);
            Tag = WindowTag.Map;
        }

        /// <summary>
        /// centers the viewport camera on an Actor
        /// </summary>
        /// <param name="actor"></param>
        public void CenterOnActor(Actor actor)
        {
            followComponent.Target = actor.SadCell;
        }

        /// <summary>
        /// centers the viewport camera on an Entity
        /// </summary>
        /// <param name="entity"></param>
        public void CenterOnActor(MagiEntity entity)
        {
            followComponent.Target = entity.SadCell;
        }

        public void CreateMapConsole()
        {
            MapConsole = new Console(Width, Height);
        }

        // Adds the entire list of entities found in the World.CurrentMap's Entities SpatialMap to
        // the MapConsole, so they can be seen onscreen
        private void SyncMapEntities(MagiMap map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            map.ConfigureRender(MapConsole);
        }

        /// <summary>
        /// Loads a Map into the MapConsole
        /// </summary>
        /// <param name="map"></param>
        public void LoadMap(MagiMap? map)
        {
            if (map is null)
                throw new NullValueException(nameof(map));
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

            Title = map.MapName;

            MapConsole.SadComponents.Add(followComponent);
        }

        public void Handle(ChangeCenteredActor message)
        {
            CenterOnActor(message.Entity);
        }

        public void Handle(LoadMapMessage message)
        {
            LoadMap(message.Map);
        }

        public void Handle(MapConsoleIsDirty message)
        {
            MapConsole.IsDirty = true;
        }

        ~MapWindow()
        {
            Locator.GetService<MessageBusService>().UnRegisterAllSubscriber(this);
        }
    }
}

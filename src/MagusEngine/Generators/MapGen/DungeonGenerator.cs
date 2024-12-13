using MagusEngine.Core.MapStuff;
using MagusEngine.Services.Factory;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Generators.MapGen
{
    // based on tunnelling room generation algorithm from RogueSharp tutorial https://roguesharp.wordpress.com/2016/03/26/roguesharp-v3-tutorial-simple-room-generation/
    public class DungeonGenerator : MapGenerator
    {
        public DungeonGenerator()
        {
            // empty
        }

        public MagiMap GenerateMazeMap(int maxRooms, int minRoomSize, int maxRoomSize)
        {
            // Create an empty map of size (mapWidht * mapHeight)
            _map = new MagiMap("Maze");

            // store a list of the rooms created so far
            List<Room> Rooms = new();

            // create up to (maxRooms) rooms on the map and make sure the rooms do not overlap with
            // each other
            for (int i = 0; i < maxRooms; i++)
            {
                // set the room's (width, height) as a random size between (minRoomSize, maxRoomSize)
                int newRoomWidth = randNum.NextInt(minRoomSize, maxRoomSize);
                int newRoomHeigth = randNum.NextInt(minRoomSize, maxRoomSize);

                // sets the room's X/Y Position at a random point between the edges of the map
                int newRoomX = randNum.NextInt(0, _map.Width - newRoomWidth - 1);
                int newRoomY = randNum.NextInt(0, _map.Height - newRoomHeigth - 1);

                // create a Rectangle representing the room's perimeter
                Rectangle roomRectangle = new(newRoomX, newRoomY, newRoomWidth, newRoomHeigth);
                Room newRoom = new(roomRectangle);

                // Does the new room intersect with other rooms already generated?
                bool doesRoomIntersect = Rooms.Any(room => newRoom.RoomRectangle.Intersects(room.RoomRectangle));

                if (!doesRoomIntersect)
                    Rooms.Add(newRoom);
            }

            // This is a dungeon, so begin by flooding the map with walls.
            FloodWalls();

            // carve out rooms for every room in the Rooms list
            foreach (var room in Rooms)
            {
                CreateRoom(room,
                    TileFactory.GenericStoneWall(), TileFactory.GenericStoneFloor());
            }

            // carve out tunnels between all rooms based on the Positions of their centers
            for (int r = 1; r < Rooms.Count; r++)
            {
                //for all remaining rooms get the center of the room and the previous room
                Point previousRoomCenter = Rooms[r - 1].RoomRectangle.Center;
                Point currentRoomCenter = Rooms[r].RoomRectangle.Center;

                // give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (randNum.NextInt(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                }
            }

            // Create doors now that the tunnels have been carved out
            foreach (var room in Rooms)
                CreateDoor(room);

            PlaceNodes(10);

            // spit out the final map
            return _map;
        }
    }
}

using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.MapStuff;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Serialization.MapConverter
{
    public class RoomTemplate : IJsonKey
    {
        // prefab maps
        public string Id { get; set; }
        public PrefabRoom Obj { get; set; }

        public Room ConfigureRoom(Point positionToBegin)
        {
            Rectangle rec = Obj.CreateRectangle()
                .WithPosition(positionToBegin);
            return new Room(rec, Obj.Tag, Obj.Furniture, Obj.Terrain)
            {
                Template = this
            };
        }

        public bool CompareRectanglesSameSize(Rectangle toCompare)
        {
            Rectangle rec = Obj.CreateRectangle();

            return toCompare.Size == rec.Size;
        }
    }

    public class PrefabRoom
    {
        public RoomTag Tag { get; set; }
        public string[] Rows { get; set; }
        public Dictionary<string, object> Terrain { get; set; }
        public Dictionary<string, object> Furniture { get; set; }

        public Rectangle CreateRectangle()
        {
            (int width, int height) = GetWidthAndHeight();
            return new(0, 0, width, height);
        }

        public (int, int) GetWidthAndHeight()
        {
            int width = 0;
            int height = Rows.Length;
            for (int i = 0; i < Rows.Length; i++)
            {
                string currentRow = Rows[i];
                width = currentRow.Length > width ? currentRow.Length : width;
            }

            return (width, height);
        }
    }
}
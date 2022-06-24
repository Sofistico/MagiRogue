using MagiRogue.GameSys;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization.MapSerialization
{
    public class RoomTemplate
    {
        // prefab maps
        public string Id { get; set; }
        public PrefabRoom Obj { get; set; }

        public Room ConfigureRoom(Point positionToBegin)
        {
            Rectangle rec = Obj.CreateRectangle()
                .WithPosition(positionToBegin);
            Room r = new Room(rec, Obj.Tag, Obj.Furniture, Obj.Terrain)
            {
                Template = this
            };

            CheckForEmptyTile();

            return r;
        }

        private void CheckForEmptyTile()
        {
            if (!string.IsNullOrEmpty(Obj.EmptyFill))
            {
            }
            return;
        }
    }

    public class PrefabRoom
    {
        public string? EmptyFill { get; set; }
        public RoomTag Tag { get; set; }
        public string[] Rows { get; set; }
        public Dictionary<string, object> Terrain { get; set; }
        public Dictionary<string, object> Furniture { get; set; }

        public Rectangle CreateRectangle()
        {
            (int width, int height) = GetWidthAndHeight();
            Rectangle rec = new Rectangle(0, 0, width, height);
            return rec;
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
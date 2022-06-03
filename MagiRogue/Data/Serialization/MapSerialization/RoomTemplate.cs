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

        public Room ConfigureRoom()
        {
            Rectangle rec = Obj.CreateRectangle();
            Room r = new Room(rec, Obj.Tag, Obj.Furniture, Obj.Terrain);

            return r;
        }
    }

    public class PrefabRoom
    {
        public string EmptyFill { get; set; }
        public RoomTag Tag { get; set; }
        public string[] Rows { get; set; }
        public Dictionary<string, object> Terrain { get; set; }
        public Dictionary<string, object> Furniture { get; set; }

        public Rectangle CreateRectangle()
        {
            int x = 0, y = 0;
            foreach (string xIdx in Rows)
            {
                foreach (char c in xIdx)
                {
                    y++;
                }
                x++;
            }

            return Rectangle.WithExtents(new SadRogue.Primitives.Point(0, 0), new Point(x, y));
        }
    }
}
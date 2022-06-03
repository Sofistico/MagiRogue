using MagiRogue.GameSys;
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
    }

    public class PrefabRoom
    {
        public string EmptyFill { get; set; }
        public RoomTag Tag { get; set; }
        public string[] Rows { get; set; }
        public List<dynamic> Terrain { get; set; }
        public List<dynamic> Furniture { get; set; }
    }
}
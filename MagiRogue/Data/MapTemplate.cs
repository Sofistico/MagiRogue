using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data
{
    public class MapJsonConverter : JsonConverter<Map>
    {
        public override Map ReadJson(JsonReader reader,
            Type objectType, Map? existingValue, bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<MapTemplate>(reader);

        public override void WriteJson(JsonWriter writer, Map? value, JsonSerializer serializer)
            => serializer.Serialize(writer, (MapTemplate)value);
    }

    public class MapTemplate
    {
        public string MapName { get; set; }
        public BasicTile[] Tiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point? LastPlayerPosition { get; set; }
        public uint MapId { get; private set; }
        public IList<Entity> Entities;

        public MapTemplate(string mapName,
            BasicTile[] tiles,
            int width,
            int height,
            Point? lastPlayerPosition,
            uint mapId)
        {
            MapName = mapName;
            Tiles = tiles;
            Width = width;
            Height = height;
            LastPlayerPosition = lastPlayerPosition;
            MapId = mapId;
        }

        public MapTemplate(string mapName, int width, int height)
        {
            MapName = mapName;
            Width = width;
            Height = height;
        }

        public static implicit operator MapTemplate(Map map)
        {
            BasicTile[] tiles = new BasicTile[map.Tiles.Length];
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                tiles[i] = map.Tiles[i];
            }

            MapTemplate template = new MapTemplate(map.MapName, tiles, map.Width,
                map.Height, map.LastPlayerPosition, map.MapId);

            template.Entities = (IList<Entity>)map.Entities.Items;

            return template;
        }

        public static implicit operator Map(MapTemplate map)
        {
            var objMap = new Map(map.MapName, map.Width, map.Height);

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                objMap.SetTerrain((TileBase)map.Tiles[i]);
            }
            for (int x = 0; x < map.Entities.Count; x++)
            {
                objMap.Add(map.Entities[x]);
            }
            objMap.SetId(map.MapId);
            objMap.LastPlayerPosition = map.LastPlayerPosition;

            return objMap;
        }
    }

    public class RegionChunkTemplate
    {
        public int X { get; }
        public int Y { get; }
        public MapTemplate[] LocalMaps { get; set; }

        public RegionChunkTemplate(int x, int y, MapTemplate[] localMaps)
        {
            X = x;
            Y = y;
            LocalMaps = localMaps;
        }

        public static implicit operator RegionChunkTemplate(RegionChunk region)
        {
            MapTemplate[] maps = new MapTemplate[region.LocalMaps.Length];
            for (int i = 0; i < region.LocalMaps.Length; i++)
            {
                maps[i] = region.LocalMaps[i];
            }
            return new(region.X, region.Y, maps);
        }
    }
}
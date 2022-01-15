using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class MapJsonConverter : JsonConverter<Map>
    {
        public override Map ReadJson(JsonReader reader,
            Type objectType, Map? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject futureMap = JObject.Load(reader);
            Map map = new Map(futureMap["MapName"].ToString(),
                (int)futureMap["Width"], (int)futureMap["Height"]);
            JToken[] tiles = futureMap["Tiles"].ToArray();
            JToken[] entities = futureMap["Entities"].ToArray();
            JToken[] explored = futureMap["Explored"].ToArray();
            uint id = (uint)futureMap["MapId"];
            Point lastPlayerPosition = new Point((int)futureMap["LastPlayerPosition"]["X"],
                    (int)futureMap["LastPlayerPosition"]["Y"]);
            map.SetId(id);
            for (int i = 0; i < tiles.Length; i++)
            {
                // tile handling
                Point pos = new Point((int)tiles[i]["Position"]["X"],
                    (int)tiles[i]["Position"]["Y"]);
                BasicTile tile = new
                    BasicTile(pos,
                    (uint)tiles[i]["Foreground"],
                    (uint)tiles[i]["Background"],
                    (char)tiles[i]["Glyph"],
                    (bool)tiles[i]["IsWalkable"],
                    (bool)tiles[i]["IsTransparent"],
                    tiles[i]["IdMaterial"].ToString(),
                    tiles[i]["Name"].ToString(),
                    EnumToString(tiles[i]["TileType"].ToString()),
                    (int)tiles[i]["MoveCost"]);
                tile.InfusedMp = (int)tiles[i]["InfusedMp"];

                ParametizeTile(tile, tiles[i]);

                map.SetTerrain((TileBase)tile);

                // handle explored flags
                map.PlayerExplored[i] = (bool)explored[i];
            }

            // entity handling
            for (int i = 0; i < entities.Length; i++)
            {
                JToken entity = entities[i];

                var name = entity["Name"].ToString();
                var foreground = (uint)entity["ForegroundPackedValue"];
                var background = (uint)entity["BackgroundPackedValue"];
                var glyph = (char)entity["Glyph"];
                var size = (int)entity["Size"];
                var weight = (float)entity["Weight"];
                var materialId = entity["MaterialId"].ToString();
                MagicManager magic;
                if (entity.SelectToken("MagicStuff") is not null)
                    magic = JsonConvert.DeserializeObject<MagicManager>(entity["MagicStuff"].ToString());
                else
                    magic = new();

                if (entity["EntityType"].ToString().Equals("Actor"))
                {
                    Stat stats = JsonConvert.DeserializeObject<Stat>(entity["Stats"].ToString());
                    Anatomy anatomy =
                        JsonConvert.DeserializeObject<Anatomy>(entity["Anatomy"].ToString());
                    List<AbilityTemplate> abilities = new List<AbilityTemplate>();
                    JToken[] jAbi = entity["Abilities"].ToArray();
                    for (int z = 0; z < jAbi.Length; z++)
                    {
                        if (jAbi[i] is not null)
                            abilities.Add
                                (JsonConvert.DeserializeObject<AbilityTemplate>(jAbi[z].ToString()));
                    }
                    var layer = (int)entity["Layer"];

                    Actor actor = new ActorTemplate(
                        name,
                        foreground,
                        background,
                        glyph,
                        layer,
                        stats,
                        anatomy,
                        size,
                        weight,
                        materialId,
                        abilities,
                        magic
                        );

                    if (entity.SelectToken("Description") is not null)
                    {
                        actor.Description = entity["Description"].ToString();
                    }
                    if (entity.SelectToken("Position") is not null)
                        actor.Position =
                            new Point((int)entity["Position"]["X"], (int)entity["Position"]["Y"]);
                    if (entity.SelectToken("IsPlayer") is not null && (bool)entity["IsPlayer"])
                    {
                        Player player = Player.ReturnPlayerFromActor(actor);
                        map.Add(player);
                    }
                    else
                        map.Add(actor);
                }
                if (entity["EntityType"].ToString().Equals("Item"))
                {
                    var condition = (int)entity["Condition"];

                    Item item = new ItemTemplate(
                        name,
                        foreground,
                        background,
                        glyph,
                        weight,
                        size,
                        materialId,
                        magic,
                        condition);

                    if (entity.SelectToken("Description") is not null)
                    {
                        item.Description = entity["Description"].ToString();
                    }

                    if (entity.SelectToken("Position") is not null)
                        item.Position =
                            new Point((int)entity["Position"]["X"], (int)entity["Position"]["Y"]);
                }
            }

            map.LastPlayerPosition = lastPlayerPosition;

            return map;
        }

        private void ParametizeTile(BasicTile tile, JToken jToken)
        {
            switch (tile.TileType)
            {
                case TileType.Null:
                    throw new Exception($"Tried to load a null tile! tile in question: {tile.Name}");

                case TileType.Water:
                    tile.IsSea = (bool)jToken["IsSea"];
                    break;

                case TileType.Node:
                    tile.MpPower = (int)jToken["MpPower"];
                    tile.NodeStrenght = (int)jToken["NodeStrenght"];
                    break;

                case TileType.WorldTile:
                    // belive i will need this, let's see.
                    break;

                case TileType.Door:
                    tile.Locked = (bool)jToken["Locked"];
                    tile.IsOpen = (bool)jToken["IsOpen"];
                    break;

                default:
                    break;
            }
        }

        private TileType EnumToString(string tileType)
        {
            return tileType switch
            {
                "Null" => TileType.Null,
                "Floor" => TileType.Floor,
                "Wall" => TileType.Wall,
                "WorldTile" => TileType.WorldTile,
                "Node" => TileType.Node,
                "Door" => TileType.Door,
                "Water" => TileType.Water,
                _ => TileType.Null
            };
        }

        public override void WriteJson(JsonWriter writer, Map? value, JsonSerializer serializer)
            => serializer.Serialize(writer, (MapTemplate)value);
    }
}

public class MapTemplate
{
    public string MapName { get; set; }
    public BasicTile[] Tiles { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Point LastPlayerPosition { get; set; }
    public uint MapId { get; private set; }
    public IList<Entity> Entities;
    public bool[] Explored;

    public MapTemplate(string mapName,
        BasicTile[] tiles,
        int width,
        int height,
        Point lastPlayerPosition,
        uint mapId, bool[] explored)
    {
        MapName = mapName;
        Tiles = tiles;
        Width = width;
        Height = height;
        LastPlayerPosition = lastPlayerPosition;
        MapId = mapId;
        Explored = explored;
    }

    public MapTemplate(string mapName, int width, int height)
    {
        MapName = mapName;
        Width = width;
        Height = height;
    }

    public MapTemplate()
    {
        // empty
    }

    public static implicit operator MapTemplate(Map map)
    {
        if (map == null)
            return null;
        BasicTile[] tiles = new BasicTile[map.Tiles.Length];
        for (int i = 0; i < map.Tiles.Length; i++)
        {
            tiles[i] = map.Tiles[i];
        }

        MapTemplate template = new MapTemplate(map.MapName, tiles, map.Width,
            map.Height, map.LastPlayerPosition, map.MapId, map.PlayerExplored.ToArray());

        List<Entity> entities = new List<Entity>();

        foreach (Entity item in map.Entities.Items)
        {
            entities.Add(item);
        }

        template.Entities = entities;

        return template;
    }

    public static implicit operator Map(MapTemplate map)
    {
        var objMap = new Map(map.MapName, map.Width, map.Height);

        for (int i = 0; i < map.Tiles.Length; i++)
        {
            if (objMap.Tiles[i] == null)
                continue;
            objMap.SetTerrain((TileBase)map.Tiles[i]);
        }
        for (int x = 0; x < map.Entities.Count; x++)
        {
            objMap.Add(map.Entities[x]);
        }
        objMap.SetId(map.MapId);
        objMap.LastPlayerPosition = map.LastPlayerPosition;
        objMap.PlayerExplored = new SadRogue.Primitives.GridViews.ArrayView<bool>(
            map.Explored, map.Width);

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
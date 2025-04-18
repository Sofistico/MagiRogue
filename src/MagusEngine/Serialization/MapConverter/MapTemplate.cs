﻿using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Serialization.MapConverter
{
    #region Serialization

    public sealed class MapJsonConverter : JsonConverter<MagiMap>
    {
        public override MagiMap ReadJson(JsonReader reader,
            Type objectType, MagiMap? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            #region Old Code

            /*JObject futureMap = JObject.Load(reader);
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
                Point pos = Point.None;
                // tile handling
                if (tiles[i].SelectToken("Position") is not null)
                {
                    pos = new Point((int)tiles[i]["Position"]["X"],
                        (int)tiles[i]["Position"]["Y"]);
                }
                if (tiles[i].SelectToken("Glyph") is null)
                    continue;
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

                map.SetTerrain((Tile)tile);

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
                    Dictionary<Limb, Item> equips = new();
                    JToken[] jEquip = entity["Equip"].ToArray();
                    for (int z = 0; z < jAbi.Length; z++)
                    {
                        if (jAbi[z] is not null)
                            abilities.Add
                                (JsonConvert.DeserializeObject<AbilityTemplate>(jAbi[z].ToString()));
                    }
                    for (int u = 0; u < jEquip.Length; u++)
                    {
                        Limb limb =
                            JsonConvert.DeserializeObject<LimbTemplate>(jEquip[u]["LimbEquipped"].ToString());
                        Item item =
                            JsonConvert.DeserializeObject<ItemTemplate>(jEquip[u]["ItemEquipped"].ToString());
                        equips.TryAdd(limb, item);
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

                    actor.Equipment = equips;

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
            map.SetSeed((int)futureMap["Seed"]);

            reader.CloseInput = true;

            return map;*/

            #endregion Old Code

            return serializer.Deserialize<MapTemplate>(reader);
        }

        /*private void ParametizeTile(BasicTile tile, JToken jToken)
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

                    tile.BiomeType = BiomeToString(jToken["BiomeType"].ToString());
                    tile.BitMask = (int)jToken["BitMask"];
                    tile.MagicalAura = (int)jToken["MagicalAura"];
                    tile.BiomeBitMask = (int)jToken["BiomeBitMask"];
                    //tile.Civ = jToken[];
                    var riverjTok = jToken["Rivers"].ToArray();

                    for (int i = 0; i < riverjTok.Length; i++)
                    {
                        tile.Rivers.Add(
                            JsonConvert.DeserializeObject<River>(riverjTok[i].ToString()));
                    }
                    //tile.Civ = JsonConvert.DeserializeObject<Civilization>(jToken["Civ"].ToString());

                    break;

                case TileType.Door:
                    tile.Locked = (bool)jToken["Locked"];
                    tile.IsOpen = (bool)jToken["IsOpen"];
                    break;

                default:
                    return;
            }
        }

        private static TileType EnumToString(string tileType)
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

        private static BiomeType BiomeToString(string biome)
        {
            return biome switch
            {
                "Sea" => BiomeType.Sea,
                "Desert" => BiomeType.Desert,
                "Savanna" => BiomeType.Savanna,
                "TropicalRainforest" => BiomeType.TropicalRainforest,
                "Grassland" => BiomeType.Grassland,
                "Woodland" => BiomeType.Woodland,
                "SeasonalForest" => BiomeType.SeasonalForest,
                "TemperateRainforest" => BiomeType.TemperateRainforest,
                "BorealForest" => BiomeType.BorealForest,
                "Tundra" => BiomeType.Tundra,
                "Ice" => BiomeType.Ice,
                "Mountain" => BiomeType.Mountain,
                _ => BiomeType.Null,
            };
        }*/

        public override void WriteJson(JsonWriter writer, MagiMap? value, JsonSerializer serializer)
        {
            MapTemplate template = (MapTemplate)value;
            /*if (template.Entities.Count == 0)
                template.Entities = null;*/
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, template);
            writer.Flush();
        }
    }

    #endregion Serialization

    public sealed class MapTemplate
    {
        public string MapName { get; set; }
        public Tile[] Tiles { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point LastPlayerPosition { get; set; }
        public uint MapId { get; private set; }
        public List<Actor> Actors { get; set; }
        public List<Item> Items { get; set; }
        public bool[] Explored { get; set; }
        public ulong Seed { get; set; }
        public bool? HasFOV { get; set; }
        public int ZAmount { get; set; }
        public List<Room> Rooms { get; set; }
        //public Light[] Ilumination { get; set; }

        [JsonConstructor]
        public MapTemplate(string mapName,
            Tile[] tiles,
            int width,
            int height,
            Point lastPlayerPosition,
            uint mapId, bool[] explored)
        //Light[] ilumination)
        {
            MapName = mapName;
            Tiles = tiles;
            Width = width;
            Height = height;
            LastPlayerPosition = lastPlayerPosition;
            MapId = mapId;
            Explored = explored;
            //Ilumination = ilumination;
        }

        public MapTemplate(string mapName, int width, int height)
        {
            MapName = mapName;
            Width = width;
            Height = height;
        }

        public static implicit operator MapTemplate(MagiMap map)
        {
            var tiles = new Tile[map.Terrain.Count];
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                tiles[i] = (Tile)map.Terrain[i];
            }

            MapTemplate template = new MapTemplate(map.MapName, tiles, map.Width,
                map.Height, map.LastPlayerPosition, map.MapId, map.PlayerExplored.ToArray());

            List<Item> items = new List<Item>();
            List<Actor> actors = new List<Actor>();

            for (int i = 0; i < map.Entities.Count; i++)
            {
                MagiEntity entity = (MagiEntity)map.Entities.Items.ToArray()[i];
                if (entity is Actor actor) { actors.Add(actor); }
                if (entity is Item item) { items.Add(item); }
                if (entity is Player player) { map.LastPlayerPosition = player.Position; }
            }

            template.Actors = actors;
            template.Items = items;
            template.Seed = map.Seed;
            template.LastPlayerPosition = map.LastPlayerPosition;

            if (!map.MapName.Equals("Planet"))
                template.HasFOV = true;
            else
                template.HasFOV = false;

            template.ZAmount = map.ZAmount;
            template.Rooms = map.Rooms;
            return template;
        }

        public static implicit operator MagiMap(MapTemplate map)
        {
            if (map is null)
                return null;
            var objMap = new MagiMap(map.MapName, map.Width, map.Height);

            if (map.HasFOV is not null && (bool)!map.HasFOV)
                objMap.GoRogueComponents.GetFirstOrDefault<MagiRogueFOVVisibilityHandler>().Disable();

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                if (map.Tiles[i] == null)
                    continue;
                var tile = (Tile)map.Tiles[i];
                objMap.SetTerrain(tile);
            }
            for (int x = 0; x < map.Actors.Count; x++)
            {
                if (!map.Actors[x].IsPlayer)
                {
                    //objMap.Add(Player.ReturnPlayerFromActor(map.Actors[x]));
                    objMap.AddMagiEntity(map.Actors[x]);
                }
            }
            for (int x = 0; x < map.Items.Count; x++)
            {
                objMap.AddMagiEntity(map.Items[x]);
            }
            objMap.SetId(map.MapId);
            objMap.LastPlayerPosition = map.LastPlayerPosition;
            objMap.PlayerExplored = new SadRogue.Primitives.GridViews.ArrayView<bool>(
                map.Explored, map.Width);
            objMap.SetSeed(map.Seed);
            objMap?.GoRogueComponents.GetFirstOrDefault<MagiRogueFOVVisibilityHandler>()?.RefreshExploredTerrain();
            objMap.ZAmount = map.ZAmount;
            objMap.Rooms = map.Rooms;
            return objMap;
        }
    }
}
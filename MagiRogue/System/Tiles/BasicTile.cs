using SadRogue.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using MagiRogue.System.Planet;
using MagiRogue.System.Civ;
using MagiRogue.Data.Serialization;

namespace MagiRogue.System.Tiles
{
    public class TileJsonConverter : JsonConverter<TileBase>
    {
        public override TileBase ReadJson(JsonReader reader,
            Type objectType, TileBase? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return (TileBase)serializer.Deserialize<BasicTile>(reader);
        }

        public override void WriteJson(JsonWriter writer, TileBase? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (BasicTile)value);
        }
    }

    public class BasicTile
    {
        [JsonProperty(Required = Required.AllowNull)]
        public Point Position { get; set; }
        public uint Foreground { get; set; }
        public uint Background { get; set; }
        public int Glyph { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public string IdMaterial { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public static int Layer { get => 0; }
        public TileType TileType { get; set; }
        public int MoveCost { get; set; }

        public bool? IsSea { get; set; }

        /// <summary>
        /// The maximum amount of mp the node can have
        /// </summary>
        public int? MpPower { get; set; }
        public int? NodeStrenght { get; set; }
        public bool? Locked { get; set; }
        public bool? IsOpen { get; set; }
        public int InfusedMp { get; set; }
        public BiomeType? BiomeType { get; set; }
        public int? MagicalAura { get; set; }
        public int? BiomeBitMask { get; set; }
        public List<River> Rivers { get; set; }
        public int BitMask { get; set; }
        public int? RiverSize { get; set; }
        public string? RegionName { get; set; }
        public Road? Road { get; set; }
        public SpecialLandType? SpecialLandType { get; set; }
        public HeatType? HeatType { get; set; }
        public float? HeatValue { get; set; }
        public HeightType? HeightType { get; set; }
        public float? HeightValue { get; set; }
        public float? MineralValue { get; set; }
        public bool? Visited { get; set; }
        public MoistureType? MoistureType { get; set; }
        public float? MoistureValue { get; set; }
        public int TileHealth { get; set; }
        public uint ForegroundLastSeen { get; set; }
        public uint BackgroundLastSeen { get; set; }
        public char GlyphLastSeen { get; set; }

        public BasicTile(Point position,
            uint foreground,
            uint background,
            int glyph,
            bool isWalkable,
            bool isTransparent,
            string idMaterial,
            string name,
            TileType tileType,
            int moveCost)
        {
            Position = position;
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            IsWalkable = isWalkable;
            IsTransparent = isTransparent;
            IdMaterial = idMaterial;
            Name = name;
            TileType = tileType;
            MoveCost = moveCost;
        }

        public BasicTile()
        {
            //Empty ctor
        }

        public static implicit operator BasicTile(TileBase tile)
        {
            TileType type;
            bool? isSea = null;
            if (tile is null)
                return null;

            if (tile.MaterialOfTile is null)
                tile.MaterialOfTile = Physics.PhysicsManager.SetMaterial("null");

            BasicTile basic = new BasicTile(tile.Position, tile.Foreground.PackedValue,
                tile.Background.PackedValue, tile.Glyph, tile.IsWalkable,
                tile.IsTransparent, tile.MaterialOfTile.Id, tile.Name, TileType.Null,
                tile.MoveTimeCost);
            if (tile is not WorldTile && !tile.Matches(tile.LastSeenAppereance))
            {
                basic.Foreground = tile.LastSeenAppereance.Foreground.PackedValue;
                basic.Background = tile.LastSeenAppereance.Background.PackedValue;
            }

            if (!string.IsNullOrWhiteSpace(tile.Description))
                basic.Description = tile.Description;

            if (tile is TileWall)
                type = TileType.Wall;
            else if (tile is TileFloor)
                type = TileType.Floor;
            else if (tile is NodeTile node)
            {
                type = TileType.Node;
                basic.NodeStrenght = node.NodeStrength;
                basic.MpPower = node.MaxMp;
            }
            else if (tile is WaterTile seaTile)
            {
                type = TileType.Water;
                isSea = seaTile.IsSea;
            }
            else if (tile is WorldTile worldTile)
            {
                basic.Rivers = new();
                for (int i = 0; i < worldTile.Rivers.Count; i++)
                {
                    basic.Rivers.Add(worldTile.Rivers[i]);
                }
                type = TileType.WorldTile;
                basic.BiomeType = worldTile.BiomeType;
                basic.BiomeBitMask = worldTile.BiomeBitmask;
                //basic.Rivers = worldTile.Rivers;
                //basic.Civ = worldTile.CivInfluence;
                basic.RiverSize = worldTile.RiverSize;
                basic.Road = worldTile.Road;
                basic.SpecialLandType = worldTile.SpecialLandType;
                basic.RegionName = worldTile.RegionName;
                basic.HeatType = worldTile.HeatType;
                basic.HeatValue = worldTile.HeatValue;
                basic.HeightType = worldTile.HeightType;
                basic.HeightValue = worldTile.HeightValue;
                basic.Visited = worldTile.Visited;
                basic.MoistureType = worldTile.MoistureType;
                basic.MoistureValue = worldTile.MoistureValue;
                basic.MineralValue = worldTile.MineralValue;
                basic.MagicalAura = worldTile.MagicalAuraStrength;
            }
            else if (tile is TileDoor door)
            {
                type = TileType.Door;
                basic.IsOpen = door.IsOpen;
                basic.Locked = door.Locked;
            }
            else
                type = TileType.Null;

            basic.TileType = type;
            basic.InfusedMp = tile.InfusedMp;
            basic.IsSea = isSea;
            basic.TileHealth = tile.TileHealth;
            basic.BitMask = tile.BitMask;

            return basic;
        }

        public static explicit operator TileBase(BasicTile basicTile)
        {
            TileBase tile;
            if (basicTile == null)
                return null;
            switch (basicTile.TileType)
            {
                case TileType.Null:
                    throw new Exception($"Tried to instantiete a tile which is null! " +
                        $"Tile: {basicTile.TileType} / {basicTile.Name}");

                case TileType.Floor:
                    tile = new TileFloor(basicTile.Name,
                        basicTile.Position,
                        basicTile.IdMaterial,
                        basicTile.Glyph,
                        new Color(basicTile.Foreground),
                        new Color(basicTile.Background));
                    break;

                case TileType.Wall:
                    tile = new TileWall(new Color(basicTile.Foreground),
                        new Color(basicTile.Background),
                        basicTile.Glyph, basicTile.Name,
                        basicTile.Position,
                        basicTile.IdMaterial);
                    break;

                case TileType.Water:
                    tile = new WaterTile(new Color(basicTile.Foreground),
                        new Color(basicTile.Background),
                        basicTile.Glyph,
                        basicTile.Position,
#pragma warning disable CS8629 // O tipo de valor de nulidade pode ser nulo.
                        (bool)basicTile.IsSea);
#pragma warning restore CS8629 // O tipo de valor de nulidade pode ser nulo.
                    break;

                case TileType.Node:
                    tile = new NodeTile(new Color(basicTile.Foreground),
                        new Color(basicTile.Background),
                        basicTile.Position,
                        (int)basicTile.MpPower,
                        (int)basicTile.NodeStrenght);
                    break;

                case TileType.WorldTile:
                    tile = new WorldTile(new Color(basicTile.Foreground),
                        new Color(basicTile.Background),
                        basicTile.Glyph,
                        basicTile.Position,
                        name: basicTile.Name)
                    {
                        BitMask = basicTile.BitMask,
                        BiomeBitmask = (int)basicTile.BiomeBitMask,
                        BiomeType = (BiomeType)basicTile.BiomeType,
                        //CivInfluence = basicTile.Civ,
                        Rivers = basicTile.Rivers,
                        RiverSize = (int)basicTile.RiverSize,
                        RegionName = basicTile.RegionName,
                        Road = basicTile.Road,
                        SpecialLandType = (SpecialLandType)basicTile.SpecialLandType,
                        MagicalAuraStrength = (int)basicTile.MagicalAura,
                        HeatType = (HeatType)basicTile.HeatType,
                        HeatValue = (float)basicTile.HeatValue,
                        HeightType = (HeightType)basicTile.HeightType,
                        HeightValue = (float)basicTile.HeightValue,
                        MineralValue = (float)basicTile.MineralValue,
                        Visited = (bool)basicTile.Visited,
                        MoistureType = (MoistureType)basicTile.MoistureType,
                        MoistureValue = (float)basicTile.MoistureValue,
                    };
                    tile.BitMask = basicTile.BitMask;
                    break;

                case TileType.Door:
                    tile = new TileDoor((bool)basicTile.Locked,
                        (bool)basicTile.IsOpen, basicTile.Position,
                        basicTile.IdMaterial);
                    break;

                default:
                    return null;
            }

            tile.Description = basicTile.Description;
            tile.InfusedMp = basicTile.InfusedMp;
            tile.TileHealth = basicTile.TileHealth;

            return tile;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TileType
    {
        Null,
        Floor,
        Wall,
        Water,
        Node,
        WorldTile,
        Door
    }
}
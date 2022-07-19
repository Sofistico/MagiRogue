using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Tiles
{
    #region Tile Serializer

    public class TileJsonConverter : JsonConverter<TileBase>
    {
        public override TileBase ReadJson(JsonReader reader,
            Type objectType, TileBase? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (TileBase)serializer.Deserialize<BasicTile>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, TileBase? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (BasicTile)value);
        }
    }

    #endregion Tile Serializer

    public class BasicTile
    {
        #region TileBase Properties

        public string? TileId { get; set; }
        public Point Position { get; set; }
        public uint Foreground { get; set; }
        public string? ForegroundStr { get; set; }
        public uint Background { get; set; }
        public string? BackgroundStr { get; set; }

        [JsonIgnore]
        public MagiColorSerialization ForegroundBackingField { get; internal set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; internal set; }
        public int Glyph { get; set; }
        public char? GlyphChar { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public string IdMaterial { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public static int Layer { get => 0; }
        public TileType TileType { get; set; }
        public int MoveCost { get; set; }
        public int TileHealth { get; set; }
        public uint ForegroundLastSeen { get; set; }
        public uint BackgroundLastSeen { get; set; }
        public char GlyphLastSeen { get; set; }
        public int InfusedMp { get; set; }
        public int BitMask { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Traits { get; set; }

        #endregion TileBase Properties

        #region NodeTile properties

        public int? MpPower { get; set; }
        public int? NodeStrenght { get; set; }

        #endregion NodeTile properties

        #region WorldTile properties

        public bool? IsSea { get; set; }

        /// <summary>
        /// The maximum amount of mp the node can have
        /// </summary>
        public bool? Locked { get; set; }
        public bool? IsOpen { get; set; }
        public BiomeType? BiomeType { get; set; }
        public int? MagicalAura { get; set; }
        public int? BiomeBitMask { get; set; }
        public List<River> Rivers { get; set; }
        public int? RiverSize { get; set; }
        public string? RegionName { get; set; }
        public Road? Road { get; set; }
        public Civilization? Civ { get; set; }
        public SpecialLandType? SpecialLandType { get; set; }
        public HeatType? HeatType { get; set; }
        public float? HeatValue { get; set; }
        public HeightType? HeightType { get; set; }
        public float? HeightValue { get; set; }
        public float? MineralValue { get; set; }
        public bool? Visited { get; set; }
        public MoistureType? MoistureType { get; set; }
        public float? MoistureValue { get; set; }

        #endregion WorldTile properties

        #region Ctor

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

        #endregion Ctor

        #region Methods

        public TileBase Copy()
        {
            TileBase tile = (TileBase)this;
            return tile;
        }

        public TileBase Copy(Point pos)
        {
            TileBase tile = (TileBase)this;
            tile.Position = pos;
            return tile;
        }

        public bool HasAnyTrait()
        {
            Traits = Traits is null ? new List<Trait>() : Traits;
            var material = Physics.PhysicsManager.SetMaterial(IdMaterial);
            if (material.ConfersTraits is not null)
            {
                Traits.AddRange(material.ConfersTraits);
                return true;
            }
            return false;
        }

        #endregion Methods

        #region operator overload

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
                basic.Civ = worldTile.CivInfluence;
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
            basic.Traits = tile.Traits;

            return basic;
        }

        public static explicit operator TileBase(BasicTile basicTile)
        {
            if (basicTile is null)
            {
                //Debug.WriteLine("Basic tile was null!");
                return null;
            }

            TileBase tile;
            // Keep a lookout if this part of the code will stop working
            int charToUse = basicTile.GlyphChar is null ? basicTile.Glyph : basicTile.GlyphChar.Value;
            basicTile.BackgroundStr = basicTile.BackgroundStr is null ? "Transparent" : basicTile.BackgroundStr;
            Color foreground;
            Color background;
            if (!string.IsNullOrEmpty(basicTile.BackgroundStr) && !string.IsNullOrEmpty(basicTile.ForegroundStr))
            {
                foreground = new MagiColorSerialization(basicTile.ForegroundStr).Color;
                background = new MagiColorSerialization(basicTile.BackgroundStr).Color;
            }
            else
            {
                foreground = new Color(basicTile.Foreground);
                background = new Color(basicTile.Background);
            }
            switch (basicTile.TileType)
            {
                case TileType.Null:
                    throw new Exception($"Tried to instantiete a tile which is null! " +
                        $"Tile: {basicTile.TileType} / {basicTile.Name}");

                case TileType.Floor:
                    tile = new TileFloor(basicTile.Name,
                        basicTile.Position,
                        basicTile.IdMaterial,
                        charToUse,
                        foreground,
                        background);
                    break;

                case TileType.Wall:
                    tile = new TileWall(foreground,
                        background,
                        charToUse, basicTile.Name,
                        basicTile.Position,
                        basicTile.IdMaterial);
                    break;

                case TileType.Water:
                    tile = new WaterTile(foreground,
                        background,
                        charToUse,
                        basicTile.Position,
#pragma warning disable CS8629 // O tipo de valor de nulidade pode ser nulo.
                        (bool)basicTile.IsSea);
#pragma warning restore CS8629 // O tipo de valor de nulidade pode ser nulo.
                    break;

                case TileType.Node:
                    tile = new NodeTile(foreground,
                        background,
                        basicTile.Position,
                        (int)basicTile.MpPower,
                        (int)basicTile.NodeStrenght);
                    break;

                case TileType.WorldTile:
                    tile = new WorldTile(foreground,
                        background,
                        charToUse,
                        basicTile.Position,
                        !basicTile.IsWalkable,
                        basicTile.IsTransparent,
                        name: basicTile.Name)
                    {
                        BitMask = basicTile.BitMask,
                        BiomeBitmask = (int)basicTile.BiomeBitMask,
                        BiomeType = (BiomeType)basicTile.BiomeType,
                        CivInfluence = basicTile.Civ,
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
                    tile.IsBlockingMove = !basicTile.IsWalkable;
                    break;

                case TileType.Door:
                    tile = new TileDoor(basicTile.Name, basicTile.Locked ?? false,
                        basicTile.IsOpen ?? false, basicTile.Position,
                        basicTile.IdMaterial)
                    {
                        Foreground = foreground
                    };
                    break;

                default:
                    return null;
            }

            tile.Description = basicTile.Description;
            tile.InfusedMp = basicTile.InfusedMp;
            if (basicTile.TileHealth > 0)
                tile.TileHealth = basicTile.TileHealth;

            tile.Traits = basicTile.Traits is null ? new List<Trait>() : basicTile.Traits;

            return tile;
        }

        #endregion operator overload
    }
}
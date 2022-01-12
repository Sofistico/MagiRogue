using SadRogue.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace MagiRogue.System.Tiles
{
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
        public int Layer { get => 0; }
        public TileType TileType { get; set; }
        public int MoveCost { get; set; }

        [JsonIgnore]
        public bool? IsSea { get; set; }

        /// <summary>
        /// The maximum amount of mp the node can have
        /// </summary>
        public int MpPower { get; set; }
        public int NodeStrenght { get; set; }
        public bool Locked { get; set; }
        public bool IsOpen { get; set; }
        public int InfusedMp { get; set; }

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

            BasicTile basic = new BasicTile(tile.Position, tile.Foreground.PackedValue,
                tile.Background.PackedValue, tile.Glyph, tile.IsWalkable,
                tile.IsTransparent, tile.MaterialOfTile.Id, tile.Name, TileType.Null,
                tile.MoveTimeCost);

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
            else if (tile is WorldTile)
                type = TileType.WorldTile;
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
                        basicTile.MpPower,
                        basicTile.NodeStrenght);
                    break;

                case TileType.WorldTile:
                    tile = new WorldTile(new Color(basicTile.Foreground),
                        new Color(basicTile.Background),
                        basicTile.Glyph,
                        basicTile.Position,
                        name: basicTile.Name);
                    break;

                case TileType.Door:
                    tile = new TileDoor(basicTile.Locked,
                        basicTile.IsOpen, basicTile.Position,
                        basicTile.IdMaterial);
                    break;

                default:
                    return null;
            }

            tile.Description = basicTile.Description;
            tile.InfusedMp = basicTile.InfusedMp;

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
/*using MagiRogue.System.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization
{
    public class TileBaseTemplate
    {
        public string Name { get; set; }
        public string TileId { get; set; }
        public string IdMaterial { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public char Glyph { get; set; }
        public string? Description { get; set; }
        public TileType TileType { get; set; }
        public int MoveCost { get; set; }
        public bool? IsSea { get; set; }
        public int? MpPower { get; set; }
        public int? NodeStrenght { get; set; }

        internal TileBase Copy()
        {
            TileBase tile;
            switch (TileType)
            {
                case TileType.Null:
                    throw new Exception($"Tried to instantiete a tile which is null! " +
                        $"Tile: {TileType} / {Name}");

                case TileType.Floor:
                    tile = new TileFloor(Name,
                        Point.None,
                        IdMaterial,
                        Glyph,
                        new Color(Foreground),
                        new Color(Background));
                    break;

                case TileType.Wall:
                    tile = new TileWall(new Color(Foreground),
                        new Color(Background),
                        Glyph, Name,
                        Point.None,
                        IdMaterial);
                    break;

                case TileType.Water:
                    tile = new WaterTile(new Color(Foreground),
                        new Color(Background),
                        Glyph,
                        Point.None,
#pragma warning disable CS8629 // O tipo de valor de nulidade pode ser nulo.
                        (bool)IsSea);
#pragma warning restore CS8629 // O tipo de valor de nulidade pode ser nulo.
                    break;

                case TileType.Node:
                    tile = new NodeTile(new Color(Foreground),
                        new Color(Background),
                        Point.None,
                        (int)MpPower,
                        (int)NodeStrenght);
                    break;
            }
        }
    }*/
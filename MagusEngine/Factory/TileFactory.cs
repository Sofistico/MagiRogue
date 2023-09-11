using Arquimedes;
using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using MagusEngine.Core;
using MagusEngine.ECS.Components;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.Factory
{
    public static class TileFactory
    {
        public static Tile GenericGrass(Point pos, Map? map = null)
        {
            var tile = CreateTile(pos, TileType.Floor, "t_soil");
            // TODO: REDO
            //for (int i = 0; i < tile.Vegetations.Length; i++)
            //{
            //    tile.AddVegetation(DataManager.QueryPlantInData("grass"), i, (GameSys.Map?)map);
            //}

            return tile;
        }

        public static Tile GenericDirtRoad(Point pos)
        {
            var road = CreateTile(pos, TileType.Floor, "dirt");
            road.Appearence.Foreground = MagiPalette.DirtRoad;
            return road;
        }

        public static Tile GenericTree()
        {
            var tile = new Tile(MagiPalette.Wood, Color.Black, 'O', false, false, Point.None)
            {
                Name = "Tree",
            };
            tile.AddComponent<MaterialComponent>(new("wood"));
            return tile;
        }

        public static Tile GenericTree(Point pos)
        {
            var tile = new Tile(MagiPalette.Wood, Color.Black, 'O', false, false, pos)
            {
                Name = "Tree",
            };
            tile.AddComponent<MaterialComponent>(new("wood"));
            return tile;
        }

        public static Tile GenericStoneWall()
        {
            return GenericStoneWall(Point.None);
        }

        internal static Tile GenericStoneWall(Point pos)
        {
            return CreateTile(pos, TileType.Wall, "stone");
        }

        internal static Tile GenericStoneFloor()
        {
            return GenericStoneFloor(Point.None);
        }

        internal static Tile GenericStoneFloor(Point pos)
        {
            return CreateTile(pos, TileType.Floor, "stone");
        }

        public static Tile GenericTreeTrunk(Point pos)
        {
            return CreateTile(pos, TileType.TreeTrunk, "wood");
        }

        public static Tile CreateTile(Point pos, TileType tileType, string? materialId = null,
            MaterialType typeToMake = MaterialType.None)
        {
            MaterialTemplate? material;
            if (!string.IsNullOrEmpty(materialId))
            {
                material = DataManager.QueryMaterial(materialId);
            }
            else
            {
                material = DataManager.QueryMaterialWithType(typeToMake);
            }
            var (foreground, background, glyph, isWalkable, isTransparent, name)
                = DetermineTileLookAndName(material!, tileType);
            return new Tile(foreground, background, glyph, isWalkable, isTransparent, pos, name, material!.Id);
        }

        private static (Color, Color, char, bool, bool, string) DetermineTileLookAndName(MaterialTemplate material,
            TileType tileType)
        {
            char glyph;
            bool isTransparent;
            bool isWalkable;
            switch (tileType)
            {
                case TileType.Wall:
                    glyph = '#';
                    isTransparent = false;
                    isWalkable = false;
                    break;

                case TileType.Liquid:
                    glyph = '~';
                    isTransparent = true;
                    isWalkable = true;
                    break;

                case TileType.Node:
                    glyph = '*';
                    isTransparent = true;
                    isWalkable = true;
                    break;

                case TileType.Door:
                    glyph = '+';
                    isTransparent = false;
                    isWalkable = false;
                    break;

                case TileType.TreeTrunk:
                    glyph = 'O';
                    isTransparent = false;
                    isWalkable = false;
                    break;

                default:
                    glyph = '.';
                    isTransparent = true;
                    isWalkable = true;
                    break;
            }
            string name = material.Name + " " + tileType.ToString();
            return (material.ReturnMagiColor().Color, Color.Black, glyph, isWalkable, isTransparent, name);
        }
    }
}
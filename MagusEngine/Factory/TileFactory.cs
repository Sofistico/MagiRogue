using Arquimedes;
using Arquimedes.Enumerators;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.EntityComponents;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Systems;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Factory
{
    public static class TileFactory
    {
        private static Material? cachedMaterial;

        public static Tile GenericGrass(Point pos)
        {
            var tile = CreateTile(pos, TileType.Floor, "dirt");
            var plant = DataManager.QueryPlantInData("grass");
            if (plant != null)
                AddVegetation(tile, plant);
            return tile;
        }

        public static void AddVegetation(Tile tile, Plant plant)
        {
            tile.AddComponent(new PlantComponent(plant));
            tile.AddComponent(new ExtraAppearanceComponent(plant.GetSadGlyph()));
            if (plant?.Material?.ConfersTraits?.Contains(Trait.GrazerEatable) == true)
            {
                tile.AddComponent(FoodComponent.Herbivore);
                tile.Traits.Add(Trait.GrazerEatable);
            }
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
                MaterialId = "wood",
            };
            return tile;
        }

        public static Tile GenericTree(Point pos)
        {
            var tile = new Tile(MagiPalette.Wood, Color.Black, 'O', false, false, pos)
            {
                Name = "Tree",
                MaterialId = "wood",
            };
            return tile;
        }

        public static Tile GenericStoneWall()
        {
            return GenericStoneWall(Point.None);
        }

        internal static Tile GenericStoneWall(Point pos, bool useCachedMaterial = false)
        {
            return CreateTile(pos, TileType.Wall, "stone", useCacheMaterial: useCachedMaterial);
        }

        internal static Tile GenericStoneFloor()
        {
            return GenericStoneFloor(Point.None);
        }

        internal static Tile GenericStoneFloor(Point pos, bool useCachedMaterial = false)
        {
            return CreateTile(pos, TileType.Floor, "stone", useCacheMaterial: useCachedMaterial);
        }

        public static Tile GenericTreeTrunk(Point pos)
        {
            return CreateTile(pos, TileType.TreeTrunk, "wood");
        }

        public static Tile CreateTile(Point pos,
            TileType tileType,
            string? materialId = null,
            MaterialType typeToMake = MaterialType.None,
            bool useCacheMaterial = false)
        {
            Material material;
            if (useCacheMaterial && cachedMaterial?.Id.Equals(materialId) == true)
            {
                material = cachedMaterial;
            }
            else if (!string.IsNullOrEmpty(materialId))
            {
                material = DataManager.QueryMaterial(materialId);
            }
            else
            {
                material = DataManager.QueryMaterialWithType(typeToMake);
            }
            cachedMaterial = material;
            return CreateTile(pos, tileType, material);
        }

        private static Tile CreateTile(Point pos,
            TileType tileType,
            Material material)
        {
            var (foreground, background, glyph, isWalkable, isTransparent, name) = DetermineTileLookAndName(material, tileType);
            return new Tile(foreground, background, glyph, isWalkable, isTransparent, pos, name, material.Id);
        }

        public static void ResetCachedMaterial() => cachedMaterial = null;

        private static (Color, Color, char, bool, bool, string) DetermineTileLookAndName(Material? material, TileType tileType)
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

        public static Tile CreateTile(Point pos, TileType type, Trait trait)
        {
            Material mat = DataManager.QueryMaterialWithTrait(trait);
            return CreateTile(pos, type, mat);
        }
    }
}

using Arquimedes.Enumerators;
using GoRogue.Components;
using MagusEngine.Core.Entities.Base;
using MagusEngine.ECS;
using MagusEngine.Systems;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Core.MapStuff
{
    public class Tile : MagiGameObject
    {
        private Material _material;

        public ColoredGlyph Appearence { get; set; }
        public ColoredGlyph? LastSeenAppereance { get; set; }
        public int MoveTimeCost { get; set; } = 100;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Trait> Traits { get; set; } = [];

        public string MaterialId
        {
            get
            {
                return _material?.Id!;
            }

            set
            {
                _material = DataManager.QueryMaterial(value)!;
            }
        }
        public Material Material => _material;

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent,
            Point pos,
            IComponentCollection? collection = null) : base(pos,
                (int)MapLayer.TERRAIN,
                isWalkable, isTransparent,
                Locator.GetService<IDGenerator>() is not null ? Locator.GetService<IDGenerator>().UseID : null,
                collection)
        {
            Appearence = new ColoredGlyph(foreground, background, glyph);
            LastSeenAppereance = (ColoredGlyph)Appearence.Clone();
        }

        public Tile() : this(Color.BlueViolet, Color.Wheat, '@', true, true, Point.None)
        {
        }

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent,
            Point pos,
            string? name,
            string idMaterial,
            int moveTimeCost = 100) : this(foreground, background, glyph, isWalkable, isTransparent, pos)
        {
            MoveTimeCost = moveTimeCost;
            Name = name;
            MaterialId = idMaterial;
        }

        private Tile(Tile tile)
            : this(tile.Appearence.Foreground,
                tile.Appearence.Background,
                tile.Appearence.GlyphCharacter,
                tile.IsWalkable,
                tile.IsTransparent,
                tile.Position,
                tile.GoRogueComponents)
        {
            Traits = tile.Traits;
            Name = tile.Name;
            MoveTimeCost = tile.MoveTimeCost;
            MaterialId = tile.MaterialId;
        }

        public T? GetComponent<T>() where T : class
            => Locator.GetService<EntityRegistry>().GetComponent<T>(ID);

        public bool GetComponent<T>(out T? comp) where T : class
        {
            return Locator.GetService<EntityRegistry>().TryGetComponent(ID, out comp);
        }

        public bool HasComponent<TFind>() where TFind : class
        {
            return Locator.GetService<EntityRegistry>().Contains<TFind>(ID);
        }

        public void RemoveComponent<T>(T comp) where T : class
        {
            if (GoRogueComponents.Contains<T>())
                GoRogueComponents.Remove(comp);
            Locator.GetService<EntityRegistry>().RemoveComponent<T>(ID);
        }

        /// <summary>
        /// Copies the given tile and returns it, deep copy, but not copying the components
        /// </summary>
        /// <returns></returns>
        public Tile Copy()
        {
            return new Tile(this);
        }

        /// <summary>
        /// Copy the tile with the pos set to the given value.
        /// Won't copy the components right now
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Tile Copy(Point pos)
        {
            var tile = Copy();
            tile.Position = pos;

            return tile;
        }

        public void AddComponent<T>(T value, string? tag = null, bool addGoRogueComponents = false) where T : class
        {
            if (addGoRogueComponents)
                GoRogueComponents.Add(value, tag);
            Locator.GetService<EntityRegistry>()?.AddComponent(ID, value);
        }

        public static Attack? ReturnAttack()
        {
            return new()
            {
                AttackVerb = ["skid", "skids"],
                ContactArea = 1_000_000, // 1 cubic meters
                AttackAbility = AbilityCategory.None,
                DamageTypeId = "blunt",
            };
        }
    }
}

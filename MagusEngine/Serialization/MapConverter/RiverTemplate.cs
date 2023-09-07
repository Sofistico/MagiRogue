﻿using Arquimedes.Enumerators;
using MagusEngine.Core.WorldStuff;
using MagusEngine.ECS.Components.TilesComponents;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine.Serialization.MapConverter
{
    // TODO: REDO
    public class RiverJsonConverter : JsonConverter<River>
    {
        public override River? ReadJson(JsonReader reader,
            Type objectType, River? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<RiverTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer,
            River? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (RiverTemplate)value);
        }
    }

    public class RiverTemplate
    {
        public int RiverCount { get; }
        public int Length { get; set; }
        public int Id { get; }

        public List<Point> Tiles { get; set; }
        public int Intersections { get; set; }
        public float TurnCount { get; set; }
        public Direction CurrentDirection { get; set; }

        public RiverTemplate(int riverCount, int length, int id,
            int intersections,
            float turnCount, Direction currentDirection)
        {
            RiverCount = riverCount;
            Length = length;
            Id = id;
            //Tiles = new();
            //for (int i = 0; i < tiles.Count; i++)
            //{
            //    Tiles.Add(tiles[i].Position);
            //}
            Intersections = intersections;
            TurnCount = turnCount;
            CurrentDirection = currentDirection;
        }

        public RiverTemplate(int riverCount, int length, int id, List<Point> tiles,
            int intersections, float turnCount, Direction currentDirection)
        {
            RiverCount = riverCount;
            Length = length;
            Id = id;
            Tiles = tiles;
            Intersections = intersections;
            TurnCount = turnCount;
            CurrentDirection = currentDirection;
        }

        public RiverTemplate()
        {
            //empty one
        }

        public static implicit operator River(RiverTemplate template)
        {
            List<WorldTile> tiles = new List<WorldTile>();
            /*for (int i = 0; i < template.Tiles.Count; i++)
            {
                tiles.Add(template.Tiles[i]);
            }*/
            var river = new River(template.Id)
            {
                CurrentDirection = template.CurrentDirection,
                TurnCount = template.TurnCount,
                Tiles = tiles,
                Intersections = template.Intersections,
                Length = template.Length
            };

            return river;
        }

        public static implicit operator RiverTemplate(River river)
        {
            return new RiverTemplate(river.RiverCount,
                river.Length, river.Id, river.Intersections,
                river.TurnCount, river.CurrentDirection);
        }
    }
}
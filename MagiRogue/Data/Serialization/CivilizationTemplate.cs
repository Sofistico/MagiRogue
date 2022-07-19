using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data.Serialization
{
    public class CivilizationJsonConverter : JsonConverter<Civilization>
    {
        public override Civilization? ReadJson
            (JsonReader reader, Type objectType,
            Civilization? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<CivilizationTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer,
            Civilization? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (CivilizationTemplate)value);
        }
    }

    public class CivilizationTemplate
    {
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public CivilizationTendency Tendency { get; set; }
        public List<Point> Territory { get; set; }
        public List<Settlement> Settlements { get; set; }

        public CivilizationTemplate(string name,
            Race primaryRace,
            CivilizationTendency tendency,
            List<Point> territory)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Tendency = tendency;
            Territory = territory;
        }

        public CivilizationTemplate()
        {
            // empty
        }

        public static implicit operator Civilization(CivilizationTemplate template)
        {
            List<Point> territory = new List<SadRogue.Primitives.Point>();

            foreach (Point tile in template.Territory)
            {
                territory.Add(tile);
            }

            Civilization civ = new Civilization(template.Name,
                template.PrimaryRace, template.Tendency)
            {
                Territory = territory,
                Settlements = template.Settlements,
            };

            return civ;
        }

        public static implicit operator CivilizationTemplate(Civilization civ)
        {
            List<Point> basicTiles = new List<Point>();

            for (int i = 0; i < civ.Territory.Count; i++)
            {
                basicTiles.Add(civ.Territory[i]);
            }

            CivilizationTemplate template = new CivilizationTemplate(civ.Name,
                civ.PrimaryRace,
                civ.Tendency,
                basicTiles)
            {
                Settlements = civ.Settlements,
            };

            return template;
        }
    }
}
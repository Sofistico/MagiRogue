using MagiRogue.Entities;
using MagiRogue.System.Civ;
using MagiRogue.System.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int MilitaryStrenght { get; set; }
        public int MagicStrenght { get; set; }
        public int Population { get; set; }
        public CivilizationTendency Tendency { get; set; }
        public float MundaneResources { get; set; }
        public float MagicalResources { get; set; }
        public List<ItemTemplate> Nodes { get; set; }
        public List<BasicTile> Territory { get; set; }

        public CivilizationTemplate(string name,
            Race primaryRace,
            int militaryStrenght,
            int magicStrenght,
            int population,
            CivilizationTendency tendency,
            float mundaneResources,
            float magicalResources,
            List<ItemTemplate> nodes,
            List<BasicTile> territory)
        {
            Name = name;
            PrimaryRace = primaryRace;
            MilitaryStrenght = militaryStrenght;
            MagicStrenght = magicStrenght;
            Population = population;
            Tendency = tendency;
            MundaneResources = mundaneResources;
            MagicalResources = magicalResources;
            Nodes = nodes;
            Territory = territory;
        }

        public CivilizationTemplate()
        {
            // empty
        }

        public static implicit operator Civilization(CivilizationTemplate template)
        {
            var territory = new Territory();

            if (template.Territory.Any(e => e is not null))
            {
                foreach (WorldTile tile in template.Territory)
                {
                    if (!tile.Collidable)
                        territory.AddWaterLand(tile);
                    else
                        territory.AddLand(tile);
                }
            }

            Civilization civ = new Civilization(template.Name,
                template.PrimaryRace, template.Population, template.Tendency)
            {
                MagicalResources = template.MagicalResources,
                MagicStrenght = template.MagicStrenght,
                MilitaryStrenght = template.MilitaryStrenght,
                MundaneResources = template.MundaneResources,
                Nodes = template.Nodes,
                Territory = territory
            };

            return civ;
        }

        public static implicit operator CivilizationTemplate(Civilization civ)
        {
            List<BasicTile> basicTiles = new List<BasicTile>();

            for (int i = 0; i < civ.Territory.OwnedWater.WorldTiles.Count; i++)
            {
                basicTiles.Add(civ.Territory.OwnedWater.WorldTiles[i]);
            }
            for (int i = 0; i < civ.Territory.OwnedLand.WorldTiles.Count; i++)
            {
                basicTiles.Add(civ.Territory.OwnedLand.WorldTiles[i]);
            }

            CivilizationTemplate template = new CivilizationTemplate(civ.Name,
                civ.PrimaryRace,
                civ.MilitaryStrenght,
                civ.MagicStrenght,
                civ.Population,
                civ.Tendency,
                civ.MundaneResources,
                civ.MagicalResources,
                civ.Nodes,
                basicTiles);

            return template;
        }
    }
}
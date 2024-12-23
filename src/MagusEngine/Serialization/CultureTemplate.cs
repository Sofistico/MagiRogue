﻿using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Serialization
{
    public sealed class CultureTemplate : IJsonKey
    {
        public string Id { get; set; }
        public string[] Races { get; set; }
        public string StartBiome { get; set; }
        public string LanguageSpoken { get; set; }
        public string[] PossibleTendencies { get; set; }
        public List<SiteType> PossibleSites { get; set; }
        public List<WorldConstruction> PossibleWorldConstructions { get; set; }
        public List<Noble> Nobles { get; set; }

        public CultureTemplate()
        {
            // empty
        }

        public Civilization ConvertToCivilization()
        {
            var rngName = RandomNames.RandomNamesFromLanguage(LanguageSpoken)?.Replace(" ", "");
            Race primaryRace = DataManager.QueryRaceInData(Races[0]);
            CivilizationTendency tendency = DetermineCivTendency();
            Civilization civ = new Civilization(rngName, primaryRace, tendency)
            {
                NoblesPosition = Nobles,
                PossibleWorldConstruction = PossibleWorldConstructions,
                PossibleSites = PossibleSites,
                TemplateId = Id,
                LanguageId = LanguageSpoken,
            };
            if (Races.Length > 0)
            {
                List<string> otherRaces = new(Races);
                otherRaces.RemoveAt(0);
                civ.OtherRaces = otherRaces;
            }
            return civ;
        }

        private CivilizationTendency DetermineCivTendency()
        {
            List<CivilizationTendency> tendencies;
            if (PossibleTendencies.Contains("Any"))
            {
                tendencies = Enum.GetValues<CivilizationTendency>().ToList();
                return tendencies.GetRandomItemFromList();
            }

            tendencies = new List<CivilizationTendency>();
            foreach (var str in PossibleTendencies)
            {
                if (Enum.TryParse(str, out CivilizationTendency res))
                {
                    tendencies.Add(res);
                }
            }
            return tendencies.GetRandomItemFromList();
        }
    }
}
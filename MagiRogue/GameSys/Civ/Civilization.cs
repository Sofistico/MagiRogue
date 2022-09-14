using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static SadConsole.Readers.Playscii;

namespace MagiRogue.GameSys.Civ
{
    //[JsonConverter(typeof(CivilizationJsonConverter))]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public sealed class Civilization
    {
        public int Id { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public List<string> OtherRaces { get; set; }
        public int TotalPopulation { get => Sites.Select(i => i.Population).Sum(); }
        public CivilizationTendency Tendency { get; set; }
        public List<Point> Territory { get; set; }
        public List<Site> Sites { get; set; }
        public int Wealth { get; set; }
        public List<CivRelation> Relations { get; set; }
        public string LanguageId { get; set; }
        public List<Noble> NoblesPosition { get; set; }

        public CivRelation this[int otherCivId]
        {
            get => Relations.FirstOrDefault(i => i.CivRelatedId == otherCivId);

            set
            {
                var relation = Relations.FirstOrDefault(i => i.CivRelatedId == otherCivId);
                relation = value;
            }
        }
        public List<HistoricalFigure> ImportantPeople { get; set; }
        public bool Dead { get; set; }
        public List<WorldConstruction> PossibleWorldConstruction { get; set; }
        public List<SiteType> PossibleSites { get; set; }
        public Dictionary<string, int> TrackAmountOfNobles { get; set; }

        [JsonConstructor]
        public Civilization(string name, Race primaryRace, CivilizationTendency tendency)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Tendency = tendency;
            Territory = new();
            Sites = new List<Site>();
            ImportantPeople = new();
            Relations = new();
            Id = GameLoop.GetCivId();
        }

        public void AddSiteToCiv(Site site)
        {
            Sites.Add(site);
            site.CivOwnerIfAny = Id;
            Territory.Add(site.WorldPos);
            Wealth += site.MundaneResources;
        }

        public void SetupInitialHistoricalFigures()
        {
            // 10% of the population is in anyway important... sadly
            int nmbrOfImportant = (int)(TotalPopulation * 0.1);
            int numberOfNobles = (int)(nmbrOfImportant * 0.1);
            bool rulerChoosen = false;
            for (int i = 0; i < nmbrOfImportant; i++)
            {
                // creating actor
                string name = Utils.RandomNames.RandomNamesFromLanguage(GetLanguage());
                Sex sex = PrimaryRace.ReturnRandomSex();
                List<Legend> legends = new List<Legend>();
                int age = PrimaryRace.GetAgeFromAgeGroup(AgeGroup.Adult);

                // first legend
                StringBuilder initialLegend = new StringBuilder("In a time before time, ");
                initialLegend.Append($"{name} was created looking like a {sex} of the {PrimaryRace.RaceName} ");
                initialLegend.Append($"with {age} as a member of {Name}");
                legends.Add(new Legend(initialLegend.ToString(), -1));
                HistoricalFigure figure = new HistoricalFigure(name, sex, legends,
                    age - 0, null, true, PrimaryRace.Description, PrimaryRace.Id)
                {
                    CurrentAge = age
                };
                figure.GenerateRandomPersonality();
                figure.GenerateRandomSkills();
                figure.DefineProfession();

                // add to civ
                figure.AddNewRelationToCiv(Id, RelationType.Member);
                ImportantPeople.Add(figure);
            }

            List<HistoricalFigure> nobles = ImportantPeople.ShuffleAlgorithmAndTakeN(numberOfNobles);
            // in here we can already say that it's not in the time before time period
            int year = 1;
            var noblesPos = NoblesPosition.Where(
                    i => !i.Responsabilities.Contains(Responsability.Rule))
                    .ToList();
            var rulerPos = NoblesPosition.FirstOrDefault(
                        i => i.Responsabilities.Contains(Responsability.Rule));

            for (int i = 0; i < numberOfNobles; i++)
            {
                bool isRuler = Mrn.OneIn(numberOfNobles);
                if (isRuler && !rulerChoosen)
                {
                    rulerChoosen = true;
                    AppointNewNoble(rulerPos, nobles[i], year);
                }

                Noble noble = noblesPos.GetRandomItemFromList();
                AppointNewNoble(noble, nobles[i], year);
            }

            if (!rulerChoosen)
            {
                // choose the ruler here if it hasn't appereade till now!
                AppointNewNoble(rulerPos, nobles.GetRandomItemFromList(), year);
                rulerChoosen = true;
            }
        }

        public void AppointNewNoble(Noble noble,
            HistoricalFigure figureToAdd,
            int yearAdded,
            string? whyItHappenead = null)
        {
            TrackAmountOfNobles ??= new Dictionary<string, int>();

            figureToAdd.AddNewNoblePos(noble,
                yearAdded,
                this);

            if (!TrackAmountOfNobles.TryAdd(noble.Id, 0))
            {
                if (TrackAmountOfNobles[noble.Id] >= noble.MaxAmmount)
                    return;
                TrackAmountOfNobles[noble.Id]++;
                StringBuilder str = new StringBuilder($"{figureToAdd.Name} ascended on the position as {noble.Name}");

                if (!string.IsNullOrEmpty(whyItHappenead))
                {
                    str.Append(' ').Append(whyItHappenead);
                }

                figureToAdd.AddLegend(str.ToString(), yearAdded);
            }
        }

        public bool RemoveNoble(Noble noble,
            HistoricalFigure figure,
            int yearWhenItHappenad,
            string? whyLostIt = null)
        {
            figure.NobleTitles.Remove(noble);

            if (TrackAmountOfNobles.ContainsKey(noble.Id))
            {
                TrackAmountOfNobles[noble.Id]--;
                if (TrackAmountOfNobles[noble.Id] <= 0)
                {
                    TrackAmountOfNobles.Remove(noble.Id);
                    StringBuilder str = new StringBuilder($"{figure.Name} lost it's position as {noble.Name}");

                    if (!string.IsNullOrEmpty(whyLostIt))
                    {
                        str.Append(' ').Append(whyLostIt);
                    }

                    figure.AddLegend(str.ToString(), yearWhenItHappenad);
                }
                return true;
            }
            return false;
        }

        public List<WorldTile> ReturnAllTerritories(Map map)
        {
            List<WorldTile> tiles = new();
            int count = Territory.Count;

            for (int i = 0; i < count; i++)
            {
                tiles.Add(map.GetTileAt<WorldTile>(Territory[i]));
            }

            return tiles;
        }

        public List<WorldTile> ReturnAllLandTerritory(Map map)
        {
            var list = ReturnAllTerritories(map);
            var landList = new List<WorldTile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collidable)
                    landList.Add(list[i]);
            }

            return landList;
        }

        public List<WorldTile> ReturnAllWaterTerritoey(Map map)
        {
            var list = ReturnAllTerritories(map);
            var waterList = new List<WorldTile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Collidable)
                    waterList.Add(list[i]);
            }

            return waterList;
        }

        public Site GetSite(WorldTile worldTile)
        {
            return Sites.FirstOrDefault(o => o.WorldPos == worldTile.Position);
        }

        public void AddCivToRelations(Civilization civ, RelationType relationType)
        {
            if (!Relations.Any(i => i.CivRelatedId.Equals(civ.Id) && i.Relation == relationType))
                Relations.Add(new CivRelation(Id, civ.Id, relationType));
        }

        public void CheckIfCivIsDead()
        {
            if (Dead)
                return;

            if (TotalPopulation <= 0)
            {
                Dead = true;
                return;
            }
        }

        public Language GetLanguage()
            => Data.DataManager.QueryLanguageInData(LanguageId);

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Name} - {PrimaryRace}";
        }

        public string RandomSiteFromLanguageName()
        {
            return RandomNames.RandomNamesFromLanguage(LanguageId).Replace(" ", "");
        }

        public (Noble, HistoricalFigure?) GetRulerNoblePosition()
        {
            Noble n = NoblesPosition.First(i => i.Responsabilities.Contains(Responsability.Rule));
            HistoricalFigure? figure = ImportantPeople.FirstOrDefault(i => i.NobleTitles.Contains(n));

            return (n, figure);
        }
    }
}
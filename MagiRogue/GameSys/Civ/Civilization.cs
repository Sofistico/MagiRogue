using GoRogue;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MagiRogue.GameSys.Civ
{
    //[JsonConverter(typeof(CivilizationJsonConverter))]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public sealed class Civilization
    {
        #region CivDefinitionsFromTemplate

        public int Id { get; set; }
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public List<string> OtherRaces { get; set; }
        public int TotalPopulation { get => Sites.Select(i => i.Population.Sum(i => i.TotalPopulation)).Sum(); }
        public CivilizationTendency Tendency { get; set; }
        public List<Point> Territory { get; set; }
        public List<Site> Sites { get; set; }
        public int Wealth { get; set; }
        public List<CivRelation> Relations { get; set; }
        public string LanguageId { get; set; }
        public List<Noble> NoblesPosition { get; set; }

        #endregion CivDefinitionsFromTemplate

        #region Information

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
        public List<int> CivsTradingWith { get; private set; }

        //public List<Discovery> Discoveries { get; set; }
        public List<Legend> Legends { get; set; }

        #endregion Information

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
            CivsTradingWith = new();
            Legends = new();
        }

        public void AddSiteToCiv(Site site)
        {
            Sites.Add(site);
            site.CivOwnerIfAny = Id;
            Territory.Add(site.WorldPos);
            Wealth += site.MundaneResources;
        }

        public List<HistoricalFigure> SetupInitialHistoricalFigures()
        {
            // 10% of the population is in anyway important... sadly
            int nmbrOfImportant = (int)(TotalPopulation * 0.08);
            int numberOfNobles = (int)(nmbrOfImportant * 0.1);
            bool rulerChoosen = false;
            for (int i = 0; i < nmbrOfImportant; i++)
            {
                var hf = CreateNewHfMemberFromPop(0);
                StringBuilder initialLegend = new StringBuilder("In a time before time, ");
                initialLegend.Append($"{hf.Name} was created looking like a {hf.HFGender} of the {hf.GetRaceName()} ");
                initialLegend.Append($"with {hf.Body.Anatomy.CurrentAge} as a member of {Name}");
                hf.Legends.Add(new Legend(initialLegend.ToString(), -1));

                ImportantPeople.Add(hf);
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

            return ImportantPeople;
        }

        public HistoricalFigure CreateNewHfMemberFromPop(int yearBorn)
        {
            // creating actor
            string name = Utils.RandomNames.RandomNamesFromLanguage(GetLanguage());
            Sex sex = PrimaryRace.ReturnRandomSex();
            List<Legend> legends = new List<Legend>();
            int age = PrimaryRace.GetAgeFromAgeGroup(AgeGroup.Adult);

            // first legend
            HistoricalFigure figure = new HistoricalFigure(name, sex, legends,
                age - yearBorn, null, true, PrimaryRace.Description, PrimaryRace.Id);
            figure.GenerateRandomPersonality();
            figure.GenerateRandomSkills();
            figure.DefineProfession();

            // add to civ
            figure.AddNewRelationToCiv(Id, RelationType.Member);

            // one in 20 will be an wizard
            if (Mrn.OneIn(20))
            {
                figure.MythWho = MythWho.Wizard;
                figure.AddNewFlag(SpecialFlag.MagicUser);
            }
            figure.SetStandardRaceFlags();

            return figure;
        }

        public HistoricalFigure CreateNewHfMemberFromBirth(int yearBorn, FamilyLink familyLink)
        {
            // creating actor
            string name = Utils.RandomNames.RandomNamesFromLanguage(GetLanguage());
            Sex sex = PrimaryRace.ReturnRandomSex();
            List<Legend> legends = new List<Legend>();
            int age = 0;

            // first legend
            HistoricalFigure figure = new HistoricalFigure(name, sex, legends,
                age - yearBorn, null, true, PrimaryRace.Description, PrimaryRace.Id);
            figure.GenerateRandomPersonality();
            figure.GenerateRandomSkills();
            figure.DefineProfession();

            // add to civ
            figure.AddNewRelationToCiv(Id, RelationType.Member);

            // one in 20 will be an wizard
            if (Mrn.OneIn(20))
            {
                figure.MythWho = MythWho.Wizard;
                figure.AddNewFlag(SpecialFlag.MagicUser);
            }
            figure.SetStandardRaceFlags();
            figure.FamilyLink = familyLink;

            return figure;
        }

        public void AppointNewNoble(Noble noble,
            HistoricalFigure figureToAdd,
            int yearAdded,
            string? whyItHappenead = null)
        {
            TrackAmountOfNobles ??= new Dictionary<string, int>();
            bool hasAllRequiredsForPos = true;
            if (noble.RequiredForPos is not null)
            {
                foreach (var flag in noble.RequiredForPos)
                {
                    if (Enum.TryParse<SpecialFlag>(flag, out var result))
                    {
                        if (figureToAdd.SpecialFlags.Contains(result))
                            continue;
                        else
                            hasAllRequiredsForPos = false;
                    }
                }
            }

            if (!hasAllRequiredsForPos)
                return;

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

        public void AppointNewNobleFromPops(Noble noble,
            int yearAdded,
            string? whyItHappenead = null)
        {
            if (TotalPopulation > 0 && TotalPopulation > ImportantPeople.Count)
            {
                var hf = CreateNewHfMemberFromPop(yearAdded);

                ImportantPeople.Add(hf);

                AppointNewNoble(noble, hf, yearAdded, whyItHappenead);
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

        public void AppointNobleToAdministerSite(Site site, int currentYear)
        {
            // check this later for a better solution
            List<Noble> noblesPos = GetNobleWithSpecificResponse(Responsability.Administrate);
            if (noblesPos.Count > 0)
            {
                Noble noble = noblesPos.GetRandomItemFromList();
                var hfs = GetAllHfsNoblesWithSpecificTitle(noble);
                HistoricalFigure hf;
                // need to check this later!
                if (hfs.Count > 0)
                {
                    hf = hfs.GetRandomItemFromList();
                }
                else
                {
                    bool rng = Mrn.OneIn(2);
                    if (rng)
                    {
                        // pick a random dude from the pops
                        hf = CreateNewHfMemberFromPop(currentYear);
                    }
                    else
                        // else elevate someone else to the position
                        hf = GetAllHfsNotNobles().GetRandomItemFromList();
                    AppointNewNoble(noble, hf, currentYear);
                }

                site.AddHfAsSiteLeader(hf, currentYear);
            }
            else
            {
                site.AddHfAsSiteLeader(GetRulerNoblePosition().Item2, currentYear);
            }
        }

        public List<Noble> GetNobleWithSpecificResponse(Responsability responsability)
            => NoblesPosition.Where(i => i.Responsabilities.Contains(responsability)).ToList();

        public List<HistoricalFigure> GetAllHfsNobles()
            => ImportantPeople.Where(i => i.NobleTitles.Count > 0).ToList();

        public List<HistoricalFigure> GetAllHfsNoblesWithSpecificTitle(Noble noble)
            => ImportantPeople.Where(i => i.NobleTitles.Contains(noble)).ToList();

        public List<HistoricalFigure> GetAllHfsNotNobles()
            => ImportantPeople.Where(i => i.NobleTitles.Count <= 0).ToList();

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

        public bool CheckIfCivIsDead()
        {
            if (Dead)
                return true;

            if (TotalPopulation <= 0)
            {
                Dead = true;
                Territory.Clear();
                foreach (var site in Sites)
                {
                    site.Dead = true;
                }
                Relations.Clear();

                return true;
            }
            return false;
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

        public void AddToTradingList(Civilization nextCiv)
        {
            if (!CivsTradingWith.Contains(nextCiv.Id))
                CivsTradingWith.Add(nextCiv.Id);
        }

        public void TradeWithOtherCiv(Civilization[] otherCivs)
        {
            foreach (var civId in CivsTradingWith)
            {
                Civilization civ = otherCivs.FirstOrDefault(i => i.Id == civId);
                if (civ is not null && Sites.Any(i => i.Famine))
                {
                    var siteWithMostFood = civ.Sites.MaxBy(i => i.FoodQuantity);
                    int resourceSpent = 100;
                    BuyFoodFromOtherCivSite(siteWithMostFood, resourceSpent);
                }
                int wealthGeneratedByTrade = GetPotentialWealthGeneratedFromSimpleTrade(civ);
            }
        }

        private int GetPotentialWealthGeneratedFromSimpleTrade(Civilization civ)
        {
            if (civ is null)
                return 0;
            return (civ.Sites.Sum(i => i.MundaneResources) / (civ.TotalPopulation))
                * ((GetRulerNoblePosition().Item2.Mind.GetAbility(AbilityName.Negotiator) + 1 / 100));
        }

        private void BuyFoodFromOtherCivSite(Site? siteWithMostFood, int resourceSpent)
        {
            siteWithMostFood.FoodQuantity -= resourceSpent;
            siteWithMostFood.MundaneResources += resourceSpent;
            Wealth -= resourceSpent;
        }

        public IEnumerable<Discovery> ListAllKnowDiscoveries()
        {
            var siteCount = Sites.Count;
            for (int i = 0; i < siteCount; i++)
            {
                Site site = Sites[i];
                int discoveryCount = site.DiscoveriesKnow.Count;
                for (int x = 0; x < discoveryCount; x++)
                {
                    yield return site.DiscoveriesKnow[x];
                }
            }
        }
    }
}
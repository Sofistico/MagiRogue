using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Core.WorldStuff.TechRes;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Generators;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.Civ
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
            get => Relations.Find(i => i.CivRelatedId == otherCivId);

            set
            {
                var relation = Relations.Find(i => i.CivRelatedId == otherCivId);
                relation = value;
            }
        }
        public List<HistoricalFigure> ImportantPeople { get; set; }
        public bool Dead { get; set; }
        public List<WorldConstruction> PossibleWorldConstruction { get; set; }
        public List<SiteType> PossibleSites { get; set; }
        public Dictionary<string, int> TrackAmountOfNobles { get; set; }
        public List<int> CivsTradingWith { get; private set; }
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
            Id = SequentialIdGenerator.CivId;
            CivsTradingWith = new();
            Legends = new();
        }

        public void AddSiteToCiv(Site site)
        {
            Sites.Add(site);
            site.CivOwnerIfAny = Id;
            AddToTerritory(site.WorldPos);
            Wealth += site.MundaneResources;
        }

        public List<HistoricalFigure> SetupInitialHistoricalFigures()
        {
            // 10% of the population is in anyway important... sadly
            int nmbrOfImportant = (int)(TotalPopulation * 0.08);
            int numberOfNobles = (int)(nmbrOfImportant * 0.1);
            numberOfNobles = numberOfNobles <= 0 ? 1 : numberOfNobles;
            bool rulerChoosen = false;
            for (int i = 0; i < nmbrOfImportant; i++)
            {
                var hf = CreateNewHfMemberFromPop(0);
                StringBuilder initialLegend = new("In a time before time, ");
                initialLegend.Append(hf.Name).Append(" was created looking like a ").Append(hf.HFGender).Append(" of the ").Append(hf.GetRaceNamePlural()).Append(" race ");
                initialLegend.Append("with ").Append(hf.Body.Anatomy.CurrentAge).Append(" years as a member of ").Append(Name);
                hf.Legends.Add(new Legend(initialLegend.ToString(), -1));

                ImportantPeople.Add(hf);
            }

            List<HistoricalFigure> nobles = ImportantPeople.ShuffleAlgorithmAndTakeN(numberOfNobles);
            // in here we can already say that it's not in the time before time period
            const int year = 1;
            var noblesPos = NoblesPosition.Where(
                    i => !i.Responsabilities.Contains(Responsability.Rule))
                    .ToList();
            var rulerPos = NoblesPosition.Find(
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

        public HistoricalFigure CreateNewHfMemberFromPop(int currentYear)
        {
            // creating actor
            BasicHFSetup(out string name, out Sex sex, out List<Legend> legends, out Race figureRace);
            int age = PrimaryRace.GetAgeFromAgeGroup(AgeGroup.Adult);

            // first legend
            HistoricalFigure figure = CreateHfFigure(currentYear, name, sex, legends, figureRace, age);
            FigureSetupPosCtor(figureRace, figure);

            return figure;
        }

        public HistoricalFigure CreateNewHfMemberFromBirth(int yearBorn, FamilyLink familyLink)
        {
            // creating actor
            BasicHFSetup(out string name, out Sex sex, out List<Legend> legends, out Race figureRace);
            HistoricalFigure figure = CreateHfFigure(yearBorn, name, sex, legends, figureRace);
            FigureSetupPosCtor(figureRace, figure);
            figure.FamilyLink = familyLink;

            return figure;
        }

        private static HistoricalFigure CreateHfFigure(int yearBorn,
            string name,
            Sex sex,
            List<Legend> legends,
            Race figureRace,
            int age = 0)
        {
            // first legend
            HistoricalFigure figure = new HistoricalFigure(name, sex, legends,
                age - yearBorn, null, true, figureRace.Description, figureRace.Id);
            figure.Body.Anatomy.CurrentAge = age;
            return figure;
        }

        private void FigureSetupPosCtor(Race figureRace, HistoricalFigure figure)
        {
            figure.SetRace(figureRace);
            figure.GenerateRandomPersonality();
            figure.GenerateRandomStatsSpread();
            if (!figure.CheckIfIsChildOrBabyForRace())
            {
                figure.GenerateRandomSkills();
                figure.DefineProfession();
            }

            // add to civ
            figure.AddNewRelationToCiv(Id, RelationType.Member);

            CheckIfGeneratedFigureIsWizard(figureRace, figure);
        }

        private void BasicHFSetup(out string name, out Sex sex, out List<Legend> legends, out Race figureRace)
        {
            name = Utils.RandomNames.RandomNamesFromLanguage(GetLanguage());
            sex = PrimaryRace.ReturnRandomSex();
            legends = new List<Legend>();
            figureRace = PrimaryRace;
        }

        private void CheckIfGeneratedFigureIsWizard(Race figureRace, HistoricalFigure figure)
        {
            // if the creature has more than the average mana for it's race, then the figure will be
            // a wizard! plus a one in 10 chance of it becoming a wizard, to simulate that it will
            // still need to learn and have the willingness to try to learn! with studious civs
            // making it a 1 in 5 chance that the figure will be a wizard
            if (figure.HasPotentialToBeAWizard())
            {
                bool chance;
                if (Tendency is CivilizationTendency.Studious)
                    chance = Mrn.OneIn(5);
                else
                    chance = Mrn.OneIn(10);
                if (chance)
                {
                    figure.AddNewFlag(SpecialFlag.MagicUser);
                    figure.Mind.AddAbilitiesToDictionary(AbilityHelper.GetMagicalAbilities(), true);
                }
            }
        }

        public void AppointNewNoble(Noble? noble,
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
            if (figureToAdd is null)
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

        public void AddLegend(string legend, int yearWhen)
        {
            StringBuilder str = new StringBuilder($"In the year of {yearWhen}, ");
            str.Append(legend);
            Legend newLegend = new Legend(str.ToString(), yearWhen);
            Legends.Add(newLegend);
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

        public List<Tile> ReturnAllTerritories(MagiMap map)
        {
            List<Tile> tiles = new();
            int count = Territory.Count;

            for (int i = 0; i < count; i++)
            {
                tiles.Add(map.GetTileAt<WorldTile>(Territory[i]));
            }

            return tiles;
        }

        public List<Tile> ReturnAllLandTerritory(MagiMap map)
        {
            var list = ReturnAllTerritories(map);
            var landList = new List<Tile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsWalkable)
                    landList.Add(list[i]);
            }

            return landList;
        }

        public List<Tile> ReturnAllWaterTerritoey(MagiMap map)
        {
            var list = ReturnAllTerritories(map);
            var waterList = new List<Tile>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsWalkable)
                    waterList.Add(list[i]);
            }

            return waterList;
        }

        public Site? GetSite(Tile worldTile)
        {
            return Sites.Find(o => o.WorldPos == worldTile.Position);
        }

        public void AddCivToRelations(Civilization civ, RelationType relationType)
        {
            if (!Relations.Any(i => i.CivRelatedId.Equals(civ.Id) && i.Relation == relationType))
            {
                Relations.Add(new CivRelation(Id, civ.Id, relationType));
            }
            else
            {
                var rel = Relations.Find(i => i.CivRelatedId == civ.Id);
                if (rel is not null)
                {
                    rel.Relation = relationType;
                }
            }
        }

        public bool CheckIfCivIsDead()
        {
            if (Dead)
                return true;

            if (TotalPopulation <= 0)
            {
                Dead = true;
                Territory.Clear();
                Relations.Clear();

                return true;
            }
            if (Sites.All(i => i.Dead))
                Dead = true;

            return false;
        }

        public Language GetLanguage()
            => DataManager.QueryLanguageInData(LanguageId);

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
            HistoricalFigure? figure = ImportantPeople.Find(i => i.NobleTitles.Contains(n));

            return (n, figure);
        }

        public void AddToTradingList(Civilization nextCiv)
        {
            if (!CivsTradeContains(nextCiv))
                CivsTradingWith.Add(nextCiv.Id);
        }

        public bool CivsTradeContains(Civilization civ)
        {
            return CivsTradingWith.Contains(civ.Id);
        }

        public void TradeWithOtherCiv(Civilization[] otherCivs)
        {
            foreach (var civId in CivsTradingWith)
            {
                Civilization? civ = Array.Find(otherCivs, i => i.Id == civId);
                if (civ is not null && Sites.Any(i => i.Famine))
                {
                    var siteWithMostFood = civ.Sites.MaxBy(i => i.FoodQuantity);
                    const int resourceSpent = 350;
                    BuyFoodFromOtherCivSite(siteWithMostFood, resourceSpent);
                }
                int wealthGeneratedByTrade = GetPotentialWealthGeneratedFromSimpleTrade(civ);
            }
        }

        private int GetPotentialWealthGeneratedFromSimpleTrade(Civilization? civ)
        {
            if (civ is null)
                return 0;
            if (civ.CheckIfCivIsDead())
                return 0;
            var civSum = civ.Sites.Where(i => !i.Dead).Sum(i => i.MundaneResources) % civ.TotalPopulation;
            var hf = GetRulerNoblePosition().Item2;
            double nobleInfluence = 1;
            if (hf is not null)
                nobleInfluence = hf.Mind.GetAbility(AbilityName.Negotiator) + 1 / 100;
            return (int)(civSum * nobleInfluence);
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

        public void AddToTerritory(Point direction)
        {
            if (Territory.Contains(direction))
                return;
            Territory.Add(direction);
        }

        public bool CheckIfRulerIsDeadAndReplace(int year)
        {
            var (noble, hf) = GetRulerNoblePosition();
            if (hf is null)
            {
                var newHf = CreateNewHfMemberFromPop(year);
                AppointNewNoble(noble, newHf, year);
                return true;
            }
            if (!hf.IsAlive)
            {
                if (noble.Succession?.Type is SucessionType.Heir)
                {
                    var inheirt = hf.GetChildrenIfAny();
                    inheirt ??= CreateNewHfMemberFromPop(year);
                    AppointNewNoble(noble, inheirt, year, $" inheirted from {hf.Name}");
                    return true;
                }
                else
                {
                    var rngGuy = ImportantPeople.GetRandomItemFromList();
                    AppointNewNoble(noble, rngGuy, year, $" was appointed by {rngGuy.PronoumPossesive()} group as {noble.Name}");
                    return true;
                }
            }
            return false;
        }
    }
}
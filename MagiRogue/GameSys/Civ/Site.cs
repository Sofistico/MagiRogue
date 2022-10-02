using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using MagiRogue.Utils;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Planet.History;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Planet.TechRes;

namespace MagiRogue.GameSys.Civ
{
    public sealed class Site
    {
        #region props

        public int Id { get; set; }
        public Point WorldPos { get; set; }
        public SiteType SiteType { get; set; }
        public string Name { get; set; }
        public SiteSize Size { get; set; }
        public int MilitaryStrenght { get; set; }
        public int MagicStrenght { get; set; }
        public int Population { get; set; }
        public int MundaneResources { get; set; }
        public int FoodQuantity { get; set; }
        public int MagicalResources { get; set; }
        public List<Room> Buildings { get; set; } = new();
        public bool Dead { get; set; }
        public int? CivOwnerIfAny { get; set; }
        public bool Famine { get; set; }
        public List<Road> Roads { get; set; } = new();
        public HistoricalFigure SiteLeader { get; set; }
        public List<Legend> SiteLegends { get; set; } = new();
        public List<Discovery> DiscoveriesKnow { get; set; } = new();
        public bool FitRoomsCloseTogether { get; set; }

        #endregion props

        public Site()
        {
            SetId();
        }

        public Site(Point pos, string name, int totalPopulation, int? civOwnerIfAny = null)
        {
            WorldPos = pos;
            Name = name;
            Population = totalPopulation;
            CivOwnerIfAny = civOwnerIfAny;
            SetId();
        }

        public void DefineSiteSize()
        {
            int usableResources = 0;
            try
            {
                usableResources = Population / MundaneResources;
            }
            catch (DivideByZeroException)
            {
                Size = SiteSize.None;
            }
            if (usableResources > 100 && usableResources < 1000)
            {
                Size = SiteSize.Small;
            }
            if (usableResources > 1000 && usableResources < 10000)
            {
                Size = SiteSize.Medium;
            }
            if (usableResources > 10000)
            {
                Size = SiteSize.Large;
            }
        }

        public void GenerateMundaneResources()
        {
            if (Population <= 0)
            {
                MundaneResources = 0;
                Dead = true;
                return;
            }
            foreach (var room in Buildings)
            {
                switch (room.Tag)
                {
                    case RoomTag.Inn:
                        MundaneResources += 10;
                        break;

                    case RoomTag.Blacksmith:
                        MundaneResources += 50;
                        break;

                    case RoomTag.Clothier:
                        MundaneResources += 35;
                        break;

                    case RoomTag.Alchemist:
                        MundaneResources += 20;
                        MagicalResources += 15;
                        break;

                    case RoomTag.Hovel:
                        MundaneResources += 15;
                        break;

                    case RoomTag.Abandoned:
                        MundaneResources -= 20;
                        break;

                    case RoomTag.House:
                        MundaneResources -= 10;
                        break;

                    case RoomTag.Throne:
                        MundaneResources -= 50;
                        break;

                    case RoomTag.Kitchen:
                        MundaneResources -= 25;
                        break;

                    case RoomTag.GenericWorkshop:
                        MundaneResources += 10;
                        break;

                    case RoomTag.Dinner:
                        MundaneResources -= 10;
                        break;

                    case RoomTag.DungeonKeeper:
                        MundaneResources += 10;
                        MagicalResources -= 10;
                        break;

                    case RoomTag.Farm:
                        MundaneResources += 10;
                        FoodQuantity += 50;
                        break;

                    default:
                        break;
                }

                if (MundaneResources <= 0)
                    room.Tag = RoomTag.Abandoned;
                if (FoodQuantity >= 0)
                    Famine = false;
            }
            // if the resources got big enough, update!
            DefineSiteSize();
        }

        private void SetId()
            => Id = SequentialIdGenerator.SiteId;

        public void CreateNewBuildings()
        {
            if (SiteType is SiteType.City)
            {
                CityBuildings();
            }
            if (SiteType is SiteType.Tower)
            {
                TowerBuildings();
            }
        }

        private void TowerBuildings()
        {
            NewHousesForSite();

            int numberOfNewTowerRooms = MagicalResources % 10;
            RoomTag[] tags = new RoomTag[]
            {
                RoomTag.Blacksmith,
                RoomTag.Temple,
                RoomTag.Alchemist,
                RoomTag.Farm,
                RoomTag.ReserachRoom,
                RoomTag.EnchantingRoom,
            };
            for (int i = 0; i < numberOfNewTowerRooms; i++)
            {
                Room business = new Room(tags.GetRandomItemFromList());
                Buildings.Add(business);
            }
            FitRoomsCloseTogether = true;
        }

        private void NewHousesForSite()
        {
            int numberOfNewHouses = Population % 10;
            for (int i = 0; i < numberOfNewHouses; i++)
            {
                Room house = new Room(RoomTag.House);
                Buildings.Add(house);
            }
        }

        private void CityBuildings()
        {
            NewHousesForSite();
            RoomTag[] tags = new RoomTag[]
            {
                RoomTag.Inn,
                RoomTag.Temple,
                RoomTag.Blacksmith,
                RoomTag.Clothier,
                RoomTag.Alchemist,
                RoomTag.Hovel,
                RoomTag.GenericWorkshop,
                RoomTag.Farm
            };

            int numberOfNewBusiness = MundaneResources % 10;
            for (int i = 0; i < numberOfNewBusiness; i++)
            {
                Room business = new Room(tags.GetRandomItemFromList());
                Buildings.Add(business);
            }
        }

        public void SimulatePopulationGrowth(WorldTile tile)
        {
            int populationCarryngCapacity = (int)tile.BiomeType;
            int totalResources = MundaneResources / 100;
            double result = (double)((double)populationCarryngCapacity / (double)((1 + totalResources))) / 100;

            if (Famine)
            {
                Population = (int)MathMagi.Round(Population / result);
                totalResources -= (int)result;
                MundaneResources -= totalResources;
                return;
            }
            var newPop = Population * result;
            Population = (int)MathMagi.Round(Population + newPop);
            int totalFoodLost;
            if (FoodQuantity != 0)
                totalFoodLost = Population / FoodQuantity;
            else
                totalFoodLost = 0;
            FoodQuantity -= totalFoodLost;
            if (FoodQuantity <= 0)
                Famine = true;
        }

        public void SimulateTradeBetweenItsRoads(Civilization civParent)
        {
            if (civParent.CivsTradingWith.Count > 0)
            {
                int resource = civParent.CivsTradingWith.Count * 5;
                if (SiteLeader is not null)
                {
                    resource *= SiteLeader.Mind.GetAbility(AbilityName.Negotiator);
                }
                MundaneResources += resource;
            }
        }

        public void AddHfAsSiteLeader(HistoricalFigure hf, int currentYear)
        {
            StringBuilder builder = new($"In the year {currentYear} the {hf.Name} assumed control of the {SiteType} {Name}");
            if (SiteLeader is not null)
            {
                builder.Append($" removing the previous leader {SiteLeader.Name} from it's post!");
                SiteLeader.RemovePreviousSiteRelation(Id);
                SiteLeader.AddRelatedSite(Id, SiteRelationTypes.Ruled);
            }

            SiteLeader = hf;

            var relationIfAny = hf.FindSiteRelation(Id);
            if (relationIfAny != null)
            {
                relationIfAny.RelationType |= SiteRelationTypes.Rules;
                if (!relationIfAny.RelationType.HasFlag(SiteRelationTypes.LivesThere))
                {
                    relationIfAny.RelationType |= SiteRelationTypes.LivesThere;
                }
            }
            else
                hf.AddRelatedSite(Id, SiteRelationTypes.LivesThere | SiteRelationTypes.Rules);

            AddNewSiteLegend(builder.ToString(), currentYear, hf);
        }

        private void AddNewSiteLegend(string legend, int when, HistoricalFigure? figure = null)
        {
            SiteLegends.Add(new Legend(legend, when, figure));
        }

        public bool CheckIfSiteHasCurrentLeaderOrDiedAndRemoveIt(int currentYear)
        {
            if (SiteLeader is not null)
            {
                if (!SiteLeader.IsAlive)
                {
                    SiteLeader.YearDeath ??= currentYear;
                    var relation = SiteLeader.FindSiteRelation(Id);
                    relation.RelationType &= ~SiteRelationTypes.Rules;
                    relation.RelationType |= SiteRelationTypes.Ruled;

                    SiteLeader = null;
                }
            }

            return SiteLeader is null;
        }

        public void AddNewLeader(Civilization civ, int currentYear)
        {
            civ.AppointNobleToAdministerSite(this, currentYear);
        }

        public void AddNewLeader(HistoricalFigure figure, int currentYear)
        {
            AddHfAsSiteLeader(figure, currentYear);
        }

        public void SimulateResearchPropagation(Civilization civ, WorldTile[,] tiles)
        {
            // TODO: Finish this!
            return;
        }

        public void AddLegend(Legend legend)
        {
            SiteLegends.Add(legend);
        }
    }
}
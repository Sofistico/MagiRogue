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
using MagiRogue.Entities;
using MagiRogue.Utils.Extensions;

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
        public List<Population> Population { get; set; }
        public int MundaneResources { get; set; }
        public int FoodQuantity { get; set; }
        public int MagicalResources { get; set; }
        public List<Building> Buildings { get; set; } = new();
        public bool Dead { get => ReturnPopNumber() <= 0; }
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

        public Site(Point pos, string name, Population totalPopulation, int? civOwnerIfAny = null)
        {
            WorldPos = pos;
            Name = name;
            Population = new List<Population>();
            Population.Add(totalPopulation);
            CivOwnerIfAny = civOwnerIfAny;
            SetId();
        }

        public void DefineSiteSize()
        {
            if (Buildings.Count <= 10)
            {
                Size = SiteSize.Small;
            }
            if (Buildings.Count > 10 && Buildings.Count <= 30)
            {
                Size = SiteSize.Medium;
            }
            if (Buildings.Count > 30)
            {
                Size = SiteSize.Large;
            }
        }

        public int ReturnPopNumber()
        {
            return Population.Select(i => i.TotalPopulation).Sum();
        }

        public void GenerateMundaneResources()
        {
            if (ReturnPopNumber() <= 0)
            {
                return;
            }

            foreach (var room in Buildings)
            {
                switch (room.PhysicalRoom.Tag)
                {
                    case RoomTag.Inn:
                        MundaneResources += 20;
                        break;

                    case RoomTag.Blacksmith:
                        MundaneResources += 100;
                        break;

                    case RoomTag.Clothier:
                        MundaneResources += 50;
                        break;

                    case RoomTag.Alchemist:
                        MundaneResources += 50;
                        MagicalResources += 25;
                        break;

                    case RoomTag.Hovel:
                        MundaneResources += 15;
                        break;

                    case RoomTag.Abandoned:
                        if (MundaneResources >= 0)
                            room.PhysicalRoom.SetPreviousTag();
                        break;

                    case RoomTag.House:
                        MundaneResources -= 2;
                        break;

                    case RoomTag.Throne:
                        MundaneResources -= 10;
                        break;

                    case RoomTag.Kitchen:
                        MundaneResources -= 20;
                        FoodQuantity += 35;
                        break;

                    case RoomTag.GenericWorkshop:
                        MundaneResources += 25;
                        break;

                    case RoomTag.Dinner:
                        MundaneResources -= 5;
                        FoodQuantity += 25;
                        break;

                    case RoomTag.DungeonKeeper:
                        MundaneResources += 15;
                        MagicalResources -= 10;
                        break;

                    case RoomTag.Farm:
                        MundaneResources += 25;
                        FoodQuantity += 100;
                        break;

                    default:
                        break;
                }

                if (MundaneResources <= 0 && room.PhysicalRoom.Tag is not RoomTag.Abandoned)
                {
                    room.PhysicalRoom.AbandonPreviousTag(RoomTag.Abandoned);
                }
                if (FoodQuantity >= 0)
                    Famine = false;
            }
            MundaneResources += 10;
            FoodQuantity += 10;
            // if the resources got big enough, update!
            DefineSiteSize();
        }

        private void SetId()
            => Id = SequentialIdGenerator.SiteId;

        public void CreateNewBuildings()
        {
            // max of 50 buildings
            if (Buildings.Count >= 50)
                return;
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
                RoomTag.ResearchRoom,
                RoomTag.EnchantingRoom,
            };
            for (int i = 0; i < numberOfNewTowerRooms; i++)
            {
                var tag = tags.GetRandomItemFromList();
                Room business = new Room();
                Building build = new Building(business);
                Buildings.Add(build);
            }
            FitRoomsCloseTogether = true;
        }

        private void NewHousesForSite()
        {
            int numberOfNewHouses = ReturnPopNumber() % 10;
            for (int i = 0; i < numberOfNewHouses; i++)
            {
                Room house = new Room(RoomTag.House);
                Buildings.Add(new Building(house));
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
                RoomTag.Farm,
            };

            int numberOfNewBusiness = MundaneResources % 10;
            for (int i = 0; i < numberOfNewBusiness; i++)
            {
                var tag = tags.GetRandomItemFromList();
                Room business = new Room(tag);
                MundaneResources -= 15;
                Buildings.Add(new Building(business));
            }
            if (TotalFoodProductionPerYear() <= TotalFoodProductionNeededToNotStarve())
            {
                Room business = new Room(RoomTag.Farm);
                MundaneResources -= 15;
                Buildings.Add(new Building(business));
            }
        }

        private int TotalFoodProductionPerYear()
        {
            int foodProduct = 0;
            foreach (var item in Buildings.Where(i => i.PhysicalRoom.Tag is RoomTag.Farm))
            {
                foodProduct += 100;
            }
            return foodProduct;
        }

        public void SimulatePopulationGrowth(WorldTile tile)
        {
            if (ReturnPopNumber() <= 0)
            {
                return;
            }
            double totalResources;
            double result;

            int populationCarryngCapacity = (int)tile.BiomeType;
            totalResources = (double)MundaneResources / 100;
            totalResources = totalResources < 0 ? totalResources * -1 : totalResources;
            result = (double)((double)populationCarryngCapacity / (double)((totalResources + 1))) / 100;

            if (Famine)
            {
                foreach (var pop in Population)
                {
                    var toSum = (double)(pop.TotalPopulation * ((result * -1)));
                    pop.TotalPopulation = (int)MathMagi.Round(pop.TotalPopulation + toSum);
                    pop.TotalPopulation = pop.TotalPopulation >= 0 ? pop.TotalPopulation : 0;
                }
                totalResources -= (int)result;
                MundaneResources -= (int)totalResources;
                return;
            }
            foreach (var pop in Population)
            {
                var newPop = pop.TotalPopulation * result;
                pop.TotalPopulation = (int)MathMagi.Round(pop.TotalPopulation + newPop);
            }
            int totalFoodLost;
            if (FoodQuantity != 0)
                totalFoodLost = ReturnPopNumber() / 2;
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
                    resource *= (SiteLeader.Mind.GetAbility(AbilityName.Negotiator) + 1);
                }
                MundaneResources += resource;
            }
        }

        public void AddHfAsSiteLeader(HistoricalFigure hf, int currentYear)
        {
            StringBuilder builder = new($"the {hf.Name} assumed control of the {SiteType} {Name}");
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
            AddLegend(builder.ToString(), currentYear);
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
            if (DiscoveriesKnow.Count <= 0)
                return;
            var sitesThatAreNotThis = civ.Sites.Where(i => i.Id != Id);
            foreach (Site siteNotThis in sitesThatAreNotThis)
            {
                siteNotThis.AddDiscovery(DiscoveriesKnow.GetRandomItemFromList());
            }
            return;
        }

        public void AddDiscovery(Discovery discovery)
        {
            if (discovery is null)
                return;
            if (DiscoveriesKnow.Any(i => i.WhatWasResearched.Id.Equals(discovery.WhatWasResearched.Id)))
            {
                return;
            }
            DiscoveriesKnow.Add(discovery);
        }

        public void AddLegend(Legend legend)
        {
            SiteLegends.Add(legend);
        }

        public void AddLegend(string legend, int yearWhen)
        {
            StringBuilder str = new StringBuilder($"In the year of {yearWhen}, ");
            str.Append(legend);
            Legend newLegend = new Legend(str.ToString(), yearWhen);
            SiteLegends.Add(newLegend);
        }

        public int TotalFoodProductionNeededToNotStarve()
        {
            return ReturnPopNumber() / 2;
        }
    }
}
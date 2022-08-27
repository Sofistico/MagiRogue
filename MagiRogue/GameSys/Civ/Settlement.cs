using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using System;
using System.Collections.Generic;
using MagiRogue.Utils;

namespace MagiRogue.GameSys.Civ
{
    public class Settlement
    {
        public Point WorldPos { get; set; }
        public string Name { get; set; }
        public SettlementSize Size { get; set; }
        public int MilitaryStrenght { get; set; }
        public int MagicStrenght { get; set; }
        public int Population { get; set; }
        public int MundaneResources { get; set; }
        public int MagicalResources { get; set; }
        public List<Room> Buildings { get; set; } = new();
        public bool Dead { get; set; }

        public Settlement()
        {
        }

        public Settlement(Point pos, string name, int totalPopulation)
        {
            WorldPos = pos;
            Name = name;
            Population = totalPopulation;
        }

        public void DefineSettlementSize()
        {
            int usableResources = 0;
            try
            {
                usableResources = Population / MundaneResources;
            }
            catch (DivideByZeroException)
            {
                Size = SettlementSize.None;
            }
            if (usableResources > 100 && usableResources < 1000)
            {
                Size = SettlementSize.Small;
            }
            if (usableResources > 1000 && usableResources < 10000)
            {
                Size = SettlementSize.Medium;
            }
            if (usableResources > 10000)
            {
                Size = SettlementSize.Large;
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

                    default:
                        break;
                }

                if (MundaneResources <= 0)
                    room.Tag = RoomTag.Abandoned;
            }
            // if the resources got big enough, update!
            DefineSettlementSize();
        }

        public void CreateNewBuildings()
        {
            int numberOfNewHouses = Population % 10;
            for (int i = 0; i < numberOfNewHouses; i++)
            {
                Room house = new Room(RoomTag.House);
                Buildings.Add(house);
            }

            int numberOfNewBusiness = MundaneResources % 10;
            for (int i = 0; i < numberOfNewBusiness; i++)
            {
                List<RoomTag> tags = new List<RoomTag>()
                {
                    RoomTag.Inn,
                    RoomTag.Temple,
                    RoomTag.Blacksmith,
                    RoomTag.Clothier,
                    RoomTag.Alchemist,
                    RoomTag.Hovel,
                    RoomTag.GenericWorkshop
                };
                Room business = new Room(tags.GetRandomItemFromList());
                Buildings.Add(business);
            }
        }
    }
}
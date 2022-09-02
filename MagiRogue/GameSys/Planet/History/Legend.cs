﻿using MagiRogue.Entities;
using MagiRogue.Utils;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Time;
using System;
using System.Text;
using MagiRogue.Data;

namespace MagiRogue.GameSys.Planet.History
{
    public class Legend
    {
        public string Happening { get; set; }
        public long TickWhen { get; set; }
        public HistoricalFigure WithWhat { get; set; }
        public HistoricalFigure WithWhats { get; set; }
        public string Where { get; set; }

        public Legend()
        {
        }

        public Legend(string happening, long tickWhen)
        {
            Happening = happening;
            TickWhen = tickWhen;
        }

        public Legend(string happening, long when, HistoricalFigure withWhat)
        {
            Happening = happening;
            TickWhen = when;
            WithWhat = withWhat;
        }

        public static Legend CreateLegendFromMyth(Myth myth, Race race = null)
        {
            string[] possibleRealms = new string[]
            {
                "fire",
                "water",
                "air",
                "earth",
                "volcano",
                "magma",
                "mountains",
                "blight",
                "undead",
                "death",
                "curse",
                "soul",
                "divinity",
            };
            string[] possibleRegions = new string[]
            {
                "deserts",
                "forests",
                "jungles",
                "mountains",
                "plains",
                "hills",
                "seas",
                "rivers",
                "tundras",
                "savannas",
                "rainforests"
            };
            string[] possibleOrigins = new string[]
            {
                "fount",
                "river",
                "wellspring",
                "light",
                "fire",
                "water",
                "air",
                "core",
                "vein",
                "blood",
                "power"
            };
            string[] possibleWhys = new string[]
            {
                "of fear",
                "of love",
                "of indifference",
                "it drained it's essence",
                "the precursors abused it",
                "they were tired of magic",
                "the world lost it's creational power",
                "because it's magic was fading",
                "magic needed to pass though him first"
            };
            string[] possibleMagic = new string[]
            {
                "the gift of magic",
                "the science of magic",
                "the study of magic",
                "need of magic",
            };
            string[] possibleBecauses = new string[]
            {
                "the races tricked it",
                "of foolishness",
                "it was angered beyond reason",
                "for change",
                "it was annoyed by the races",
                "the races asked for it",
                "it was meant to be",
                "so that the world could be populated",
                "a higher power demanded it",
                "death was a blessing",
                "death was a curse",
                "death was liberation from the mortal coil"
            };

            race = race is null ? DataManager.ListOfRaces.GetRandomItemFromList() : race;

            var legend = new Legend()
            {
                TickWhen = -1,
                Where = "In a time before time",
            };
            StringBuilder happening = new($"{myth.Name} the {myth.MythWho.ToString().SeparateByUpperLetter()}");

            // the switch from hell!
            // TODO: Make all that happenead translate to the world and it's people!
            switch (myth.MythAction)
            {
                case Data.Enumerators.MythAction.Created:
                    happening.Append(" created ");
                    switch (myth.MythWhat)
                    {
                        case Data.Enumerators.MythWhat.Race:
                            happening.Append($"the {race.RaceName}");
                            break;

                        case Data.Enumerators.MythWhat.OriginMagic:
                            string origin = possibleOrigins.GetRandomItemFromList();
                            happening.Append($"the {origin} of magic");
                            break;

                        case Data.Enumerators.MythWhat.CostMagic:

                            string why = possibleWhys.GetRandomItemFromList();
                            happening.Append("the cost of magic, ");
                            happening.Append($"because {why}");
                            break;

                        case Data.Enumerators.MythWhat.Magic:
                            happening.Append(possibleMagic.GetRandomItemFromList());
                            break;

                        case Data.Enumerators.MythWhat.Land:
                            happening.Append("the land");
                            break;

                        case Data.Enumerators.MythWhat.Region:
                            string region = possibleRegions.GetRandomItemFromList();
                            happening.Append($"the {region}");
                            break;

                        case Data.Enumerators.MythWhat.World:
                            happening.Append("the world");
                            break;

                        case Data.Enumerators.MythWhat.God:
                            string name = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the god {name}");
                            break;

                        case Data.Enumerators.MythWhat.Item:
                            string itemName = DataManager.ListOfItems.GetRandomItemFromList().Name;
                            happening.Append($"the {itemName}");
                            break;

                        case Data.Enumerators.MythWhat.Reagent:
                            happening.Append($"the TBD YOU LAZYGUY");
                            break;

                        case Data.Enumerators.MythWhat.OuterRealm:
                            string realm = possibleRealms.GetRandomItemFromList();
                            happening.Append($"the outer realm of {realm}");
                            break;

                        case Data.Enumerators.MythWhat.Space:
                            happening.Append($"the beetwen worlds");
                            break;

                        case Data.Enumerators.MythWhat.Death:
                            string because = possibleBecauses.GetRandomItemFromList();
                            happening.Append($"death, because {because}");
                            break;

                        case Data.Enumerators.MythWhat.Individual:
                            string indName = RandomNames.RandomNamesFromLanguage
                                (DataManager.ListOfLanguages.GetRandomItemFromList());
                            Race indRace = DataManager.ListOfRaces.GetRandomItemFromList();
                            happening.Append($"the {indName} of {indRace.RaceName}");
                            break;

                        default:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                            break;
                    }

                    break;

                case Data.Enumerators.MythAction.Destroyed:
                    happening.Append(" destroyed ");
                    switch (myth.MythWhat)
                    {
                        case Data.Enumerators.MythWhat.Race:
                            happening.Append($"the primordials of {race.RaceName}, so that they not exist!");
                            race.DeadRace = true;
                            break;

                        case Data.Enumerators.MythWhat.OriginMagic:
                            happening.Append("the origin of magic, thus making magic hard");
                            break;

                        case Data.Enumerators.MythWhat.CostMagic:
                            happening.Append("the cost of magic, thus making magic free");
                            break;

                        case Data.Enumerators.MythWhat.Magic:
                            happening.Append("magic, thus making magic impossible");
                            break;

                        case Data.Enumerators.MythWhat.Land:
                            string landName = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the lost land of {landName}");
                            break;

                        case Data.Enumerators.MythWhat.Region:
                            string regionName = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the lost region of {regionName}");

                            break;

                        case Data.Enumerators.MythWhat.World:
                            string worldName = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the lost world of {worldName}, thus forcing the creation cycle to begin anew");
                            break;

                        case Data.Enumerators.MythWhat.God:
                            string godName = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the god {godName}");
                            break;

                        case Data.Enumerators.MythWhat.Item:
                            string lostItem = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the item {lostItem}, thus making sure that it won't exist");
                            break;

                        case Data.Enumerators.MythWhat.Reagent:
                            string lostReagent = RandomNames.RandomNamesFromLanguage(
                                DataManager.ListOfLanguages.GetRandomItemFromList());
                            happening.Append($"the reagent {lostReagent}, thus making sure that it won't exist");
                            break;

                        case Data.Enumerators.MythWhat.Afterlife:
                            happening.Append("the afterlife, forcing the souls to wander the beetween land");
                            break;

                        case Data.Enumerators.MythWhat.OuterRealm:
                            string realm = possibleRealms.GetRandomItemFromList();
                            happening.Append($"the {realm}");
                            break;

                        case Data.Enumerators.MythWhat.Space:
                            happening.Append("the space beetween worlds, making so that only the world remains");
                            break;

                        case Data.Enumerators.MythWhat.Death:
                            // doesn't really makes sense to nothing to die!
                            happening.Append("death, so that none may die");
                            return null;

                        case Data.Enumerators.MythWhat.Demons:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                            break;

                        case Data.Enumerators.MythWhat.Angels:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                            break;

                        case Data.Enumerators.MythWhat.Spirits:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                            break;

                        case Data.Enumerators.MythWhat.Forces:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                            break;

                        case Data.Enumerators.MythWhat.Individual:
                            string indName = RandomNames.GiberishFullName(5, 5);
                            happening.Append($"the {indName}, so that none may rise like him!");
                            break;

                        default:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                            break;
                    }

                    break;

                case Data.Enumerators.MythAction.Modified:
                    happening.Append($" modified {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} to better suit it's needs");
                    break;

                case Data.Enumerators.MythAction.Antagonized:
                    happening.Append($" antagonized ");
                    switch (myth.MythWhat)
                    {
                        case Data.Enumerators.MythWhat.Race:
                            happening.Append($"the primordials of {race.RaceName}");

                            break;

                        case Data.Enumerators.MythWhat.OriginMagic:
                            happening.Append("to oppose the origin of magic!");
                            break;

                        case Data.Enumerators.MythWhat.CostMagic:
                            happening.Append("to oppose the cost of magic!");
                            break;

                        case Data.Enumerators.MythWhat.Magic:
                            happening.Append("to oppose magic and those who wield it!");

                            break;

                        case Data.Enumerators.MythWhat.Land:
                            happening.Append("the creation of the land!");

                            break;

                        case Data.Enumerators.MythWhat.Region:
                            string region = possibleRegions.GetRandomItemFromList();
                            happening.Append($"to oppose the creation of {region}!");

                            break;

                        case Data.Enumerators.MythWhat.World:
                            happening.Append($"to oppose creation of the world!");

                            break;

                        case Data.Enumerators.MythWhat.God:
                            happening.Append("to make enemy with the god");
                            break;

                        case Data.Enumerators.MythWhat.Item:
                            Item ite = DataManager.ListOfItems.GetRandomItemFromList();
                            happening.Append($"the creation of the item {ite}");
                            break;

                        case Data.Enumerators.MythWhat.Reagent:
                            happening.Append("the creation of the reagent");
                            break;

                        case Data.Enumerators.MythWhat.Afterlife:
                            happening.Append("the afterlife");
                            break;

                        case Data.Enumerators.MythWhat.OuterRealm:
                            happening.Append($"the outer realm of {possibleRealms.GetRandomItemFromList()}");
                            break;

                        case Data.Enumerators.MythWhat.Space:
                            happening.Append("the space between worlds");
                            break;

                        case Data.Enumerators.MythWhat.Death:
                            happening.Append("death");
                            break;

                        case Data.Enumerators.MythWhat.Demons:
                            happening.Append("the demons");
                            break;

                        case Data.Enumerators.MythWhat.Angels:
                            happening.Append("the angels");

                            break;

                        case Data.Enumerators.MythWhat.Spirits:
                            happening.Append("the spirits");

                            break;

                        case Data.Enumerators.MythWhat.Forces:
                            happening.Append("the forces");

                            break;

                        case Data.Enumerators.MythWhat.Individual:
                            string name = RandomNames.GiberishFullName(6, 6);
                            happening.Append($"the {name}");
                            break;

                        default:
                            happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                            break;
                    }
                    break;

                case Data.Enumerators.MythAction.Killed:
                    happening.Append(" killed ");
                    switch (myth.MythWhat)
                    {
                        case Data.Enumerators.MythWhat.Race:
                            happening.Append($"the race {race.RaceName}");
                            race.DeadRace = true;
                            break;

                        case Data.Enumerators.MythWhat.OriginMagic:
                            happening.Append("the origin of magic");
                            break;

                        case Data.Enumerators.MythWhat.CostMagic:
                            return null;

                        case Data.Enumerators.MythWhat.Magic:
                            happening.Append("magic");
                            break;

                        case Data.Enumerators.MythWhat.Land:
                            happening.Append("the land, creating a dead zone");
                            break;

                        case Data.Enumerators.MythWhat.Region:
                            happening.Append("the region, creating a dead zone");
                            break;

                        case Data.Enumerators.MythWhat.World:
                            happening.Append("the world, creating a dead world");
                            break;

                        case Data.Enumerators.MythWhat.God:
                            happening.Append($"the god {RandomNames.GiberishFullName(4, 4)}");
                            break;

                        case Data.Enumerators.MythWhat.Item:
                            return null;

                        case Data.Enumerators.MythWhat.Reagent:
                            return null;

                        case Data.Enumerators.MythWhat.Afterlife:
                            return null;

                        case Data.Enumerators.MythWhat.OuterRealm:
                            happening.Append($"the outer realm of {possibleRealms.GetRandomItemFromList()}, creating a dead realm");
                            break;

                        case Data.Enumerators.MythWhat.Space:
                            return null; //space is already dead

                        case Data.Enumerators.MythWhat.Death:
                            return null; //you can't kill death

                        case Data.Enumerators.MythWhat.Demons:
                            happening.Append("the demons, so none remains");
                            break;

                        case Data.Enumerators.MythWhat.Angels:
                            happening.Append("the angels, so none remains");
                            break;

                        case Data.Enumerators.MythWhat.Spirits:
                            happening.Append("the spirits, so none remains");
                            break;

                        case Data.Enumerators.MythWhat.Forces:
                            happening.Append("the forces, so none remains");
                            break;

                        case Data.Enumerators.MythWhat.Individual:
                            var name = RandomNames.GiberishFullName(4, 5);
                            happening.Append($"the {name}");
                            break;

                        default:
                            break;
                    }

                    break;

                case Data.Enumerators.MythAction.Gave:
                    happening.Append(" gave ");
                    switch (myth.MythWhat)
                    {
                        case Data.Enumerators.MythWhat.Race:
                            happening.Append($"to the {race.RaceName} something!");
                            return null;

                        case Data.Enumerators.MythWhat.OriginMagic:
                            happening.Append("the origin of magic away");
                            break;

                        case Data.Enumerators.MythWhat.CostMagic:
                            happening.Append("a new cost to magic!");
                            break;

                        case Data.Enumerators.MythWhat.Magic:
                            happening.Append("magic to his creation");
                            break;

                        case Data.Enumerators.MythWhat.Land:
                            happening.Append("it's land away to creation!");
                            break;

                        case Data.Enumerators.MythWhat.Region:
                            happening.Append("it's region away to creation!");
                            break;

                        case Data.Enumerators.MythWhat.World:
                            happening.Append("away it's world to come to creation!");
                            break;

                        case Data.Enumerators.MythWhat.God:
                            happening.Append($"a gift to god {RandomNames.RandomNamesFromRandomLanguage()}!");
                            break;

                        case Data.Enumerators.MythWhat.Item:
                            happening.Append("the secrets of item to creation!");
                            break;

                        case Data.Enumerators.MythWhat.Reagent:
                            happening.Append("the secrets of reagent to creation!");

                            break;

                        case Data.Enumerators.MythWhat.Afterlife:
                            happening.Append("a piece of it's power to the afterlife!");

                            break;

                        case Data.Enumerators.MythWhat.OuterRealm:
                            happening.Append($"gave away it's {possibleRealms.GetRandomItemFromList()} realm!");

                            break;

                        case Data.Enumerators.MythWhat.Space:
                            return null;

                        case Data.Enumerators.MythWhat.Death:

                            return null;

                        case Data.Enumerators.MythWhat.Demons:
                            happening.Append("gave away something to the demons!");
                            break;

                        case Data.Enumerators.MythWhat.Angels:
                            happening.Append("gave away something to the angels!");

                            break;

                        case Data.Enumerators.MythWhat.Spirits:
                            happening.Append("gave away something to the spirits!");

                            break;

                        case Data.Enumerators.MythWhat.Forces:
                            happening.Append("gave away something to the forces!");

                            break;

                        case Data.Enumerators.MythWhat.Individual:
                            happening.Append($"gave away something to the {RandomNames.GiberishFullName(5, 5)}!");
                            break;

                        default:
                            break;
                    }
                    break;

                case Data.Enumerators.MythAction.Ascended:
                    happening.Append(" ascended as a god");

                    break;

                case Data.Enumerators.MythAction.Descended:
                    happening.Append(" descended to the world");

                    break;

                case Data.Enumerators.MythAction.OpenPortal:
                    happening.Append($" opened portal to the outer realm {possibleRealms.GetRandomItemFromList()}");

                    break;

                case Data.Enumerators.MythAction.ClosedPortal:
                    happening.Append($" closed portal to the outer realm {possibleRealms.GetRandomItemFromList()}");

                    break;

                case Data.Enumerators.MythAction.Cursed:
                    happening.Append($" cursed the {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} with something!");

                    break;

                case Data.Enumerators.MythAction.Blessed:
                    happening.Append($" blessed the {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} with something!");

                    break;

                default:
                    happening.Append(" bugged the game!");
                    break;
            }

            legend.Happening = happening.ToString();

            return legend;
        }
    }
}
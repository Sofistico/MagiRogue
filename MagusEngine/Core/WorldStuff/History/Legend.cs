using Arquimedes.Enumerators;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.WorldStuff.History
{
    public class Legend
    {
        public string Happening { get; set; }
        public int YearWhen { get; set; }
        public long TickWhen { get; set; }
        public HistoricalFigure WithWho { get; set; }
        public List<HistoricalFigure> WithWhos { get; set; }
        public string Where { get; set; }

        public Legend()
        {
        }

        public Legend(string happening, int yearWhen)
        {
            Happening = happening;
            YearWhen = yearWhen;
        }

        public Legend(string happening, long tickWhen)
        {
            Happening = happening;
            TickWhen = tickWhen;
        }

        public Legend(string happening, long when, HistoricalFigure withWho)
        {
            Happening = happening;
            TickWhen = when;
            WithWho = withWho;
        }

        public static Legend CreateLegendFromMyth(Myth myth, ref HistoricalFigure figure,
            Race race = null,
            string worldName = null)
        {
            string[] possibleRealms = DataManager.ListOfRealmsName.ToArray();
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
            string[] possibleOrigins = DataManager.ListOfMagicFounts.ToArray();
            string[] possibleWhys =
            [
                "of fear",
                "of love",
                "of indifference",
                "it drained it's essence",
                "the precursors abused it",
                "they were tired of magic",
                "the world lost it's creational power",
                "because it's magic was fading",
                "magic needed to pass though him first"
            ];
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

            race ??= DataManager.ListOfRaces.GetRandomItemFromList().Value;

            var legend = new Legend()
            {
                TickWhen = -1,
                Where = "In a time before time",
            };
            StringBuilder happening = new($"{myth.Name} {myth.MythWho.ToString().SeparateByUpperLetter()}");

            // the switch from hell!
            // TODO: Make all that happenead translate to the world and it's people!
            switch (myth.MythAction)
            {
                case MythAction.Created:
                    happening.Append(" created ");
                    CreationActions(myth,
                        figure,
                        race,
                        worldName,
                        possibleRealms,
                        possibleRegions,
                        possibleOrigins,
                        possibleWhys,
                        possibleMagic,
                        possibleBecauses,
                        legend,
                        happening);

                    break;

                case MythAction.Destroyed:
                    happening.Append(" destroyed ");
                    DestroyingActions(myth, figure, happening, race, worldName, possibleRealms);

                    break;

                case MythAction.Modified:
                    happening.Append($" modified {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} to better suit it's needs");
                    break;

                case MythAction.Antagonized:
                    happening.Append($" antagonized ");
                    AntagonazingActions(myth, race, figure, possibleRealms, possibleRegions, happening);
                    break;

                case MythAction.Killed:
                    happening.Append(" killed ");
                    KillingActions(myth, figure, happening, race, possibleRealms);

                    break;

                case MythAction.Gave:
                    happening.Append(" gave ");
                    GaveActions(myth, happening, figure, race, possibleRealms);
                    break;

                case MythAction.Ascended:
                    happening.Append(" ascended as a god");
                    figure.SpecialFlags.Add(SpecialFlag.Myth);

                    break;

                case MythAction.Descended:
                    happening.Append(" descended to the world");
                    figure.SpecialFlags.Add(SpecialFlag.MythDescended);

                    break;

                case MythAction.OpenPortal:
                    happening.Append($" opened portal to the outer realm {possibleRealms.GetRandomItemFromList()}");
                    figure.SpecialFlags.Add(SpecialFlag.OpenedPortal);

                    break;

                case MythAction.ClosedPortal:
                    happening.Append($" closed portal to the outer realm {possibleRealms.GetRandomItemFromList()}");

                    break;

                case MythAction.Cursed:
                    happening.Append($" cursed the {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} with something!");

                    break;

                case MythAction.Blessed:
                    happening.Append($" blessed the {myth.MythWhat.ToString().SeparateByUpperLetter().ToLower()} with something!");

                    break;

                default:
                    happening.Append(" bugged the game!");
                    break;
            }

            legend.Happening = happening.ToString();

            return legend;
        }

        private static void GaveActions(Myth myth,
            StringBuilder happening,
            HistoricalFigure figure,
            Race race,
            string[] possibleRealms)
        {
            switch (myth.MythWhat)
            {
                case MythWhat.Race:
                    happening.Append($"to the {race.RaceName} something!");
                    return;

                case MythWhat.OriginMagic:
                    happening.Append("the origin of magic away");
                    break;

                case MythWhat.CostMagic:
                    happening.Append("a new cost to magic!");
                    break;

                case MythWhat.Magic:
                    happening.Append("magic to his creation");
                    figure.SpecialFlags.Add(SpecialFlag.GaveMagicToCreation);
                    break;

                case MythWhat.Land:
                    happening.Append("it's land away to creation!");
                    break;

                case MythWhat.Region:
                    happening.Append("it's region away to creation!");
                    break;

                case MythWhat.World:
                    happening.Append("away it's world to come to creation!");
                    break;

                case MythWhat.God:
                    happening.Append($"a gift to god {RandomNames.RandomNamesFromRandomLanguage()}!");
                    break;

                case MythWhat.Item:
                    happening.Append("the secrets of item to creation!");
                    break;

                case MythWhat.Reagent:
                    Material temp = DataManager.ListOfMaterials.GetRandomItemFromList().Value;
                    happening.Append($"the secrets of material {temp.Name} to creation!");

                    break;

                case MythWhat.Afterlife:
                    happening.Append("a piece of it's power to the afterlife!");

                    break;

                case MythWhat.OuterRealm:
                    happening.Append($"away it's {possibleRealms.GetRandomItemFromList()} realm!");

                    break;

                case MythWhat.Space:
                    return;

                case MythWhat.Death:
                    return;

                case MythWhat.Demon:
                    happening.Append("away something to the demons!");
                    break;

                case MythWhat.Angel:
                    happening.Append("away something to the angels!");

                    break;

                case MythWhat.Spirit:
                    happening.Append("away something to the spirits!");

                    break;

                case MythWhat.Force:
                    happening.Append("away something to the forces!");

                    break;

                case MythWhat.Individual:
                    happening.Append($"away something to the {RandomNames.GiberishFullName(5, 5)}!");
                    break;

                default:
                    break;
            }
        }

        private static void AntagonazingActions(Myth myth,
            Race race,
            HistoricalFigure figure,
            string[] possibleRealms,
            string[] possibleRegions,
            StringBuilder happening)
        {
            switch (myth.MythWhat)
            {
                case MythWhat.Race:
                    happening.Append("the primordials of ").Append(race.RaceName);
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.OriginMagic:
                    happening.Append("to oppose the origin of magic!");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.CostMagic:
                    happening.Append("to oppose the cost of magic!");

                    break;

                case MythWhat.Magic:
                    happening.Append("to oppose magic and those who wield it!");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.Land:
                    happening.Append("the creation of the land!");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.Region:
                    string region = possibleRegions.GetRandomItemFromList();
                    happening.Append($"to oppose the creation of {region}!");

                    break;

                case MythWhat.World:
                    happening.Append($"to oppose creation of the world!");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.God:
                    happening.Append("to make enemy with the god");
                    break;

                case MythWhat.Item:
                    Item ite = DataManager.ListOfItems.GetRandomItemFromList().Value;
                    happening.Append($"the creation of the item {ite}");
                    break;

                case MythWhat.Reagent:
                    Material temp = DataManager.ListOfMaterials.GetRandomItemFromList().Value;
                    happening.Append($"the creation of the reagent {temp.Name}");
                    break;

                case MythWhat.Afterlife:
                    happening.Append("the afterlife");
                    break;

                case MythWhat.OuterRealm:
                    happening.Append($"the outer realm of {possibleRealms.GetRandomItemFromList()}");
                    break;

                case MythWhat.Space:
                    happening.Append("the space between worlds");
                    break;

                case MythWhat.Death:
                    happening.Append("death");
                    break;

                case MythWhat.Demon:
                    happening.Append("the demons");
                    break;

                case MythWhat.Angel:
                    happening.Append("the angels");

                    break;

                case MythWhat.Spirit:
                    happening.Append("the spirits");

                    break;

                case MythWhat.Force:
                    happening.Append("the forces");

                    break;

                case MythWhat.Individual:
                    string name = RandomNames.GiberishFullName(6, 6);
                    happening.Append($"the {name}");
                    break;

                default:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                    break;
            }
        }

        private static void CreationActions(Myth myth, HistoricalFigure figure, Race race, string worldName, string[] possibleRealms,
            string[] possibleRegions, string[] possibleOrigins, string[] possibleWhys, string[] possibleMagic,
            string[] possibleBecauses, Legend legend, StringBuilder happening)
        {
            switch (myth.MythWhat)
            {
                case MythWhat.Race:
                    happening.Append($"the {race.RaceName}");
                    //figure.SpecialFlags.Add(SpecialFlag.RaceCreator);
                    break;

                case MythWhat.OriginMagic:
                    string origin = possibleOrigins.GetRandomItemFromList();
                    happening.Append($"the {origin} of magic");
                    figure.SpecialFlags.Add(SpecialFlag.MagicCreator);
                    break;

                case MythWhat.CostMagic:

                    string why = possibleWhys.GetRandomItemFromList();
                    happening.Append("the cost of magic, ");
                    happening.Append($"because {why}");
                    break;

                case MythWhat.Magic:
                    happening.Append(possibleMagic.GetRandomItemFromList());
                    break;

                case MythWhat.Land:
                    happening.Append("the land");
                    break;

                case MythWhat.Region:
                    string region = possibleRegions.GetRandomItemFromList();
                    happening.Append($"the {region}");
                    break;

                case MythWhat.World:
                    happening.Append($"the world {worldName}");
                    figure.SpecialFlags.Add(SpecialFlag.WorldCreator);
                    break;

                case MythWhat.God:
                    string name = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the god {name}");
                    legend.WithWho = new(name, $"a god born of {myth.Name}");
                    break;

                case MythWhat.Item:
                    string itemName = DataManager.ListOfItems.GetRandomItemFromList().Value.Name;
                    happening.Append($"the {itemName}");
                    break;

                case MythWhat.Reagent:
                    Material temp = DataManager.ListOfMaterials.GetRandomItemFromList().Value;
                    happening.Append($"the material {temp.Name}");
                    break;

                case MythWhat.OuterRealm:
                    string realm = possibleRealms.GetRandomItemFromList();
                    happening.Append($"the outer realm of {realm}");
                    break;

                case MythWhat.Space:
                    happening.Append($"the beetwen worlds");
                    break;

                case MythWhat.Death:
                    string because = possibleBecauses.GetRandomItemFromList();
                    happening.Append($"death, because {because}");
                    break;

                case MythWhat.Individual:
                    string indName = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    Race indRace = DataManager.ListOfRaces.GetRandomItemFromList().Value;
                    happening.Append($"the {indName} of {indRace.RaceName}");
                    legend.WithWho = new HistoricalFigure(indName, indRace.Description,
                        indRace.ReturnRandomSex(), indRace.Id, true);
                    break;

                default:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                    break;
            }
        }

        private static void DestroyingActions(Myth myth, HistoricalFigure figure, StringBuilder happening,
            Race race, string worldName, string[] possibleRealms)
        {
            switch (myth.MythWhat)
            {
                case MythWhat.Race:
                    happening.Append($"the primordials of {race.RaceName}, so that they not exist!");
                    race.DeadRace = true;
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);
                    break;

                case MythWhat.OriginMagic:
                    happening.Append("the origin of magic, thus making magic hard");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);
                    break;

                case MythWhat.CostMagic:
                    happening.Append("the cost of magic, thus making magic free");
                    break;

                case MythWhat.Magic:
                    happening.Append("magic, thus making magic impossible");
                    figure.SpecialFlags.Add(SpecialFlag.MagicKiller);
                    break;

                case MythWhat.Land:
                    string landName = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the lost land of {landName}");
                    break;

                case MythWhat.Region:
                    string regionName = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the lost region of {regionName}");

                    break;

                case MythWhat.World:
                    happening.Append($"the lost world of {worldName}, thus forcing the creation cycle to begin anew");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);
                    break;

                case MythWhat.God:
                    string godName = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the god {godName}");
                    break;

                case MythWhat.Item:
                    string lostItem = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the item {lostItem}, thus making sure that it won't exist");
                    break;

                case MythWhat.Reagent:
                    string lostReagent = RandomNames.RandomNamesFromLanguage(DataManager.ListOfLanguages.GetRandomItemFromList().Value);
                    happening.Append($"the reagent {lostReagent}, thus making sure that it won't exist");
                    break;

                case MythWhat.Afterlife:
                    happening.Append("the afterlife, forcing the souls to wander the beetween land");
                    break;

                case MythWhat.OuterRealm:
                    string realm = possibleRealms.GetRandomItemFromList();
                    happening.Append($"the {realm}");
                    break;

                case MythWhat.Space:
                    happening.Append("the space beetween worlds, making so that only the world remains");
                    break;

                case MythWhat.Death:
                    // doesn't really makes sense to nothing to die!
                    happening.Append("death, so that none may die");
                    return;

                case MythWhat.Demon:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                    break;

                case MythWhat.Angel:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                    break;

                case MythWhat.Spirit:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                    break;

                case MythWhat.Force:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");

                    break;

                case MythWhat.Individual:
                    string indName = RandomNames.GiberishFullName(5, 5);
                    happening.Append($"the {indName}, so that none may rise like him!");
                    break;

                default:
                    happening.Append($"the {myth.MythWhat.ToString().ToLower()}");
                    break;
            }
        }

        private static void KillingActions(Myth myth, HistoricalFigure figure, StringBuilder happening,
            Race race, string[] possibleRealms)
        {
            switch (myth.MythWhat)
            {
                case MythWhat.Race:
                    happening.Append($"the race {race.RaceName}");
                    race.DeadRace = true;
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.OriginMagic:
                    happening.Append("the origin of magic");
                    figure.SpecialFlags.Add(SpecialFlag.Antagonist);

                    break;

                case MythWhat.CostMagic:
                    return;

                case MythWhat.Magic:
                    happening.Append("magic");
                    figure.SpecialFlags.Add(SpecialFlag.MagicKiller);

                    break;

                case MythWhat.Land:
                    happening.Append("the land, creating a dead zone");
                    break;

                case MythWhat.Region:
                    happening.Append("the region, creating a dead zone");
                    break;

                case MythWhat.World:
                    happening.Append("the world, creating a dead world");
                    break;

                case MythWhat.God:
                    happening.Append($"the god {RandomNames.GiberishFullName(4, 4)}");
                    break;

                case MythWhat.Item:
                    return;

                case MythWhat.Reagent:
                    return;

                case MythWhat.Afterlife:
                    return;

                case MythWhat.OuterRealm:
                    happening.Append($"the outer realm of {possibleRealms.GetRandomItemFromList()}, creating a dead realm");
                    break;

                case MythWhat.Space:
                    return; //space is already dead

                case MythWhat.Death:
                    return; //you can't kill death

                case MythWhat.Demon:
                    happening.Append("the demons, so none remains");
                    break;

                case MythWhat.Angel:
                    happening.Append("the angels, so none remains");
                    break;

                case MythWhat.Spirit:
                    happening.Append("the spirits, so none remains");
                    break;

                case MythWhat.Force:
                    happening.Append("the forces, so none remains");
                    break;

                case MythWhat.Individual:
                    var name = RandomNames.GiberishFullName(4, 5);
                    happening.Append($"the {name}");
                    break;

                default:
                    break;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Happening);
            return sb.ToString();
        }

        public static string StringFromChangingSiteLoc(HistoricalFigure figureToChange, Site site)
        {
            return $"the {figureToChange.Name} has changed {figureToChange.PronoumPossesive()} location to {site.Name}!";
        }

        public static string ReturnMadeFriendString(HistoricalFigure figure, HistoricalFigure randomPerson)
        {
            return $"the {figure.Name} became friends with {randomPerson.Name}";
        }
    }
}

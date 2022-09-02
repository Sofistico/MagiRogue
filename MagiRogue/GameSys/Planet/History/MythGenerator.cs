using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;

namespace MagiRogue.GameSys.Planet.History
{
    public class MythGenerator
    {
        private int mythId;

        public MythGenerator()
        {
        }

        public List<Myth> GenerateMyths(List<Race> races,
            List<HistoricalFigure> figures, PlanetMap planet)
        {
            List<Myth> myths = new List<Myth>();
            // generate the primordials hfs (gods, demons, world, etc)
            // and also define who or what created the world!
            myths.AddRange(WhoCreatedTheWorld(figures, races));

            return myths;
        }

        private List<Myth> WhoCreatedTheWorld(List<HistoricalFigure> figures, List<Race> races)
        {
            string[] adjectives = new string[]
            {
                "celestial",
                "demonic",
                "hardened",
                "draconic",
                "colossal",
                "lively",
                "murderous",
                "traitorous",
                "evil",
                "cursed",
                "blessed",
                "immortal"
            };

            List<Myth> myths = new List<Myth>();
            MythWho[] primordias = Enum.GetValues<MythWho>().ToArray();
            MythWho precursor = primordias.GetRandomItemFromList();
            HistoricalFigure precursorFigure = null;
            Myth myth = null;
            Legend legend = null;
            bool alive = GameLoop.GlobalRand.NextBool();
            Race race = races.GetRandomItemFromList();
            bool createdRace = false;

            switch (precursor)
            {
                case MythWho.God:
                    precursorFigure = new HistoricalFigure(
                        RandomNames.GiberishFullName(6, 4),
                        "The creator god",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive // needs to be alive to create the universe after all! unleass the creation killed him...
                        );
                    createdRace = true;
                    break;

                case MythWho.Gods:
                    precursorFigure = new HistoricalFigure(
                       $"The pantheon of {RandomNames.RandomNamesFromRandomLanguage()}",
                       "The pantheon of the creator gods");

                    break;

                case MythWho.Egg:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()}",
                        "A cosmic egg");
                    break;

                case MythWho.Chaos:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()}",
                        "A primordial chaos");
                    break;

                case MythWho.Chance:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()} chance",
                        "Pure cosmic chance");

                    break;

                case MythWho.Science:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()} science",
                        "Mad science!");
                    break;

                case MythWho.Wizard:
                    precursorFigure = new HistoricalFigure($"The {RandomNames.GiberishFullName(6, 6)} wizard",
                        "A wizard did it!", race.ReturnRandomSex(), race.Id, alive);
                    createdRace = true;
                    break;

                case MythWho.Magic:
                    precursorFigure = new HistoricalFigure($"The Primordial Magic {adjectives.GetRandomItemFromList()}",
                        "Primordial magic");

                    break;

                case MythWho.Titan:
                    precursorFigure = new HistoricalFigure($"The Primordial Titan {adjectives.GetRandomItemFromList()}",
                        "a Primordial titan");

                    break;

                case MythWho.Titans:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()} Primordial Titans",
                        "a Primordial titans");
                    break;

                case MythWho.Precursors:
                    precursorFigure = new HistoricalFigure($"The {adjectives.GetRandomItemFromList()} Precursors",
                        "Precursors");

                    break;

                case MythWho.Demons:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of demons",
                        "Group of demons");

                    break;

                case MythWho.Angels:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of angels",
                        "Group of angels");
                    break;

                case MythWho.Spirits:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of spirits",
                        "Group of spirits");
                    break;

                case MythWho.Forces:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of forces",
                        "Group of forces");
                    break;

                default:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of unkows",
                        "Group of unkows");

                    break;
            }
            myth = new Myth(mythId++, precursorFigure.Name, precursor, MythAction.Created, MythWhat.World);
            legend = Legend.CreateLegendFromMyth(myth);
            precursorFigure.AddLegend(legend);
            if (createdRace)
            {
                Myth raceCreationMyth = new Myth(mythId++,
                    precursorFigure.Name,
                    precursor,
                    MythAction.Created,
                    MythWhat.Race);
                Legend raceCreation = Legend.CreateLegendFromMyth(raceCreationMyth, race);
                if (!alive)
                {
                    StringBuilder str = new StringBuilder(raceCreation.Happening);
                    str.Append($" which consumed {precursorFigure.Pronoum()}!");
                }
                precursorFigure.AddLegend(raceCreation);
            }

            myths.Add(myth);
            figures.Add(precursorFigure);

            return myths;
        }
    }
}
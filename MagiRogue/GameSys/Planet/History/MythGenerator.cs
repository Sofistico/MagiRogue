using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Data.Enumerators;
using MagiRogue.Utils;
using MagiRogue.Data;

namespace MagiRogue.GameSys.Planet.History
{
    public sealed class MythGenerator
    {
        // unhard code this!
        private const int nmbrInteractions = 5;
        private const int nmbrOfNewMyths = 3;

        public MythGenerator()
        {
        }

        public List<Myth> GenerateMyths(List<Race> races,
            List<HistoricalFigure> figures, PlanetMap planet)
        {
            // there is lot that will need some tune up and some rewrite in the future
            // but for now, let's see to where i can go with it!
            List<Myth> myths = new List<Myth>();
            // generate the primordials hfs (gods, demons, world, etc)
            // and also define who or what created the world!
            myths.AddRange(WhoCreatedTheWorld(figures, races, planet.Name));
            myths.AddRange(CreateSomeMinorMyths(figures, races, nmbrOfNewMyths));
            myths.AddRange(BunchOfMythsInteraction(races, figures, planet, nmbrInteractions));

            foreach (var item in figures)
            {
                // generate random personality for the various primoridal beings
                item.GenerateRandomPersonality();
            }

            return myths;
        }

        private IEnumerable<Myth> CreateSomeMinorMyths(List<HistoricalFigure> figures,
            List<Race> races,
            int nmbrOfMythsToCreate)
        {
            List<Myth> myths = new();
            for (int i = 0; i < nmbrOfMythsToCreate; i++)
            {
                var num = new MythWhat[]{
                    MythWhat.God,
                    MythWhat.Forces,
                    MythWhat.Race,
                    MythWhat.Spirits,
                    MythWhat.Forces,
                };
                Myth newMyth = new Myth(figures[0].Name, figures[0].MythWho,
                    MythAction.Created, num.GetRandomItemFromList());
                HistoricalFigure createdFigure = newMyth.CreateFigureFromMyth(figures[0].Name);
                if (createdFigure is not null)
                {
                    figures[0].AddRelatedHf(createdFigure.Id, HfRelationType.Creator);
                    figures.Add(createdFigure);
                    myths.Add(newMyth);
                }
            }

            return myths;
        }

        private IEnumerable<Myth> BunchOfMythsInteraction(List<Race> races,
            List<HistoricalFigure> figures, PlanetMap planet, int nmbrOfIterations)
        {
            List<Myth> myths = new();
            bool processingMyths = true;
            int currentIteration = 0;
            Stack<HistoricalFigure> stack = new Stack<HistoricalFigure>(figures);

            while (processingMyths)
            {
                currentIteration++;

                while (stack.Count > 0)
                {
                    HistoricalFigure figure = stack.Pop();
                    Myth myth = figure.MythAct();
                    myths.Add(myth);

                    if (myth.MythAction is MythAction.Created &&
                        (myth.MythWhat is MythWhat.Race
                        || myth.MythWhat is MythWhat.Angels
                        || myth.MythWhat is MythWhat.Demons
                        || myth.MythWhat is MythWhat.Forces
                        || myth.MythWhat is MythWhat.God
                        || myth.MythWhat is MythWhat.Individual
                        || myth.MythWhat is MythWhat.Spirits
                        || myth.MythWhat is MythWhat.Region))
                    {
                        HistoricalFigure createdFigure = myth.CreateFigureFromMyth(figure.Name);
                        figures.Add(createdFigure);
                    }

                    Legend legend = Legend.CreateLegendFromMyth(myth, ref figure, worldName: planet.Name);
                    figure.AddLegend(legend);
                    myths.Add(myth);
                }

                if (currentIteration >= nmbrOfIterations)
                {
                    processingMyths = false;
                    stack.Clear();
                }

                foreach (HistoricalFigure figure in figures)
                {
                    stack.Push(figure);
                }

                if (currentIteration >= nmbrOfIterations)
                    processingMyths = false;
            }

            return myths;
        }

        //TODO: REmove all hardcode from this shit!
        private List<Myth> WhoCreatedTheWorld(List<HistoricalFigure> figures, List<Race> races, string planetName)
        {
            string[] adjectives = DataManager.ListOfAdjectives.ToArray();

            List<Myth> myths = new List<Myth>();
            MythWho[] primordias = Enum.GetValues<MythWho>().ToArray();
            MythWho precursor = primordias.GetRandomItemFromList();
            HistoricalFigure precursorFigure;

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
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "A cosmic egg");
                    break;

                case MythWho.Chaos:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        $"A primordial chaos in the shape of a {DataManager.ListOfShapes.GetRandomItemFromList().Name[0]}");
                    break;

                case MythWho.Chance:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "Pure cosmic chance");

                    break;

                case MythWho.Science:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "The Big Bang");
                    break;

                case MythWho.Wizard:
                    precursorFigure = new HistoricalFigure($"{RandomNames.GiberishFullName(6, 6)}",
                        "A wizard did it!", race.ReturnRandomSex(), race.Id, alive);
                    createdRace = true;
                    break;

                case MythWho.Magic:
                    precursorFigure = new HistoricalFigure($"The Primordial {adjectives.GetRandomItemFromList()}",
                        "Primordial magic");

                    break;

                case MythWho.Titan:
                    precursorFigure = new HistoricalFigure($"The Primordial {adjectives.GetRandomItemFromList()}",
                        "a Primordial titan");

                    break;

                case MythWho.Titans:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} Primordial",
                        "a Primordial titans");
                    break;

                case MythWho.Precursors:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "Precursors");

                    break;

                case MythWho.Demons:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of",
                        "Group of demons");

                    break;

                case MythWho.Angels:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of",
                        "Group of angels");
                    break;

                case MythWho.Spirits:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of",
                        "Group of spirits");
                    break;

                case MythWho.Forces:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of",
                        "Group of forces");
                    break;

                default:
                    precursorFigure = new HistoricalFigure($"a {adjectives.GetRandomItemFromList()} group of {DataManager.ListOfShapes.GetRandomItemFromList().Name[0]}",
                        $"Group of {DataManager.ListOfShapes.GetRandomItemFromList().Name[0]}");

                    break;
            }

            Myth myth = new Myth(
                precursorFigure.Name,
                precursor,
                MythAction.Created,
                MythWhat.World);
            Legend legend = Legend.CreateLegendFromMyth(myth, ref precursorFigure, worldName: planetName);
            precursorFigure.AddLegend(legend);
            precursorFigure.MythWho = precursor;

            if (createdRace)
            {
                Myth raceCreationMyth = new Myth(
                    precursorFigure.Name,
                    precursor,
                    MythAction.Created,
                    MythWhat.Race);
                Legend raceCreation = Legend.CreateLegendFromMyth(raceCreationMyth, ref precursorFigure, race);
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
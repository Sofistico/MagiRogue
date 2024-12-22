using Arquimedes.Enumerators;
using GoRogue.Random;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Exceptions;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagusEngine.Core.WorldStuff.History
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
            // there is lot that will need some tune up and some rewrite in the future but for now,
            // let's see to where i can go with it!
            List<Myth> myths =
            [
                // generate the primordials hfs (gods, demons, world, etc) and also define who or what
                // created the world!
                .. WhoCreatedTheWorld(figures, races, planet.Name),
                .. CreateSomeMinorMyths(figures, races, nmbrOfNewMyths),
                .. BunchOfMythsInteraction(races, figures, planet, nmbrInteractions),
            ];

            foreach (var item in figures)
            {
                // generate random personality for the various primoridal beings
                if (item is not null)
                {
                    item.GenerateRandomPersonality();
                    item.IsAlive = true;
                }
            }

            return myths;
        }

        private static IEnumerable<Myth> CreateSomeMinorMyths(List<HistoricalFigure> figures,
            List<Race> races,
            int nmbrOfMythsToCreate)
        {
            List<Myth> myths = new();
            for (int i = 0; i < nmbrOfMythsToCreate; i++)
            {
                var num = new MythWhat[]{
                    MythWhat.God,
                    MythWhat.Force,
                    MythWhat.Race,
                    MythWhat.Spirit,
                    MythWhat.Force,
                };
                Myth newMyth = new Myth(figures[0].Name, figures[0].MythWho,
                    MythAction.Created, num.GetRandomItemFromList());
                HistoricalFigure createdFigure = newMyth.CreateFigureFromMyth(figures[0].Name);
                if (createdFigure is not null)
                {
                    createdFigure.HfAnatomy.Ages = false;

                    figures[0].AddRelatedHf(createdFigure.Id, HfRelationType.Creator);
                    figures.Add(createdFigure);
                    myths.Add(newMyth);
                }
            }

            return myths;
        }

        private static IEnumerable<Myth> BunchOfMythsInteraction(List<Race> races,
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
                    if (figure is null)
                        continue;
                    Myth myth = figure.MythAct();
                    myths.Add(myth);

                    if (myth.MythAction is MythAction.Created &&
                        (myth.MythWhat is MythWhat.Race
                        || myth.MythWhat is MythWhat.Angel
                        || myth.MythWhat is MythWhat.Demon
                        || myth.MythWhat is MythWhat.Force
                        || myth.MythWhat is MythWhat.God
                        || myth.MythWhat is MythWhat.Individual
                        || myth.MythWhat is MythWhat.Spirit
                        || myth.MythWhat is MythWhat.Region))
                    {
                        HistoricalFigure createdFigure = myth.CreateFigureFromMyth(figure.Name);
                        if (createdFigure is not null)
                            figures.Add(createdFigure);
                    }

                    Legend legend = Legend.CreateLegendFromMyth(myth, ref figure, worldName: planet.Name);
                    figure.AddLegend(legend);
                    myths.Add(myth);
                }

                if (currentIteration >= nmbrOfIterations)
                {
                    processingMyths = false;
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
            string[] adjectives = DataManager.ListOfAdjectives.GetEnumerableCollection().ToArray();

            List<Myth> myths = new List<Myth>();
            MythWho[] primordias = Enum.GetValues<MythWho>().ToArray();
            MythWho precursor = primordias.GetRandomItemFromList();
            HistoricalFigure precursorFigure;

            bool alive = GlobalRandom.DefaultRNG.NextBool();
            Race race = races.GetRandomItemFromCollection() ?? throw new NullValueException(nameof(race));
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

                case MythWho.Egg:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} egg",
                        "A cosmic egg");
                    break;

                case MythWho.Chaos:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} chaos",
                        $"A primordial chaos in the shape of a {DataManager.ListOfShapes!.GetEnumerableCollection().GetRandomItemFromCollection()!.Name[0]}");
                    break;

                case MythWho.Chance:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} chance",
                        "Pure cosmic chance");

                    break;

                case MythWho.Science:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "The Big Bang");
                    break;

                case MythWho.Magic:
                    precursorFigure = new HistoricalFigure($"the {adjectives.GetRandomItemFromList()} magic",
                        "Primordial magic");

                    break;

                case MythWho.Titan:
                    precursorFigure = new HistoricalFigure($"primordial {adjectives.GetRandomItemFromList()} titan",
                        "a Primordial titan",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive // needs to be alive to create the universe after all! unleass the creation killed him...
                        );

                    break;

                case MythWho.Precursor:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()}",
                        "Precursor",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive);

                    break;

                case MythWho.Demon:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} {RandomNames.RandomNamesFromRandomLanguage()}",
                        "a demon",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive);

                    break;

                case MythWho.Angel:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} {RandomNames.RandomNamesFromRandomLanguage()}",
                        "an angel",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive);
                    break;

                case MythWho.Force:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} {RandomNames.RandomNamesFromRandomLanguage()}",
                        "a force",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive);
                    break;

                default:
                    precursorFigure = new HistoricalFigure($"{adjectives.GetRandomItemFromList()} {RandomNames.RandomNamesFromRandomLanguage()}",
                        $"A figure shaped like {DataManager.ListOfShapes!.GetEnumerableCollection().GetRandomItemFromCollection()!.Name[0]}");

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
            precursorFigure.HfAnatomy.Ages = false;

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

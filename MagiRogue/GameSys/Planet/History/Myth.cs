using MagiRogue.Data;
using MagiRogue.Utils;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagiRogue.GameSys.Planet.History
{
    public class Myth
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MythWho MythWho { get; set; }
        public MythAction MythAction { get; set; }
        public MythWhat MythWhat { get; set; }
        /*public List<string> WhatItCreated { get; set; } = new();
        public List<string> WhatItDestroyed { get; set; } = new();
        public List<string> WhatItAntagonized { get; set; } = new();
        public List<string> WhatItGave { get; set; } = new();
        public List<string> WhatItBlessed { get; set; } = new();
        public List<string> WhatItCursed { get; set; } = new();
        public List<string> WhatItModified { get; set; } = new();*/

        public Myth()
        {
            Id = GameLoop.GetMythId();
        }

        public Myth(string name, MythWho mythWho, MythAction action, MythWhat whatAction)
        {
            Id = GameLoop.GetMythId();
            Name = name;
            MythWho = mythWho;
            MythAction = action;
            MythWhat = whatAction;
        }

        public HistoricalFigure CreateFigureFromMyth(string creatorName)
        {
            if (MythAction is not MythAction.Created)
                return null;

            HistoricalFigure figure;
            Race race;

            switch (MythWhat)
            {
                //case MythWhat.Race:
                //    // in the future, dynamic races will be here!
                //    //figure = null;
                //    break;

                case MythWhat.God:
                    race = DataManager.RandomRace();
                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()}",
                        $"A god of the realm of {DataManager.RandomRealm()}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    figure.MythWho = MythWho.God;
                    break;

                case MythWhat.Demon:
                    race = DataManager.RandomRace();

                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()} {creatorName} demon",
                        $"A demon created by {creatorName}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    figure.MythWho = MythWho.Demon;
                    break;

                case MythWhat.Angel:
                    race = DataManager.RandomRace();

                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()} {creatorName} angel",
                        $"A angel created by {creatorName}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    figure.MythWho = MythWho.Angel;

                    break;

                //case MythWhat.Spirits:
                //    figure = new HistoricalFigure($"A group of {creatorName} demons",
                //        $"A group of demons created by {creatorName}");
                //    figure.MythWho = MythWho.Spi;
                //    break;

                case MythWhat.Force:
                    race = DataManager.RandomRace();
                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()} {creatorName} force",
                        $"A force created by {creatorName}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    figure.MythWho = MythWho.Force;
                    break;

                case MythWhat.Individual:
                    race = DataManager.RandomRace();
                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()}",
                        $"A god of the realm of {DataManager.RandomRealm()}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    figure.MythWho = MythWho.God;
                    break;

                default:
                    figure = null;
                    break;
            }
            if (figure is null)
                return null;
            figure.SpecialFlags.Add(SpecialFlag.Myth);
            figure.SpecialFlags.Add(SpecialFlag.Supernatural);
            figure.SpecialFlags.Add(SpecialFlag.Magical);
            figure.SpecialFlags.Add(SpecialFlag.Sapient);
            figure.GetAnatomy().Ages = false;
            return figure;
        }

        public static MythWhat SetFlag(MythWhat a, MythWhat b)
        {
            return a | b;
        }

        public static MythWhat UnsetFlag(MythWhat a, MythWhat b)
        {
            return a & (~b);
        }

        // Works with "None" as well
        public static bool HasFlag(MythWhat a, MythWhat b)
        {
            return (a & b) == b;
        }

        public static MythWhat ToogleFlag(MythWhat a, MythWhat b)
        {
            return a ^ b;
        }

        public override string ToString()
        {
            return $"{MythWho} - {MythAction} - {MythWhat}";
        }
    }
}
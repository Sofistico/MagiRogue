﻿using MagiRogue.Data;
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

        public Myth(int id)
        {
            Id = id;
        }

        public Myth(int id, string name, MythWho mythWho, MythAction action, MythWhat whatAction)
        {
            Id = id;
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
                case MythWhat.Race:
                    // in the future, dynamic races will be here!
                    figure = null;
                    break;

                case MythWhat.God:
                    race = DataManager.RandomRace();
                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()}",
                        $"A god of the realm of {DataManager.RandomRealm()}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    break;

                case MythWhat.Demons:
                    figure = new HistoricalFigure($"A group of {creatorName} demons",
                        $"A group of demons created by {creatorName}");
                    break;

                case MythWhat.Angels:
                    figure = new HistoricalFigure($"A group of {creatorName} demons",
                        $"A group of demons created by {creatorName}");

                    break;

                case MythWhat.Spirits:
                    figure = new HistoricalFigure($"A group of {creatorName} demons",
                        $"A group of demons created by {creatorName}");

                    break;

                case MythWhat.Forces:
                    figure = new HistoricalFigure($"A group of {creatorName} demons",
                        $"A group of demons created by {creatorName}");

                    break;

                case MythWhat.Individual:
                    race = DataManager.RandomRace();
                    figure = new HistoricalFigure($"{RandomNames.RandomNamesFromRandomLanguage()}",
                        $"A god of the realm of {DataManager.RandomRealm()}",
                        race.ReturnRandomSex(),
                        race.Id,
                        true);
                    break;

                default:
                    figure = null;
                    break;
            }

            return figure;
        }
    }
}
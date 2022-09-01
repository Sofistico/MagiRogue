using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Time;
using System;

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

        public static Legend CreateLegendFromMyth(Myth myth)
        {
            var legend = new Legend();

            switch (myth.MythWho)
            {
                case Data.Enumerators.MythWho.God:
                    break;

                case Data.Enumerators.MythWho.Gods:
                    break;

                case Data.Enumerators.MythWho.Egg:
                    break;

                case Data.Enumerators.MythWho.Chaos:
                    break;

                case Data.Enumerators.MythWho.Chance:
                    break;

                case Data.Enumerators.MythWho.Science:
                    break;

                case Data.Enumerators.MythWho.Wizard:
                    break;

                case Data.Enumerators.MythWho.Magic:
                    break;

                case Data.Enumerators.MythWho.Titan:
                    break;

                case Data.Enumerators.MythWho.Titans:
                    break;

                case Data.Enumerators.MythWho.Precursors:
                    break;

                case Data.Enumerators.MythWho.Demons:
                    break;

                case Data.Enumerators.MythWho.Angels:
                    break;

                case Data.Enumerators.MythWho.Spirits:
                    break;

                case Data.Enumerators.MythWho.Forces:
                    break;

                default:
                    break;
            }

            switch (myth.MythAction)
            {
                case Data.Enumerators.MythAction.Created:
                    break;

                case Data.Enumerators.MythAction.Destroyed:
                    break;

                case Data.Enumerators.MythAction.Modified:
                    break;

                case Data.Enumerators.MythAction.Antagonized:
                    break;

                case Data.Enumerators.MythAction.Killed:
                    break;

                case Data.Enumerators.MythAction.Gave:
                    break;

                case Data.Enumerators.MythAction.Ascended:
                    break;

                case Data.Enumerators.MythAction.Descended:
                    break;

                case Data.Enumerators.MythAction.OpenPortal:
                    break;

                case Data.Enumerators.MythAction.ClosedPortal:
                    break;

                case Data.Enumerators.MythAction.Cursed:
                    break;

                case Data.Enumerators.MythAction.Blessed:
                    break;

                default:
                    break;
            }

            switch (myth.MythWhat)
            {
                case Data.Enumerators.MythWhat.Race:
                    break;

                case Data.Enumerators.MythWhat.OriginMagic:
                    break;

                case Data.Enumerators.MythWhat.CostMagic:
                    break;

                case Data.Enumerators.MythWhat.Magic:
                    break;

                case Data.Enumerators.MythWhat.Land:
                    break;

                case Data.Enumerators.MythWhat.Region:
                    break;

                case Data.Enumerators.MythWhat.World:
                    break;

                case Data.Enumerators.MythWhat.God:
                    break;

                case Data.Enumerators.MythWhat.Item:
                    break;

                case Data.Enumerators.MythWhat.Reagent:
                    break;

                case Data.Enumerators.MythWhat.Afterlife:
                    break;

                case Data.Enumerators.MythWhat.OuterRealm:
                    break;

                case Data.Enumerators.MythWhat.Space:
                    break;

                case Data.Enumerators.MythWhat.Death:
                    break;

                case Data.Enumerators.MythWhat.Demons:
                    break;

                case Data.Enumerators.MythWhat.Angels:
                    break;

                case Data.Enumerators.MythWhat.Spirits:
                    break;

                case Data.Enumerators.MythWhat.Forces:
                    break;

                case Data.Enumerators.MythWhat.Individual:
                    break;

                default:
                    break;
            }

            return legend;
        }
    }
}
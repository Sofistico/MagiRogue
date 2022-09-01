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
            List<Myth> myths = new List<Myth>();
            MythWho[] primordias = Enum.GetValues<MythWho>().ToArray();
            MythWho precursor = primordias.GetRandomItemFromList();
            switch (precursor)
            {
                case MythWho.God:
                    Race race = races.GetRandomItemFromList();
                    bool alive = GameLoop.GlobalRand.NextBool();
                    HistoricalFigure godHf = new HistoricalFigure(
                        RandomNames.GiberishFullName(6, 4),
                        "The creator god",
                        race.ReturnRandomSex(),
                        race.Id,
                        alive // needs to be alive to create the universe after all!
                        );
                    StringBuilder mythStr = new StringBuilder();
                    Myth myth = new Myth(mythId++, precursor, MythAction.Created, MythWhat.World);
                    Legend legend = Legend.CreateLegendFromMyth(myth);
                    godHf.AddLegend(legend);
                    myths.Add(myth);
                    figures.Add(godHf);

                    break;

                case MythWho.Gods:
                    break;

                case MythWho.Egg:
                    break;

                case MythWho.Chaos:
                    break;

                case MythWho.Chance:
                    break;

                case MythWho.Science:
                    break;

                case MythWho.Wizard:
                    break;

                case MythWho.Magic:
                    break;

                case MythWho.Titan:
                    break;

                case MythWho.Titans:
                    break;

                case MythWho.Precursors:
                    break;

                case MythWho.Demons:
                    break;

                case MythWho.Angels:
                    break;

                case MythWho.Spirits:
                    break;

                case MythWho.Forces:
                    break;

                default:
                    break;
            }

            return myths;
        }
    }
}
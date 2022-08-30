using MagiRogue.GameSys.Time;

namespace MagiRogue.GameSys.Civ
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
    }
}
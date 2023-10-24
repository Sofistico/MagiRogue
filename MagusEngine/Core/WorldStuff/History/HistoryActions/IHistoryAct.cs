namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    public interface IHistoryAct
    {
        public bool? Act(HistoricalFigure figure);
    }
}

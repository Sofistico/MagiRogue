namespace Arquimedes.Interfaces
{
    public interface IDataRepository<out TData, in TQuery>
    {
        TData? Query(TQuery id);
        IEnumerable<TData> GetEnumerableCollection();
    }
}

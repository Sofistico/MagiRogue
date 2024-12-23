namespace Arquimedes.Interfaces
{
    public interface IDataRepository<out TData, in TQuery>
    {
        public TData? Query(TQuery id);
        public IEnumerable<TData> GetEnumerableCollection();
    }
}

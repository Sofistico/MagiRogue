namespace Arquimedes.Interfaces
{
    public interface IDataRepository<T, TQuery>
    {
        public T? Query(TQuery id);
        public IEnumerable<T> GetEnumerableCollection();
    }
}

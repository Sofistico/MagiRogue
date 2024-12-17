using Arquimedes.Interfaces;
using Arquimedes.Utils;

namespace Arquimedes.Data
{
    public class ListDataRepository<T>(string path) : IDataRepository<T, Func<T>>
    {
        private readonly Lazy<List<T>> _data = new(() => FileUtils.GetSourceTreeList<T>(path));

        public IEnumerable<T> GetEnumerableCollection()
        {
            return _data.Value;
        }

        public T? Query(Func<T> id)
        {
            return id.Invoke();
        }
    }
}

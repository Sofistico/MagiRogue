using Arquimedes.Interfaces;
using Arquimedes.Utils;

namespace Arquimedes.Data
{
    public class KeyedDataRepository<T>(string path) : IDataRepository<T, string> where T : IJsonKey
    {
        private readonly Lazy<Dictionary<string, T>> _data = new(() => FileUtils.GetSourceTreeDict<T>(path));

        public IEnumerable<T> GetEnumerableCollection()
        {
            return _data.Value.Values;
        }

        public T? Query(string id)
        {
            return _data.Value.TryGetValue(id, out T? value) ? value : default;
        }
    }
}

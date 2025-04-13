using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            if (string.IsNullOrEmpty(id))
                return default;
            ref T val = ref CollectionsMarshal.GetValueRefOrNullRef(_data.Value, id);
            return Unsafe.IsNullRef(ref val) ? default : val;
        }
    }
}

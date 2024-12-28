using System.Collections;

namespace Arquimedes.DataStructures
{
    public class SwapbackArray<T>(int size = 1) : IEnumerable<T>
    {
        private T[] _array = new T[size];

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public T? GetAndRemove(int index)
        {
            T? val = _array[index];
            _array[index] = _array[^1];
            Array.Resize(ref _array, _array.Length - 1);

            return val;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}

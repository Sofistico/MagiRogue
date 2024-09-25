using System;
using System.Collections;
using System.Collections.Generic;

namespace MagusEngine.ECS
{
    public class SparseSet : IEnumerable
    {
        private int size;
        private readonly uint[] dense;
        private readonly int[] sparse;

        public int Count => size;

        public SparseSet(uint maxValue)
        {
            size = 0;
            dense = new uint[maxValue];
            sparse = new int[maxValue];
        }

        public void Add(uint value)
        {
            if (!Contains(value))
            {
                dense[size] = value;
                sparse[value] = size;
                size++;
            }
        }

        public void Remove(uint value)
        {
            if (Contains(value))
            {
                dense[sparse[value]] = dense[size - 1];
                sparse[dense[size - 1]] = sparse[value];
                size--;
            }
        }

        public int Index(uint value) => sparse[value];

        public bool Contains(uint value)
        {
            return sparse[value] < size && dense[sparse[value]] == value;
        }

        public void Clear() => size = 0;

        public IEnumerator<uint> GetEnumerator()
        {
            for (var i = 0; i < size; i++)
            {
                yield return dense[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // public override int GetHashCode() => System.HashCode.Combine(size, dense, sparse, Count);
    }
}

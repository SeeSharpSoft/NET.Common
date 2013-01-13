using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeeSharpSoft.Collections;

namespace SeeSharpSoft.Utils
{
    public class Permutations<T> : Permutations, IEnumerable<IEnumerable<T>>
    {
        T[] _elements;

        public Permutations(IEnumerable<T> collection)
            : base(collection.Count())
        {
            _elements = collection.ToArray();
        }

        #region IEnumerable<IEnumerable<T>> Members

        public new IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            using (IEnumerator<int[]> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T[] result = new T[enumerator.Current.Length];
                    for (int i = 0; i < enumerator.Current.Length; i++)
                    {
                        result[enumerator.Current[i]] = _elements[i];
                    }
                    yield return result;
                }
            }
        }

        #endregion
    }

    public class Permutations : IEnumerable<int[]>
    {
        private int _n;
        public Permutations(int n)
        {
            _n = n;
        }

        #region IEnumerable<IEnumerable<int>> Members

        public IEnumerator<int[]> GetEnumerator()
        {
            return new PermutationEnumerator(_n);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private class PermutationEnumerator : IEnumerator<int[]>
        {
            private int _n;
            public PermutationEnumerator(int n)
            {
                if (n < 0) throw new ArgumentOutOfRangeException("Only non-negative numbers allowed.");

                _n = n;

                Reset();
            }

            #region IEnumerator<IEnumerable<int>> Members

            private int[] _current;
            public int[] Current
            {
                get { return _current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _depthPositionMap.Clear();
                _depthPositionMap = null;
                _current = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                while (!TryGetNextPermutation(out _current)) ;

                if (_current == null) return false;

                return true;
            }

            public void Reset()
            {
                _current = new int[_n];

                for (int i = 0; i < _n; i++)
                {
                    _current[i] = i;
                }

                _depthPositionMap = new SortedList<int, Pair<int, int[]>>();

                _depthPositionMap.Add(0, new Pair<int, int[]>(0, _current));
            }

            #endregion

            private SortedList<int, Pair<int, int[]>> _depthPositionMap = null;

            private bool TryGetNextPermutation(out int[] permutation)
            {
                permutation = null;

                if (_depthPositionMap.Count == 0) return true; 

                int depth = _depthPositionMap.Keys[_depthPositionMap.Count - 1];

                Pair<int, int[]> pair = _depthPositionMap[depth];

                if (depth == pair.Second.Length - 1)
                {
                    permutation = pair.Second;
                    _depthPositionMap.Remove(depth);
                    return true;
                }

                if (pair.First == pair.Second.Length)
                {
                    _depthPositionMap.Remove(depth);
                }
                else
                {
                    int[] elements = pair.Second.ToArray();

                    int tmp = elements[pair.First];
                    elements[pair.First] = elements[depth];
                    elements[depth] = tmp;

                    _depthPositionMap.Add(depth + 1, new Pair<int, int[]>(depth + 1, elements));

                    pair.First++;
                }

                return false;
            }
        }
    }
}
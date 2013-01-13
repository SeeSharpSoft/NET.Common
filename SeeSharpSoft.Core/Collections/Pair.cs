using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft.Collections
{
    public class Pair<F, S> : IEquatable<Pair<F, S>>
    {
        public F First { set; get; }
        public S Second { set; get; }

        public Pair() { }
        public Pair(F first, S second)
            : this()
        {
            First = first;
            Second = second;
        }

        public override Boolean Equals(Object o)
        {
            return Equals(o as Pair<F, S>);
        }

        public override Int32 GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        #region IEquatable<Pair<F,S>> Members

        public bool Equals(Pair<F, S> other)
        {
            if (other == null)
                return false;
            else
                return Equals(First, other.First) && Equals(Second, other.Second);
        }

        #endregion
    }

    public class PairList<F, S> : List<Pair<F, S>>
    {
        public PairList() : base() {}
        public PairList(IEnumerable<Pair<F, S>> collection) : base(collection) { }

        public void Add(F first, S second)
        {
            Add(new Pair<F, S>(first, second));
        }

        public bool ContainsFirst(F first)
        {
            return this.FirstOrDefault(elem => elem.First.Equals(first)) != null;
        }

        public bool ContainsSecond(S second)
        {
            return this.FirstOrDefault(elem => elem.Second.Equals(second)) != null;
        }

        public S this[F first]
        {
            get
            {
                Pair<F, S> element = this.FirstOrDefault(elem => elem.First.Equals(first));

                return element == null ? default(S) : element.Second;
            }
            set
            {
                Pair<F, S> element = this.FirstOrDefault(elem => elem.First.Equals(first));

                if (element == null) this.Add(new Pair<F, S>(first, value));
                else element.Second = value;
            }
        }
    }
}
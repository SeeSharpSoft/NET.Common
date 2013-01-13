using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace SeeSharpSoft.Collections
{
    public class GenericHashtable<K, V> : Hashtable, IEnumerable<KeyValuePair<K, V>>
    {
        public GenericHashtable() : base() { }
        public GenericHashtable(int capacity) : base(capacity) { }

        public new void Add(object key, object value)
        {
            if (key is K && value is V) base.Add(key, value);
            else throw new ArgumentException("Key must be type of K and value must be type of V.");
        }

        public virtual void Add(K key, V value)
        {
            base.Add(key, value);
        }

        public V this[K key]
        {
            get
            {
                return (V)base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public new object this[object key]
        {
            get { if (key is K) return base[key]; return null; }
            set { if (key is K && value is V) base[key] = value; else throw new ArgumentException("Key must be type of K and value must be type of V."); }
        }

        public new IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            IDictionaryEnumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
                yield return new KeyValuePair<K, V>((K)enumerator.Key, (V)enumerator.Value);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<K, V> elem in this)
            {
                result.AppendFormat("[{0}; {1}]\r\n", elem.Key, elem.Value);
            }
            return result.ToString();
        }
    }

    public class HashIndexedList<T> : GenericHashtable<int, T>, IList<T>
    {
        public HashIndexedList() : base() { }
        public HashIndexedList(int capacity) : base(capacity) { }

        #region IList<T> Members
        public int IndexOf(T item)
        {
            return item.GetHashCode();
        }
        public void Insert(int hashCode, T item)
        {
            base.Add(hashCode, item);
        }
        public void RemoveAt(int hashCode)
        {
            base.Remove(hashCode);
        }
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T elem in collection)
            {
                Add(elem);
            }
        }
        #endregion

        #region ICollection<T> Members
        public void Add(T item)
        {
            base.Add(item.GetHashCode(), item);
        }
        public bool Contains(T item)
        {
            return base.Contains(item.GetHashCode());
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }
        public bool Remove(T item)
        {
            if (!Contains(item)) return false;
            base.Remove(item.GetHashCode());
            return true;
        }
        #endregion

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (T item in base.Values)
            {
                yield return item;
            }
        }

        #endregion
    }

    public class HashDictionary<K, V> : GenericHashtable<K, V>, IDictionary<K, V>
    {
        public HashDictionary() : base() { }
        public HashDictionary(int capacity) : base(capacity) { }

        public bool ContainsKey(K key)
        {
            return base.ContainsKey(key);
        }

        public new ICollection<K> Keys
        {
            get { return (ICollection<K>)base.Keys; }
        }

        public bool Remove(K key)
        {
            if (!ContainsKey(key)) return false;

            base.Remove(key);
            return true;
        }

        public bool TryGetValue(K key, out V value)
        {
            if (!this.ContainsKey(key))
            {
                value = default(V);
                return false;
            }

            value = (V)base[key];
            return true;
        }

        public new ICollection<V> Values
        {
            get { return (ICollection<V>)base.Values; }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            V value = this[item.Key];
            if (value.Equals(item.Value)) return true;
            return false;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<K, V> elem in this)
            {
                array[arrayIndex] = elem;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            V value = this[item.Key];
            if (value.Equals(item.Value)) return Remove(item.Key);
            return false;
        }
    }

    public class GenericOrderedDictionary<K, V> : OrderedDictionary, IEnumerable<KeyValuePair<K, V>>, IDictionary<K, V>
    {
        public GenericOrderedDictionary() : base() { }
        public GenericOrderedDictionary(int capacity) : base(capacity) { }
        public GenericOrderedDictionary(IDictionary<K, V> dict) : base()
        {
            this.AddRange(dict);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void Add(object key, object value)
        {
            Add((K)key, (V)value);
        }

        public virtual void Add(K key, V value)
        {
            base.Add(key, value);
        }

        public V this[K key]
        {
            get
            {
                return (V)base[(object)key];
            }
            set
            {
                base[(object)key] = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new object this[object key]
        {
            get { return this[(K)key]; }
            set { this[(K)key] = (V)value; }
        }

        public new IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            IDictionaryEnumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
                yield return new KeyValuePair<K, V>((K)enumerator.Key, (V)enumerator.Value);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (KeyValuePair<K, V> elem in this)
            {
                result.AppendFormat("[{0}; {1}]\r\n", elem.Key, elem.Value);
            }
            return result.ToString();
        }

        #region IDictionary<K,V> Members

        void IDictionary<K, V>.Add(K key, V value)
        {
            Add(key, value);
        }

        bool IDictionary<K, V>.ContainsKey(K key)
        {
            return Contains(key);
        }

        ICollection<K> IDictionary<K, V>.Keys
        {
            get { return Keys.OfType<K>().ToList(); }
        }

        bool IDictionary<K, V>.Remove(K key)
        {
            try
            {
                Remove(key);
            }
            catch
            {
                return false;
            }
            return true;
        }

        bool IDictionary<K, V>.TryGetValue(K key, out V value)
        {
            if (Contains(key))
            {
                value = this[key];
                return true;
            }
            value = default(V);
            return false;
        }

        ICollection<V> IDictionary<K, V>.Values
        {
            get { return Values.OfType<V>().ToList(); }
        }

        #endregion

        #region ICollection<KeyValuePair<K,V>> Members

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            return this.Contains(item);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            return ((IDictionary<K, V>)this).Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Members

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
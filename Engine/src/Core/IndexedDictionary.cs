using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.src.Core
{
    // O(n) lookup but 99% of the time we will just loop through it, not lookup using the key.
    public class IndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private List<KeyValuePair<TKey, TValue>> _list = new();


        public TValue this[TKey key]
        {
            get
            {
                foreach (var item in _list)
                {
                    if (item.Key.Equals(key))
                        return item.Value;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    var item = _list[i];

                    if (item.Key.Equals(key))
                    {
                        _list[i] = new(key, value);
                        return;
                    }
                }

                _list.Add(new (key, value));
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                foreach (var item in _list)
                {
                    keys.Add(item.Key);
                }
                return keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                foreach (var item in _list)
                {
                    values.Add(item.Value);
                }
                return values;
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _list.Contains(item);
        public bool ContainsKey(TKey key) => Keys.Contains(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>  _list.GetEnumerator();

        public bool Remove(TKey key)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list[i];

                if (item.Key.Equals(key))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => _list.Remove(item);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list[i];

                if (item.Key.Equals(key))
                {
                    value = item.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}

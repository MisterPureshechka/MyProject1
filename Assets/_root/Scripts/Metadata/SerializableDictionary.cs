using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Metadata
{

    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [Serializable]
        public class KeyValuePair
        {
            [SerializeField] private TKey _key;
            [SerializeField] private TValue _value;

            public TKey Key
            {
                get => _key;
                set => _key = value;
            }

            public TValue Value
            {
                get => _value;
                set => _value = value;
            }

            public KeyValuePair(TKey key, TValue value)
            {
                _key = key;
                _value = value;
            }
        }

        [SerializeField] private List<KeyValuePair> _items = new List<KeyValuePair>();

        public List<KeyValuePair> Items
        {
            get => _items;
            set => _items = value;
        }

        // Индексатор для доступа по ключу
        public TValue this[TKey key]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
                    {
                        return item.Value;
                    }
                }
                throw new KeyNotFoundException($"Key {key} not found.");
            }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(Items[i].Key, key))
                    {
                        Items[i].Value = value;
                        return;
                    }
                }
                Items.Add(new KeyValuePair(key, value));
            }
        }

        // Добавление элемента
        public void Add(TKey key, TValue value)
        {
            Items.Add(new KeyValuePair(key, value));
        }

        // Получение элемента
        public TValue Get(TKey key)
        {
            return this[key]; // Используем индексатор
        }

        // Попытка получить элемент
        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var item in Items)
            {
                if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
                {
                    value = item.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }
    }
}
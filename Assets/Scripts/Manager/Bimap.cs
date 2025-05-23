using System.Collections.Generic;

public class BiMap<K, V>
{
    private Dictionary<K, V> keyToValue = new Dictionary<K, V>();
    private Dictionary<V, K> valueToKey = new Dictionary<V, K>();

    public void Add(K key, V value)
    {
        if (keyToValue.ContainsKey(key) || valueToKey.ContainsKey(value))
            throw new System.ArgumentException("Duplicate key or value detected.");

        keyToValue[key] = value;
        valueToKey[value] = key;
    }

    public bool IsValiedKey(K key)
    {
        return keyToValue.ContainsKey(key);
    }

    public bool IsValiedValue(V value)
    {
        return valueToKey.ContainsKey(value);
    }

    public V GetValue(K key)
    {
        return keyToValue.TryGetValue(key, out var value) ? value : default;
    }

    public K GetKey(V value)
    {
        return valueToKey.TryGetValue(value, out var key) ? key : default;
    }

    public bool RemoveByKey(K key)
    {
        if (keyToValue.TryGetValue(key, out var value))
        {
            keyToValue.Remove(key);
            valueToKey.Remove(value);
            return true;
        }
        return false;
    }

    public bool RemoveByValue(V value)
    {
        if (valueToKey.TryGetValue(value, out var key))
        {
            valueToKey.Remove(value);
            keyToValue.Remove(key);
            return true;
        }
        return false;
    }
}
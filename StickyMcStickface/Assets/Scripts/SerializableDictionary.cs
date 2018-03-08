using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<K, V>
{
    [SerializeField]
    private List<K> _keys;
    [SerializeField]
    private List<V> _values;

    private Dictionary<K, V> _dict = new Dictionary<K, V>();

    public SerializableDictionary(List<K> keys, List<V> values)
    {
        if (keys.Count != values.Count)
            throw new ArgumentException();
        _keys = keys;
        _values = values;

        for (int i = 0; i < _keys.Count; i++)
            _dict.Add(_keys[i], _values[i]);
    }

    public Dictionary<K, V> Dict
    {
        get { return _dict; }
    }
}
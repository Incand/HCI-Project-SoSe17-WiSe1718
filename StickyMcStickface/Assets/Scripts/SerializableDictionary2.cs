using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary2<K, V>
{
    [SerializeField]
    private K[] _keys;
    [SerializeField]
    private V[] _values;

    public SerializableDictionary2() {}

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        if (_keys.Length != _values.Length)
            throw new ArgumentException();

        for (int i = 0; i < _keys.Length; i++)
            yield return new KeyValuePair<K, V>(_keys[i], _values[i]);
    }
}
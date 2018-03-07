using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceDictionary : Dictionary<string, Resource>, ISerializationCallbackReceiver 
{
    [SerializeField]
    private List<Resource> _values = new List<Resource> ();

    #region INTERFACE_IMPLEMENTAION

    // save the dictionary to lists
    public void OnBeforeSerialize ()
    {
        _values.Clear ();
        foreach (Resource resource in this.Values) {
            _values.Add (resource);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize ()
    {
        this.Clear ();

        for (int i = 0; i < _values.Count; i++) {
            // Stupid hacky shit...
            if (_values [i].Name.Equals ("") || this.ContainsKey (_values [i].Name))
                _values [i] = new Resource (GetResourceNameByIndex (i), 0.0f);

            Add (_values [i]);
        }
    }

    #endregion


    private string GetResourceNameByIndex (int i)
    {
        switch (i) {
        case 0: return "Energy";
        case 1: return "Squirrelium";
        case 2: return "Calculation Power";
        default: return i.ToString ();
        }
    }


    public void Add (Resource resource)
    {
        this.Add (resource.Name, resource);
    }

}

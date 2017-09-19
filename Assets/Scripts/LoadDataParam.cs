using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class LoadDataParam
{
    [SerializeField]
    public List<string> paramKeys;

    [SerializeField]
    public List<string> paramTypes;

    [SerializeField]
    public List<string> paramValues;

    [SerializeField]
    public List<string> Types;

    public LoadDataParam()
    {
        this.paramKeys   = new List<string>();
        this.paramTypes  = new List<string>();
        this.paramValues = new List<string>();
        this.Types = new List<string>();
    }

    public void addValue(string key, string type, string value)
    {
        Debug.Log(key);
        this.paramKeys.Add(key);
        this.paramTypes.Add(type);
        this.paramValues.Add(value);
    }



    public bool hasKey(string key)
    {
        int indexOf = this.paramKeys.FindIndex(x => x.Equals(key,StringComparison.OrdinalIgnoreCase));

        if (indexOf != -1)
        {
            return true;
        }

        return false;
    }


    public string getValue(string key)
    {
        int indexOf = this.paramKeys.FindIndex(x => x.Equals(key,StringComparison.OrdinalIgnoreCase));

        return this.paramValues[indexOf];
    }

    public string getType(string key)
    {
        int indexOf = this.paramKeys.FindIndex(x => x.Equals(key,StringComparison.OrdinalIgnoreCase));

        return this.paramTypes[indexOf];
    }
}

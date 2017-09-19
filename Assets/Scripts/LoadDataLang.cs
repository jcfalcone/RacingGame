using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class LoadDataLang
{
    [SerializeField]
    public List<string> translationsKey;

    [SerializeField]
    public List<string> translationsText;

    public LoadDataLang()
    {
        this.translationsKey    = new List<string>();
        this.translationsText   = new List<string>();
    }

    public void addValue(string key, string text)
    {
        this.translationsKey.Add(key);
        this.translationsText.Add(text);
    }

    public string tr(string key, string defaltText)
    {
        //int indexOf = this.translationsKey.IndexOf(key);
        int indexOf = this.translationsKey.FindIndex(x => x.Equals(key,StringComparison.OrdinalIgnoreCase));

        if (indexOf == -1)
        {
            return defaltText;
        }

        return this.translationsText[indexOf];
    }
}

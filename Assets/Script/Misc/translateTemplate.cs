using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class translateTemplate : MonoBehaviour {
    
    [SerializeField]
    protected string TranslationKey;

    abstract public void UpdateTxt();
}

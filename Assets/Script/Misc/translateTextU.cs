using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class translateTextU : translateTemplate 
{
    Text curText;

	// Use this for initialization
	void Awake () 
    {
        this.curText = GetComponent<Text>();
        this.curText.text = LoadDataManager.instance.currTranslations.tr(TranslationKey, this.curText.text);
    }

    override public void UpdateTxt()
    {
        this.curText = GetComponent<Text>();
        this.curText.text = LoadDataManager.instance.currTranslations.tr(TranslationKey, this.curText.text);
    }
}

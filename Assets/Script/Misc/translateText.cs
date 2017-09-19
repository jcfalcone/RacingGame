using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class translateText : translateTemplate 
{

    TextMeshProUGUI curText;

	// Use this for initialization
	void Awake () 
    {
        this.curText = GetComponent<TextMeshProUGUI>();
        this.curText.text = LoadDataManager.instance.currTranslations.tr(TranslationKey, this.curText.text);
	}

    override public void UpdateTxt()
    {
        this.curText = GetComponent<TextMeshProUGUI>();
        this.curText.text = LoadDataManager.instance.currTranslations.tr(TranslationKey, this.curText.text);
    }
}

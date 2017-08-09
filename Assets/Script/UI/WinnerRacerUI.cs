using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerRacerUI : MonoBehaviour 
{
    RectTransform uiTransform;
    CanvasGroup   uiGroup;

    bool moveUI = true;

    Vector2 originalPos;

	// Use this for initialization
	void Start () 
    {
        this.uiTransform = GetComponent<RectTransform>();
        this.uiGroup = GetComponent<CanvasGroup>();

        this.originalPos = this.uiTransform.anchoredPosition;

        Vector2 uiPos = this.originalPos;
        uiPos.x += 300;

        this.uiTransform.anchoredPosition = uiPos;


        this.uiGroup.alpha = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (this.moveUI)
        {
            this.uiTransform.anchoredPosition = Vector2.Lerp(this.uiTransform.anchoredPosition, this.originalPos, Time.unscaledDeltaTime * 6);
            this.uiGroup.alpha += Time.unscaledDeltaTime * 2;

            if (this.uiGroup.alpha >= 1)
            {
                this.moveUI = false;
                this.uiTransform.anchoredPosition = this.originalPos;
            }
        }
	}
}

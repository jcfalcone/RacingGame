using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class tutorialSkipBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{

    [SerializeField]
    Image skipProgressImg;

    [SerializeField]
    TutorialManager manager;

    bool pressed = false;
    bool finished = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        this.pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.pressed = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!this.finished)
        {
            if (this.pressed)
            {
                this.skipProgressImg.fillAmount = Mathf.Clamp(this.skipProgressImg.fillAmount + (Time.unscaledDeltaTime * 0.75f), 0.3f, 1f);
            }
            else
            {
                this.skipProgressImg.fillAmount = Mathf.Clamp(this.skipProgressImg.fillAmount - (Time.unscaledDeltaTime * 0.75f), 0.3f, 1f);
            }

            if (this.skipProgressImg.fillAmount >= 1f)
            {
                this.finished = true;
                this.manager.endTutorial();

                Destroy(this);
            }
        }
	}
}

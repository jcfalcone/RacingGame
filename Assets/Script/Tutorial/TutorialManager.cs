using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour 
{
    [SerializeField]
    CanvasGroup generalGroup;

    [SerializeField]
    RectTransform overlayMask;

    [SerializeField]
    Image overlay;

    [SerializeField]
    TutorialHint[] tutorialHints;

    [SerializeField]
    GameObject playerItem;

    int currHint = 0;
    int currSubHint = 0;

    bool startSubHint = false;

    bool canClick = true;

    float timeHint;
    bool startTimeHint = false;

    float timeSubHint;
    bool startTimeSubHint = false;

    float timeClick;
    bool startTimeClick = false;

	// Use this for initialization
	void Start () 
    {
        this.generalGroup.alpha = 0;
        this.tutorialHints[this.currHint].hintGroup.alpha = 0;
        this.overlayMask.gameObject.SetActive(false);

        for (int count = 0; count < this.tutorialHints[this.currHint].subHintList.Length; count++)
        {
            this.tutorialHints[this.currHint].subHintList[count].alpha = 0;
        }

        ControlMaster.instance.PauseGame();
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (this.getInput() && this.canClick)
        {
            this.forceMoveNextHint();
            this.canClick = false;

            this.timeClick = 0;
            this.startTimeClick = true;

            return;
        }

        if (this.startTimeClick)
        {
            this.timeClick += Time.unscaledDeltaTime;

            if (this.timeClick > 0.2f)
            {
                this.startTimeClick = false;
                this.canClick = true;
            }
        }

        if (this.startTimeHint)
        {
            this.timeHint += Time.unscaledDeltaTime;

            if (this.timeHint > this.tutorialHints[this.currHint].waitTime)
            {
                this.startTimeHint = false;
                this.startSubHint = true;
            }
        }

        if (this.startTimeSubHint)
        {
            this.timeSubHint += Time.unscaledDeltaTime;

            if (this.timeSubHint > this.tutorialHints[this.currHint].subHintWaitTime[this.currSubHint])
            {
                this.startTimeSubHint = false;
                this.moveNextSubHint();
            }
        }

        if (this.currHint >= this.tutorialHints.Length)
        {
            this.generalGroup.alpha -= Time.unscaledDeltaTime;

            if (this.generalGroup.alpha <= 0)
            {
                Destroy(gameObject);
                UIManager.instance.ShowStartNumber();

                this.playerItem.SetActive(false);
            }
            return;
        }

        if (this.generalGroup.alpha < 1)
        {
            this.generalGroup.alpha += Time.unscaledDeltaTime;

            return;
        }

        if (this.currHint > 0 && this.tutorialHints[this.currHint - 1].hintGroup.alpha != 0)
        {
            this.tutorialHints[this.currHint - 1].hintGroup.alpha -= Time.unscaledDeltaTime;

            if (this.tutorialHints[this.currHint - 1].hintGroup.alpha <= 0)
            {
                this.tutorialHints[this.currHint - 1].hint.SetActive(false);
                this.tutorialHints[this.currHint].hint.SetActive(true);
                this.tutorialHints[this.currHint].hintGroup.alpha = 0;
            }
        }
        else
        {

            this.playerItem.SetActive(true);

            if (this.tutorialHints[this.currHint].useOverlay)
            {
                this.overlayMask.anchoredPosition = Vector2.Lerp(this.overlayMask.anchoredPosition, this.tutorialHints[this.currHint].overlayPos, Time.unscaledDeltaTime * 2f);
                this.overlayMask.localScale = Vector3.Lerp(this.overlayMask.localScale, this.tutorialHints[this.currHint].overlayScale, Time.unscaledDeltaTime * 2f);
            }

            if (this.tutorialHints[this.currHint].hintGroup.alpha < 1)
            {
                this.tutorialHints[this.currHint].hintGroup.alpha += Time.unscaledDeltaTime;

                if(this.tutorialHints[this.currHint].hintGroup.alpha >= 1)
                {
                    this.startTimeHint = true;
                    this.timeHint = 0;
                    //StartCoroutine(waitHint());
                }
            }

            if (this.startSubHint)
            {
                if (this.tutorialHints[this.currHint].hasSubHints)
                {
                    this.tutorialHints[this.currHint].subHintList[this.currSubHint].alpha += Time.unscaledDeltaTime;

                    if (this.tutorialHints[this.currHint].subHintList[this.currSubHint].alpha >= 1)
                    {
                        this.startSubHint = false;
                        this.startTimeSubHint = true;
                        this.timeSubHint = 0;
                        //StartCoroutine(waitNextSubHint());
                    }
                }
                else
                {
                    this.moveNextHint();
                }
            }
        }
    }

    void moveNextSubHint()
    {

        this.currSubHint++;
        this.startSubHint = true;

        if (this.currSubHint >= this.tutorialHints[this.currHint].subHintList.Length)
        {
            this.moveNextHint();
        }
    }

    void moveNextHint()
    {
        this.currHint++;
        this.currSubHint = 0;
        this.startSubHint = false;

        if (this.currHint < this.tutorialHints.Length)
        {
            this.overlayMask.gameObject.SetActive(this.tutorialHints[this.currHint].useOverlay);
        }
    }

    void forceMoveNextHint()
    {
        if (this.startTimeHint)
        {
            this.timeHint = this.tutorialHints[this.currHint].waitTime;

            return;
        }

        if (this.startTimeSubHint)
        {
            this.timeSubHint = this.tutorialHints[this.currHint].subHintWaitTime[this.currSubHint];

            return;
        }

        if (this.currHint > 0 && this.tutorialHints[this.currHint - 1].hintGroup.alpha != 0 && this.currHint < this.tutorialHints.Length)
        {
            this.tutorialHints[this.currHint - 1].hint.SetActive(false);
            this.tutorialHints[this.currHint].hint.SetActive(true);
            this.tutorialHints[this.currHint].hintGroup.alpha = 0;

            return;
        }

        if (this.currHint < this.tutorialHints.Length)
        {
            if (this.tutorialHints[this.currHint].hintGroup.alpha != 1)
            {
                this.tutorialHints[this.currHint].hintGroup.alpha = 1;
                return;
            }

            if (this.tutorialHints[this.currHint].hasSubHints)
            {
                if (this.tutorialHints[this.currHint].subHintList[this.currSubHint].alpha != 1)
                {
                    this.tutorialHints[this.currHint].subHintList[this.currSubHint].alpha = 1;
                    return;
                }
                else
                {
                    this.moveNextSubHint();
                    return;
                }
            }

            this.currSubHint++;
            this.startSubHint = true;

            if (this.currSubHint >= this.tutorialHints[this.currHint].subHintList.Length)
            {
                this.moveNextHint();

                if (this.currHint > 0 && this.tutorialHints[this.currHint - 1].hintGroup.alpha != 0)
                {
                    this.tutorialHints[this.currHint - 1].hintGroup.alpha = 0;
                    this.tutorialHints[this.currHint - 1].hint.SetActive(false);

                    if (this.currHint < this.tutorialHints.Length - 1)
                    {
                        this.tutorialHints[this.currHint].hint.SetActive(true);
                        this.tutorialHints[this.currHint].hintGroup.alpha = 0;
                    }

                    return;
                }
            }
        }
    }

    IEnumerator waitNextSubHint()
    {
        this.startSubHint = false;

        yield return new WaitForSeconds(this.tutorialHints[this.currHint].subHintWaitTime[this.currSubHint]);

        this.moveNextSubHint();
    }

    IEnumerator waitHint()
    {
        yield return new WaitForSeconds(this.tutorialHints[this.currHint].waitTime);

        startSubHint = true;
    }

    IEnumerator clickCoolDown()
    {
        yield return new WaitForSeconds(0.2f);

        this.canClick = true;
    }

    bool getInput()
    {
        #if UNITY_EDITOR
        if(Input.anyKey)
        {
            return true;
        }
        #elif UNITY_IOS || UNITY_ANDROID
        if(Input.touchCount > 0)
        {
            return true;
        }
        #else
        if(Input.anyKey)
        {
            return true;
        }
        #endif

        return false;
    }

    public void endTutorial()
    {
        this.currHint = this.tutorialHints.Length;
        this.startTimeClick = false;
        this.startTimeHint = false;
        this.startTimeSubHint = false;
    }
}

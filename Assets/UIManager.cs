using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class UIManager : MonoBehaviour 
{
    public static UIManager instance
    {
        get { return _instance; }//can also use just get;
        set { _instance = value; }//can also use just set;
    }

    //Creates a class variable to keep track of GameManger
    static UIManager _instance = null;


    [SerializeField]
    GameObject[] startCanvas;

    [SerializeField]
    PlayableDirector timeline;

    [SerializeField]
    Text[] startRaceNumbers;
    RectTransform[] startRaceNumbersRect;

    bool showNumbers = false;
    bool checkDirector = true;

    int countNumbers = 0;

    Vector3 vectorOne = Vector3.one;

	// Use this for initialization
	void Start () 
    {
        Vector3 newScale = new Vector3(1.5f, 1.5f, 1.5f);

        this.startRaceNumbersRect = new RectTransform[this.startRaceNumbers.Length];

        for (int count = 0; count < this.startRaceNumbers.Length; count++)
        {
            Color tempColor = this.startRaceNumbers[count].color;
            tempColor.a     = 0f;
            this.startRaceNumbers[count].color = tempColor;

            this.startRaceNumbersRect[count] = this.startRaceNumbers[count].gameObject.GetComponent<RectTransform>();
            this.startRaceNumbersRect[count].localScale = newScale;
        }

        ControlMaster.instance.PauseGame();

        //check if GameManager instance already exists in Scene
        if(instance)
        {
            //GameManager exists,delete copy
            DestroyImmediate(gameObject);
        }
        else
        {
            //assign GameManager to variable "_instance"
            instance = this;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (this.showNumbers)
        {
            Color tempColor = this.startRaceNumbers[countNumbers].color;
            Vector3 tempScale = this.startRaceNumbersRect[countNumbers].localScale;

            tempColor.a += Time.unscaledDeltaTime * 1.3f;

            this.startRaceNumbers[countNumbers].color = tempColor;
            this.startRaceNumbersRect[countNumbers].localScale = Vector3.Lerp(tempScale, this.vectorOne, Time.unscaledDeltaTime * 1.3f);

            if (this.startRaceNumbers[countNumbers].color.a >= 1f)
            {
                this.startRaceNumbers[countNumbers].gameObject.SetActive(false);
                this.countNumbers++;

                if (this.countNumbers >= this.startRaceNumbers.Length)
                {
                    this.showNumbers = false;
                    ControlMaster.instance.UnPauseGame();

                    for (int count = 0; count < this.startCanvas.Length; count++)
                    {
                        this.startCanvas[count].SetActive(true);
                    }
                }
            }
        }

        if (this.checkDirector)
        {
            if (timeline.state == UnityEngine.Playables.PlayState.Paused)
            {
                this.showNumbers = true;
                this.checkDirector = false;
            }
        }
	}

    public void ShowStartNumber()
    {
        this.showNumbers = true; 
    }
}

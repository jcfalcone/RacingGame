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
    Text lapNumber;

    [SerializeField]
    GameObject[] startCanvas;

    [SerializeField]
    PlayableDirector timeline;

    [SerializeField]
    Text[] startRaceNumbers;
    RectTransform[] startRaceNumbersRect;

    [Header("Final")]
    [SerializeField]
    GameObject finalCanvas;

    [SerializeField]
    GameObject finalUIPrefab;

    [SerializeField]
    GameObject canvasRacerGroup;

    [SerializeField]
    int distanceFinalUI;

    [SerializeField]
    Color currPlayerColor;

    bool showNumbers = false;
    bool checkDirector = true;

    int countNumbers = 0;
    int countRacerEnd = 1;

    Vector3 vectorOne = Vector3.one;

    AsyncOperation async;

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
                this.timeline.gameObject.SetActive(false);
            }
        }
	}

    public void ShowStartNumber()
    {
        this.showNumbers = true; 
    }

    public void UpdateLapNumber(int lap, int totalLap)
    {
        this.lapNumber.text = string.Format("LAP {0} of {1}", lap, totalLap);
    }

    public void OnClickMap(string mapToLoad)
    {
        StartCoroutine(startLoadLevel(mapToLoad));
        StartCoroutine(checkProgress());
    }

    IEnumerator startLoadLevel(string mapToLoad)
    {
        this.async = Application.LoadLevelAsync(mapToLoad);
        this.async.allowSceneActivation = false;

        yield return async;
    }

    IEnumerator checkProgress()
    {
        while (this.async.progress < 0.9f)
        {
            yield return null;
        }

        this.async.allowSceneActivation = true;
    }

    public void nextLevel()
    {
        async.allowSceneActivation = true;
    }

    public void showFinal()
    {
        for (int count = 0; count < this.startCanvas.Length; count++)
        {
            this.startCanvas[count].SetActive(false);
        }

        finalCanvas.SetActive(true);
    }

    public void addRacerFinal(string racerName, Sprite racerPicture, int totalPoints, bool localPlayer)
    {
        GameObject racerObj = Instantiate(this.finalUIPrefab, this.canvasRacerGroup.transform);
        RectTransform racerTransform = racerObj.GetComponent<RectTransform>();
        Vector2 anchorPos = racerTransform.anchoredPosition;

        anchorPos.y -= (countRacerEnd - 1) * distanceFinalUI;

        racerTransform.anchoredPosition = anchorPos;

        if (localPlayer)
        {
            Image raceObjBack = racerObj.GetComponent<Image>();
            raceObjBack.color       = currPlayerColor;
        }

        Text racerObjPos    = racerObj.transform.Find("RacerPos").GetComponent<Text>();
        Image racerObjImage = racerObj.transform.Find("RacerImage").GetComponent<Image>();
        Text racerObjName   = racerObj.transform.Find("RacerName").GetComponent<Text>();
        Text racerObjPoints = racerObj.transform.Find("RacingPoints").GetComponent<Text>();

        racerObjPos.text        = countRacerEnd.ToString();
        racerObjImage.sprite    = racerPicture;
        racerObjName.text       = racerName;
        racerObjPoints.text     = "+" + totalPoints;

        countRacerEnd++;

    }
}

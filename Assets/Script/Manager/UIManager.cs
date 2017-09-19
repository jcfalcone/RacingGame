using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

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
    AudioSource startSound;

    [SerializeField]
    PlayableDirector timeline;

    [SerializeField]
    Text[] startRaceNumbers;
    RectTransform[] startRaceNumbersRect;

    [Header("Pause")]
    [SerializeField]
    GameObject pauseUI;

    [SerializeField]
    GameObject pauseUIBackground;

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

    [Header("Final Exp Bar")]
    [SerializeField]
    TextMeshProUGUI prevLevelLbl;

    [SerializeField]
    TextMeshProUGUI currLevelLbl;

    [SerializeField]
    Image expBar;

    bool updateExpBar = false;
    int prevLvl;
    int prevExp;

    int lvlRequiredExp = -1;
    float destPerc = -1;

    int countLvl;

    int currExp;

    bool showNumbers = false;

    [SerializeField]
    bool checkDirector = true;

    int countNumbers = 0;
    int countRacerEnd = 1;

    Vector3 vectorOne = Vector3.one;

    string lapDefaultTxt;

    AsyncOperation async;

	// Use this for initialization
	void Start () 
    {


        this.updatePlayerExpBar(0, 25, 1, 3);

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

        this.lapDefaultTxt = this.lapNumber.text;

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

            tempColor.a += Time.unscaledDeltaTime * 1.1f;

            this.startRaceNumbers[countNumbers].color = tempColor;
            this.startRaceNumbersRect[countNumbers].localScale = Vector3.Lerp(tempScale, this.vectorOne, Time.unscaledDeltaTime * 1.1f);

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
                this.startSound.Play();
            }
        }

        if (this.updateExpBar)
        {
            if (this.countLvl > 0)
            {
                this.expBar.fillAmount = Mathf.Lerp(this.expBar.fillAmount, 1f, Time.deltaTime);

                if (this.expBar.fillAmount >= 0.95f)
                {

                    this.prevLvl++;

                    this.expBar.fillAmount = 0;
                    this.currLevelLbl.text = (prevLvl + 2).ToString();
                    this.prevLevelLbl.text = (prevLvl + 1).ToString();

                    //this.expBar.fillAmount = 1f;
                    this.countLvl--;
                }
            }
            else
            {
                if (this.lvlRequiredExp == -1)
                {
                    this.lvlRequiredExp = PlayerData.instance.requiredLevelExp(this.prevLvl + 1);
                    this.destPerc = (float)this.currExp / (float)this.lvlRequiredExp;
                }

                this.expBar.fillAmount = Mathf.Lerp(this.expBar.fillAmount, this.destPerc, Time.deltaTime);
            }
        }
	}

    public void ShowStartNumber()
    {
        this.showNumbers = true;
        this.checkDirector = false;
        this.timeline.gameObject.SetActive(false);
        this.startSound.Play();

    }

    public void UpdateLapNumber(int lap, int totalLap)
    {
        string newLap = this.lapDefaultTxt;
        newLap = newLap.Replace("[CURRLAP]", lap.ToString());
        newLap = newLap.Replace("[TOTALLAP]", totalLap.ToString());
        this.lapNumber.text = string.Format(newLap, lap, totalLap);
    }

    public void OnClickMap(int mapToLoad)
    {
		AudioTest.instance.crossFadeLevelMusic (mapToLoad);
        StartCoroutine(startLoadLevel(mapToLoad +1));
        StartCoroutine(checkProgress());
    }

    IEnumerator startLoadLevel(int mapToLoad)
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
        Time.timeScale = 1f;
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

    public void Pause()
    {
        if (ControlMaster.instance.paused)
        {
            ControlMaster.instance.UnPauseGame();
            this.pauseUI.SetActive(false);
            this.pauseUIBackground.SetActive(false);
        }
        else
        {
            ControlMaster.instance.PauseGame();
            this.pauseUI.SetActive(true);
            this.pauseUIBackground.SetActive(true);
        }
    }

    public void updatePlayerExpBar(int prevExp, int currExp, int prevLvl, int currLvl)
    {
        this.prevExp = prevExp;
        this.prevLvl = prevLvl;
        this.currExp = currExp;
        this.lvlRequiredExp = -1;

        this.countLvl = currLvl - this.prevLvl;
        this.updateExpBar = true;

        this.currLevelLbl.text = (prevLvl + 2).ToString();
        this.prevLevelLbl.text = (prevLvl + 1).ToString();

        this.expBar.fillAmount = prevExp / PlayerData.instance.requiredLevelExp(this.prevLvl + 1);
    }
}

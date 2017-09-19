using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerMenu : MonoBehaviour 
{
    public static UIManagerMenu instance
    {
        get { return _instance; }//can also use just get;
        set { _instance = value; }//can also use just set;
    }

    //Creates a class variable to keep track of GameManger
    static UIManagerMenu _instance = null;

    [SerializeField]
    Animator camAnim;

    [SerializeField]
    Animator mainMenuAnimator;

    [SerializeField]
    GameObject touchCanvas;

    [SerializeField]
    Text SFXSound;

    [SerializeField]
    Text MUSSound;

    [Header("Character")]
    [SerializeField]
    KartSelection charDisplay;

    [Header("Loading")]
    [SerializeField]
    CanvasGroup loadingGroup;

    [SerializeField]
    Image loadingImg;

    [SerializeField]
    translateTemplate[] textElements;

    bool startGame = false;
    bool startLoading = false;
    bool hideLoading = true;

    int levelToLoad = 0;

    AsyncOperation async = null;

	// Use this for initialization
	void Awake () 
    {
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
        if (!startGame)
        {
            if (Input.touchCount > 0 || Input.GetButtonUp("Fire1"))
            {
                this.touchCanvas.SetActive(false);
                this.camAnim.enabled = true;
                this.camAnim.CrossFade("CameraStart", 0f);
                this.startGame = true;

            }
        }

        if (this.startLoading)
        {
            if (this.loadingGroup.alpha < 1)
            {
                this.loadingGroup.gameObject.SetActive(true);
                this.loadingGroup.alpha += Time.deltaTime;
            }

        }
        else
        {
            if (this.loadingGroup.alpha > 0)
            {
                this.loadingGroup.alpha -= Time.deltaTime;
            }
            else
            {
                this.loadingGroup.gameObject.SetActive(false);
            }
        }

        if (this.async != null)
        {
            if (this.loadingGroup.alpha >= 1)
            {
                if (this.async.progress >= 0.9f)
                {
                    this.hideLoading = true;
                }
            }

            if (this.hideLoading)
            {
                Color loadingColor = this.loadingImg.color;
                loadingColor.a -= Time.deltaTime;
                this.loadingImg.color = loadingColor;

                if (loadingColor.a <= 0)
                {
                    this.async.allowSceneActivation = true;
                }
            }
        }
	}

    public void OnClickSetting()
    {
        this.camAnim.CrossFade("CameraSettings", 0f);
    }

    public void OnClickSaveSetting()
    {
        this.camAnim.CrossFade("CameraSettingToStart", 0f);
    }

    public void OnClickNewGame()
    {
        this.mainMenuAnimator.enabled = true;
        this.mainMenuAnimator.CrossFade("MainMenuInitial", 0f);
    }

    public void OnClickMap(int mapToLoad)
	{

        this.levelToLoad = mapToLoad;
        this.camAnim.CrossFade("CameraSelectCar", 0f);
        this.charDisplay.gameObject.SetActive(true);
		/*AudioTest.instance.crossFadeLevelMusic (mapToLoad);
        StartCoroutine(startLoadLevel(mapToLoad + 1));
        this.loading(true);*/
        //StartCoroutine(startLevel(1)); // Just for testing
    }

    IEnumerator startLoadLevel(int mapToLoad)
    {
        async = Application.LoadLevelAsync(mapToLoad);
        async.allowSceneActivation = false;

        yield return async;
    }

    public void nextLevel()
    {
        async.allowSceneActivation = true;
    }

    IEnumerator startLevel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nextLevel();
    }

    public void OnClickMapReturn()
    {
        this.mainMenuAnimator.enabled = true;
        this.mainMenuAnimator.CrossFade("MainMenuMapToInitial", 0f);
    }

    public void updateSFXLabel(int SFXVol)
    {
        this.SFXSound.text = SFXVol.ToString();
    }

    public void updateMUSLabel(int MUSVol)
    {
        this.MUSSound.text = MUSVol.ToString();
    }

    public void loading(bool showLoading)
    {
        this.startLoading = showLoading;
    }

    public void OnSelectChar()
    {
        PlayerData.instance.currKart = this.charDisplay.selectKart;
        AudioTest.instance.crossFadeLevelMusic (this.levelToLoad);
        StartCoroutine(startLoadLevel(this.levelToLoad + 1));
        this.loading(true);
    }

    public void OnReturnChar()
    {
        this.camAnim.CrossFade("CameraReturnSelection", 0f);
        StartCoroutine(disableCharDisplay());
    }

    IEnumerator disableCharDisplay()
    {
        yield return new WaitForSeconds(0.3f);
        this.charDisplay.gameObject.SetActive(false);

    }

    public void setLanguage(string newLanguage)
    {
        LoadDataManager.instance.updateLang(newLanguage.ToUpper());

        for (int count = 0; count < this.textElements.Length; count++)
        {
            this.textElements[count].UpdateTxt();
        }
    }

}

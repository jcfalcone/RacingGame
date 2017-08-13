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

    bool startGame = false;

    AsyncOperation async;

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

    public void OnClickMap(string mapToLoad)
    {
        StartCoroutine(startLoadLevel(mapToLoad));
        StartCoroutine(startLevel(1)); // Just for testing
    }

    IEnumerator startLoadLevel(string mapToLoad)
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

}

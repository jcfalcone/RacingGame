using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerMenu : MonoBehaviour 
{
    [SerializeField]
    Animator camAnim;

    [SerializeField]
    Animator mainMenuAnimator;

    [SerializeField]
    GameObject touchCanvas;

    bool startGame = false;

    AsyncOperation async;

	// Use this for initialization
	void Start () 
    {
		
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
}

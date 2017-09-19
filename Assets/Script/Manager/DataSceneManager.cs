using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DataSceneManager : Singleton<DataSceneManager>
{
    [SerializeField]
    Image loadingImg;

    bool hideLoading = false;

    AsyncOperation async = null;

	// Use this for initialization
	void Start () 
    {
        //check if GameManager instance already exists in Scene
        if(instance != null)
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
        if (this.async != null)
        {
            if (this.async.progress >= 0.9f)
            {
                this.hideLoading = true;
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

    public IEnumerator loadMainMenu()
    {
        this.async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
        yield return async;
    }
}

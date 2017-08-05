using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevManager : MonoBehaviour 
{


    float deltaTime = 0.0f;

	// Use this for initialization
	void Start () 
    {
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        DontDestroyOnLoad(gameObject);
        #else
        Destroy(gameObject);
        #endif
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }
	
    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        switch (Screen.orientation)
        {
            case ScreenOrientation.Portrait:
                GUI.Box(new Rect(20, Screen.height - 10 - Screen.height * 0.10f, Screen.width * 0.60f, Screen.height * 0.10f), text);
                break;
            case ScreenOrientation.Landscape:
                GUI.Box(new Rect(20, Screen.height - 10 - Screen.height * 0.10f, Screen.width * 0.05f, Screen.height * 0.10f), text);
                break;
            default:
                GUI.Box(new Rect(20, Screen.height - 10 - Screen.height * 0.10f, Screen.width * 0.05f, Screen.height * 0.10f), text);
                break;
        }
    }
}

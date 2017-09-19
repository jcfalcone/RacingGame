using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevManager : MonoBehaviour 
{
	public static DevManager instance
	{
		get { return _instance; }//can also use just get;
		set { _instance = value; }//can also use just set;
	}

	//Creates a class variable to keep track of GameManger
	static DevManager _instance = null;


    float deltaTime = 0.0f;

	// Use this for initialization
	void Start () 
    {
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        DontDestroyOnLoad(gameObject);
        #else
        Destroy(gameObject);
		#endif

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

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
	
    void OnGUI()
    {
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", deltaTime * 1000.0f, 1.0f / deltaTime);

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

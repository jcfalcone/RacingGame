using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour 
{
	public static DataManager instance
	{
		get { return _instance; }//can also use just get;
		set { _instance = value; }//can also use just set;
	}

	//Creates a class variable to keep track of GameManger
	static DataManager _instance = null;

    public static string MUSIC_VOL_KEY = "musicVol";
    public static string SFX_VOL_KEY   = "sfxVol";
    public static string HIGHSCORE_KEY = "HighScore";

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

	// Use this for initialization
	void Awake () 
    {
		DontDestroyOnLoad(this);

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

    void Start()
    {
        
    }
	
	// Update is called once per frame
	void OnSaveData () 
    {
        FindObjectOfType<AudioTest>().Save();
        LoadAndDisplaySaveFile();
	}

    void OnClearData()
    {
        PlayerPrefs.DeleteAll();
        LoadAndDisplaySaveFile();
    }

    void LoadAndDisplaySaveFile()
    {
        this.musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0f);
        this.sfxVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0f);
    }
}

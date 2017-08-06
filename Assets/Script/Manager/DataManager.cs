using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour 
{
    public const string MUSIC_VOL_KEY = "MusicVolume";
    public const string SFX_VOL_KEY   = "SfxVolume";
    public const string HIGHSCORE_KEY = "HighScore";

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

	// Use this for initialization
	void Awake () 
    {
        DontDestroyOnLoad(this);
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

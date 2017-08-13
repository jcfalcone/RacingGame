using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour 
{

    [SerializeField]
    AudioMixerSnapshot unpausedSnap;

    [SerializeField]
    AudioMixerSnapshot pausedSnap;

    [SerializeField]
    AudioMixer masterMixer;


    [SerializeField]
    float fadeInSpeed = 12;

    [SerializeField]
    AudioMixerSnapshot[] levelSnap;

    bool isPaused;

    int SFXVolume = 5;
    int MUSVolume = 5;

    float transitionIn;

	// Use this for initialization
	void Awake () 
    {
        if (!PlayerPrefs.HasKey(DataManager.MUSIC_VOL_KEY))
        {
            PlayerPrefs.SetInt(DataManager.MUSIC_VOL_KEY, 5);
        }

        if (!PlayerPrefs.HasKey(DataManager.SFX_VOL_KEY))
        {
            PlayerPrefs.SetInt(DataManager.SFX_VOL_KEY, 5);
        }

        this.MUSVolume = PlayerPrefs.GetInt(DataManager.MUSIC_VOL_KEY);
        this.SFXVolume = PlayerPrefs.GetInt(DataManager.SFX_VOL_KEY);

        UIManagerMenu.instance.updateMUSLabel(this.MUSVolume);
        UIManagerMenu.instance.updateSFXLabel(this.SFXVolume);

        this.transitionIn = 60 / fadeInSpeed;

        DontDestroyOnLoad(this);
	}

    void Start()
    {
        this.masterMixer.SetFloat(DataManager.SFX_VOL_KEY, -(80f - 80f * (this.SFXVolume / 10f)));
        this.masterMixer.SetFloat(DataManager.MUSIC_VOL_KEY, -(80f - 80f * (this.MUSVolume / 10f)));
    }
	
    public void OnClickPlayButton(string _ButtonName)
    {
        switch (_ButtonName.ToUpper())
        {
            case "gunshot":
                break;
            case "playerhurt":
                break;
            default:
                Debug.LogErrorFormat("Button {0} is not supported", _ButtonName);
                break;
        }
    }

    public void OnClickPauseButton()
    {
        this.isPaused = !this.isPaused;

        if (this.isPaused)
        {
            this.pausedSnap.TransitionTo(0.1f);
        }
        else
        {
            this.unpausedSnap.TransitionTo(0.1f);
        }
    }

    public void SetMinusSFXVolume()
    {
        this.SFXVolume--;
        this.SFXVolume = Mathf.Clamp(this.SFXVolume, 0 , 10);
        this.masterMixer.SetFloat(DataManager.SFX_VOL_KEY, -(80f - 80f * (this.SFXVolume / 10f)));
        UIManagerMenu.instance.updateSFXLabel(this.SFXVolume);
    }

    public void SetPlusSFXVolume()
    {
        this.SFXVolume++;

        this.SFXVolume = Mathf.Clamp(this.SFXVolume, 0 , 10);
        this.masterMixer.SetFloat(DataManager.SFX_VOL_KEY, -(80f - 80f * (this.SFXVolume / 10f)));
        UIManagerMenu.instance.updateSFXLabel(this.SFXVolume);
    }

    public void SetMinusMUSVolume()
    {

        this.MUSVolume--;
        this.MUSVolume = Mathf.Clamp(this.MUSVolume, 0 , 10);
        this.masterMixer.SetFloat(DataManager.MUSIC_VOL_KEY, -(80f - 80f * (this.MUSVolume / 10f)));

        UIManagerMenu.instance.updateMUSLabel(this.MUSVolume);
    }

    public void SetPlusMUSVolume()
    {
        this.MUSVolume++;
        this.MUSVolume = Mathf.Clamp(this.MUSVolume, 0 , 10);
        this.masterMixer.SetFloat(DataManager.MUSIC_VOL_KEY, -(80f - 80f * (this.MUSVolume / 10f)));

        UIManagerMenu.instance.updateMUSLabel(this.MUSVolume);
    }

    public void Save()
    {
        float musicVolume = 0f;
        float sfxVolume = 0f;

        PlayerPrefs.SetInt(DataManager.MUSIC_VOL_KEY, this.MUSVolume);
        PlayerPrefs.SetInt(DataManager.SFX_VOL_KEY, this.SFXVolume);
    }

    public void crossFadeLevelMusic(int level)
    {
        if (level > this.levelSnap.Length)
        {
            Debug.LogWarning("Invalid level number: "+level);
            return;
        }

        this.levelSnap[level].TransitionTo(transitionIn);
    }
}

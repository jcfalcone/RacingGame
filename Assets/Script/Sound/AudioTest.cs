using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour {

    [SerializeField]
    AudioMixerSnapshot unpausedSnap;

    [SerializeField]
    AudioMixerSnapshot pausedSnap;

    [SerializeField]
    AudioMixer masterMixer;

    bool isPaused;

    const string SFX_VOL = "sfxVol";
    const string MUSIC_VOL = "musVol";

	// Use this for initialization
	void Start () {
		
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

    public void SetSFXVolume(float _SFXVolume)
    {
        this.masterMixer.SetFloat(SFX_VOL, _SFXVolume);
    }

    public void SetMUSVolume(float _MUSVolume)
    {
        this.masterMixer.SetFloat(MUSIC_VOL, _MUSVolume);
    }

    public void Save()
    {
        float musicVolume = 0f;
        float sfxVolume = 0f;

        this.masterMixer.GetFloat(MUSIC_VOL, out musicVolume);
        this.masterMixer.GetFloat(SFX_VOL, out sfxVolume);

        PlayerPrefs.SetFloat(DataManager.MUSIC_VOL_KEY, musicVolume);
        PlayerPrefs.SetFloat(DataManager.MUSIC_VOL_KEY, sfxVolume);
    }
}

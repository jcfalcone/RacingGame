using System.Collections;
using System.Collections.Generic; 
using System;
using UnityEngine;
using UnityEngine.UI;

public class ControlMaster : MonoBehaviour 
{


    public static ControlMaster instance
    {
        get { return _instance; }//can also use just get;
        set { _instance = value; }//can also use just set;
    }

    //Creates a class variable to keep track of GameManger
    static ControlMaster _instance = null;

    [Header("Game Setting")]
    [SerializeField]
    public int totalLaps = 3;

    [Header("AI Waypoints")]
    [SerializeField]
    public Transform[] waypointList;


    [Header("Checkpoits")]
    [SerializeField]
    CheckPoint[] checkpointList;

    [Header("UI")]
    [SerializeField]
    Text bestTimeLabel;

    [Header("Items")]
    [SerializeField]
    public GameObject[] itemPrefabList;

    [SerializeField]
    public AudioClip[] itemAudio;

    float startLapTime;
    float bestTime = 0;
    float curBestTime = 0;

    public int currLap = 0;

	// Use this for initialization
	void Start () 
    {

        if (bestTime == 0)
        {
            bestTimeLabel.text = "--:--.--";
        }

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

    public void completeTrack(int Lap)
    {
        if(currLap != Lap)
        {
            return;
        }

        float currTime = (Time.time - startLapTime);

        if (currTime < bestTime || bestTime == 0)
        {
            bestTime = currTime;
            TimeSpan ts = TimeSpan.FromSeconds(bestTime);
            bestTimeLabel.text = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00") + ":" + Mathf.Round(ts.Milliseconds / 10);
        }

        for(int count = 0; count < this.checkpointList.Length; count++)
        {
            checkpointList[count].reset();
        }

        startLapTime = Time.time;

        currLap++;
        //checkpointList.Clear();
    }

    public void startRace()
    {
        List<CarTemplate> carList = CarPositionMaster.instance.carList;

        foreach(CarTemplate car in carList)
        {
            car.StartRace();
        }

        this.UnPauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
    }


}

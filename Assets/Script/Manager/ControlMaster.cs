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

    [Header("Spawnpoints")]
    [SerializeField]
    Transform[] spawnpointList;

    [Header("AI Waypoints")]
    [SerializeField]
    public Transform[] waypointList;

    [Header("Camera")]
    [SerializeField]
    CarCamera playerCamera;

    [Header("Checkpoits")]
    [SerializeField]
    public CheckPoint[] checkpointList;

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

    CarControl playerControl;

    public int currLap = 0;

    public bool paused = false;

	// Use this for initialization
	void Start () 
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

        this.totalLaps = LoadDataManager.instance.getParameter<int>("TotalLaps", this.totalLaps);

        GameObject player = Instantiate(PlayerData.instance.kartPlayerPrefab, this.spawnpointList[0].transform.position, this.spawnpointList[0].transform.rotation);
        CarPrepare tempPrepare = player.GetComponent<CarPrepare>();
        tempPrepare.SetUpKart(LoadDataManager.instance.kartMaterials[PlayerData.instance.currKart]);

        this.playerControl = player.GetComponent<CarControl>();

        Transform playerLookAt = player.transform.Find("CameraLookObj");

        this.playerCamera.SetPlayer(player.transform, playerLookAt);

        List<KartMaterial> materials = new List<KartMaterial>(LoadDataManager.instance.kartMaterials);

        materials.RemoveAt(PlayerData.instance.currKart);

        for (int count = 1; count < this.spawnpointList.Length; count++)
        {
            int enemyMaterial = UnityEngine.Random.Range(0, 1000) % materials.Count;

            player = Instantiate(PlayerData.instance.kartAIPrefab, this.spawnpointList[count].transform.position, this.spawnpointList[0].transform.rotation);
            tempPrepare = player.GetComponent<CarPrepare>();
            tempPrepare.SetUpKart(materials[enemyMaterial]);

            materials.RemoveAt(enemyMaterial);
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

            //bestTimeLabel.text = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00") + ":" + Mathf.Round(ts.Milliseconds / 10);
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
        this.paused = true;
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        this.paused = false;
        Time.timeScale = 1f;
    }

    public void UseItem()
    {
        this.playerControl.useItem();
    }


}

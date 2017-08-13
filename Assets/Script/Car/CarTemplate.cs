using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CarTemplate : MonoBehaviour , IEquatable<CarTemplate> , IComparable<CarTemplate>
{

    [Header("Player")]
    [SerializeField]
    public bool localPlayer = false;

    [Header("Items")]
    [SerializeField]
    protected Transform spawnPoint;

    protected GameObject currentItem;
    protected itemTemplate currentItemControl;


    [Header("UI")]
    [SerializeField]
    string PlayerName;

    [SerializeField]
    RacerData data;

    [Header("Camera")]
    [SerializeField]
    GameObject finalCamera;

    [Header("Sound")]
    [SerializeField]
    AudioSource itemSource;

    [Header("Speed")]
    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    public float maxTorque;

    [SerializeField]
    float maxReverseSpeed;

    [SerializeField]
    public float lowestSteerAtSpeed;

    [SerializeField]
    public float decelerationSpeed = 30;

    [SerializeField]
    public float maxGrassSpeed;

    [Header("Wheels")]
    [SerializeField]
    public WheelCollider[] rearWheelList;

    [SerializeField]
    protected Transform[] rearWheelTransList;

    [SerializeField]
    public WheelCollider[] frontWheelList;

    [SerializeField]
    protected Transform[] frontWheelTransList;

    [SerializeField]
    int[] gearRatio;

    public float totalScore
    {
        get
        {
            return (this.currLap * 200000) + (this.currWaypointIndex * 1000) - this.nextWaypointDist;
        }
    }

    protected AudioSource carAudio;

    protected bool braked;
    public bool resetMotorTorque = true;
    public float maxCurrentSpeed;
    public float currentSpeed;
    public Rigidbody rb;

    public int carPosition;
    public int   currWaypointIndex = 0;
    protected float nextWaypointDist = 0;
    protected int currLap = 0;

    protected List<int> checkpointList = new List<int>();

    float bestTime = 0;
    float startRaceTime = 0;

    protected bool finalCompleted = false;

	// Use this for initialization
    protected virtual void Start () 
    {
        this.checkpointList = new List<int>();

        this.rb = GetComponent<Rigidbody>();

        Vector3 centerOfMass = this.rb.centerOfMass;

        centerOfMass.y -= 0.9f;
        //centerOfMass.z = 0.9f;
        this.rb.centerOfMass = centerOfMass;

        this.maxCurrentSpeed = this.maxSpeed;

        CarPositionMaster.instance.addPlayer(this);

        this.carAudio = GetComponent<AudioSource>();

        /*if (this.localPlayer || 1 == 1)
        {
            this.currLap = 4;
            completeTrack();
        }*/

    }

    public void StartRace()
    {
        this.startRaceTime = Time.time;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        CarTemplate objAsPart = obj as CarTemplate;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public int CompareTo(CarTemplate item)
    {
        if (item == null)
        {
            return 1;
        }
        else
        {
            return this.totalScore.CompareTo(item.totalScore);
        }
    }

    public bool Equals(CarTemplate other)
    {
        if (other == null) return false;
        return (this.totalScore.Equals(other.totalScore));
    }

    public void updateRearWheel()
    {
        int count = 0;

        foreach (WheelCollider wheel in rearWheelList)
        {
            this.currentSpeed = Mathf.Round(2 * Mathf.PI * wheel.radius * wheel.rpm * 60 / 1000);
            rearWheelTransList[count].Rotate(wheel.rpm / 60f * 360f * Time.deltaTime, 0, 0);

            RaycastHit hit;
            Vector3 wheelPos = wheel.transform.position -wheel.transform.up * wheel.suspensionDistance;

            if(Physics.Raycast(wheel.transform.position, -wheel.transform.up, out hit, wheel.radius + wheel.suspensionDistance))
            {
                wheelPos = hit.point + wheel.transform.up * wheel.radius;

                if (hit.transform.CompareTag("Grass"))
                {
                    this.maxCurrentSpeed = this.maxGrassSpeed;
                }
                else
                {
                    this.maxCurrentSpeed = this.maxSpeed;
                }
            }

            rearWheelTransList[count].transform.position = wheelPos;

            count++;
        }
    }

    public void updateFrontWheel()
    {
        int count = 0;

        foreach (WheelCollider wheel in frontWheelList)
        {
            frontWheelTransList[count].Rotate(wheel.rpm / 60f * 360f * Time.deltaTime, 0, 0);

            Vector3 localEuler = frontWheelTransList[count].localEulerAngles;

            localEuler.y = wheel.steerAngle - frontWheelTransList[count].localEulerAngles.z;

            frontWheelTransList[count].localEulerAngles = localEuler;

            RaycastHit hit;
            Vector3 wheelPos = wheel.transform.position -wheel.transform.up * wheel.suspensionDistance;

            if(Physics.Raycast(wheel.transform.position, -wheel.transform.up, out hit, wheel.radius + wheel.suspensionDistance))
            {
                wheelPos = hit.point + wheel.transform.up * wheel.radius;
            }

            frontWheelTransList[count].transform.position = wheelPos;

            count++;
        }
    }

    public void moveCar(float Torque)
    {

        foreach (WheelCollider wheel in rearWheelList)
        {
            if (resetMotorTorque)
            {
                wheel.motorTorque = 0;
            }

            if (this.currentSpeed < this.maxCurrentSpeed && this.currentSpeed > -maxReverseSpeed && !this.braked)
            {
                wheel.motorTorque = Torque;//Input.GetAxis("Vertical");
            }

            wheel.brakeTorque = 0;

            if (Torque == 0)
            {
                wheel.brakeTorque = this.decelerationSpeed;
            }
        }
    }

    public void addLap()
    {
        this.currLap++;

        if (this.localPlayer)
        {
            UIManager.instance.UpdateLapNumber(this.currLap + 1, ControlMaster.instance.totalLaps);
        }
    }

    public void setCarWaypoint(int currWaypoint, float nextWaypointDistance)
    {
        this.currWaypointIndex = currWaypoint;
        this.nextWaypointDist = nextWaypointDistance;
    }

    public void updateCarWaypoint(float nextWaypointDistance)
    {
        this.nextWaypointDist = nextWaypointDistance;
    }

    public void addCheckPoint(int Id)
    {
        if (!checkpointList.Contains(Id))
        {
            checkpointList.Add(Id);
        }
    }

    public bool isTrackCompleted()
    {
        return this.checkpointList.Count >= 4;
    }

    virtual public void completeTrack()
    {
        if (this.isTrackCompleted())
        {
            ControlMaster.instance.completeTrack(this.currLap);
            this.addLap();
        }

        if (this.currLap >= ControlMaster.instance.totalLaps && !finalCompleted)
        {
            this.bestTime = Time.time - this.startRaceTime;
            UIManager.instance.addRacerFinal(this.PlayerName, this.data.racerPicture, 10 - this.carPosition * 5, this.localPlayer);

            if (this.localPlayer)
            {
                UIManager.instance.showFinal();

                if (this.finalCamera)
                {
                    Camera.main.gameObject.SetActive(false);
                    this.finalCamera.SetActive(true);
                }
            }

            finalCompleted = true;
        }

        this.checkpointList.Clear();
    }

    public void resetCheckpoint()
    {
        this.checkpointList.Clear();
    }

    virtual public void RandomItem()
    {
        if (this.currentItem == null)
        {
            int randomItem = UnityEngine.Random.Range(0, 1000) % ControlMaster.instance.itemPrefabList.Length;
            this.currentItem = ControlMaster.instance.itemPrefabList[randomItem];
            this.currentItemControl = this.currentItem.GetComponent<itemTemplate>();
        }
    }

    virtual public void useItem()
    {
        if (this.currentItem != null)
        {
            Vector3 spawnPos = this.spawnPoint.position;

            if(this.currentItemControl.effect == itemTemplate.ItemEffect.OilStan)
            {
                Vector3 normalPos = -transform.forward;
                normalPos.y = 0;
                spawnPos += normalPos * 5f;
            }

            GameObject item = Instantiate(this.currentItem, spawnPos, this.spawnPoint.rotation);

            this.itemSource.PlayOneShot(this.currentItemControl.launchSound);

            this.currentItem = null;
            this.currentItemControl = null;
        }
    }

    protected void carSound()
    {
        int gearMinValue = 0;
        int gearMaxValue = 0;
        for (int count = 0; count < this.gearRatio.Length; count++)
        {
            if (this.gearRatio[count] < this.currentSpeed)
            {
                gearMinValue = gearMaxValue;
                gearMaxValue = this.gearRatio[count];
            }
        }

        float enginePitch = Mathf.Clamp(((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue)), 1f, 3f);

        if ((gearMaxValue - gearMinValue) == 0)
        {
            enginePitch = 1;
        }

        this.carAudio.pitch = Mathf.Lerp(this.carAudio.pitch, enginePitch, Time.deltaTime * 2);
    }
	
}

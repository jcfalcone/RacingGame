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

    [SerializeField]
    protected GameObject currentItem;
    protected itemTemplate currentItemControl;

    [Header("Speed")]
    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    public float maxTorque;

    [SerializeField]
    float maxReverseSpeed;

    [SerializeField]
    protected float lowestSteerAtSpeed;

    [SerializeField]
    protected float decelerationSpeed = 30;

    [SerializeField]
    protected float maxGrassSpeed;

    [Header("Wheels")]
    [SerializeField]
    public WheelCollider[] rearWheelList;

    [SerializeField]
    protected Transform[] rearWheelTransList;

    [SerializeField]
    protected WheelCollider[] frontWheelList;

    [SerializeField]
    protected Transform[] frontWheelTransList;

    public float totalScore
    {
        get
        {
            return (this.currLap * 200000) + (this.currWaypointIndex * 1000) - this.nextWaypointDist;
        }
    }


    protected bool braked;
    public bool resetMotorTorque = true;
    public float maxCurrentSpeed;
    protected float currentSpeed;
    protected Rigidbody rb;

    public int carPosition;
    public int   currWaypointIndex = 0;
    protected float nextWaypointDist = 0;
    protected int currLap = 0;

    protected List<int> checkpointList = new List<int>();

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

    protected void updateRearWheel()
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

    protected void updateFrontWheel()
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

    protected void moveCar(float Torque)
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

    public void completeTrack()
    {
        if (this.isTrackCompleted())
        {
            ControlMaster.instance.completeTrack(this.currLap);
            this.addLap();
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

    public void useItem()
    {
        Debug.Log("Item Used");
        if (this.currentItem != null)
        {
            Vector3 spawnPos = this.spawnPoint.position;

            if(this.currentItemControl.effect == itemTemplate.ItemEffect.OilStan)
            {
                Vector3 normalPos = this.spawnPoint.position.normalized;
                normalPos.y = 0;
                spawnPos += normalPos * 5f;
            }

            GameObject item = Instantiate(this.currentItem, spawnPos, this.spawnPoint.rotation);

            this.currentItem = null;
            this.currentItemControl = null;
        }
    }
	
}

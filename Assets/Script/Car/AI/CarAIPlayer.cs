using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAIPlayer 
{
    [System.Flags]
    public enum aiSensors
    {
        None = 0,
        Front = 1,
        FrontRight = 2,
        FrontRightAngled = 4,
        FrontLeft = 8,
        FrontLeftAngled = 16,
        Right = 32,
        Left = 64,

        AllFront = Front | FrontRight | FrontLeft
    }

    [Header("Speed")]
    [SerializeField]
    float avoidSpeed = 15f;

    [Header("AI")]
    [SerializeField]
    [Range(1f, 50f)]
    float maxSteer = 15f;

    [SerializeField]
    [Range(1f, 50f)]
    float minDistance = 1f;

    [Header("Sensors")]
    [SerializeField]
    float sensorSize = 5f;

    [SerializeField]
    float sideSensorSize = 5f;

    [SerializeField]
    float sensorFrontStartPos = 5f;

    [SerializeField]
    float sensorFrontSidesStartPos = 5f;

    [SerializeField]
    float sensorFrontSidesStartAngle = 30f;

    [SerializeField]
    float sensorAvoidSensitivity = 0.5f;

    [SerializeField]
    LayerMask sensorLayers;

    [SerializeField]
    aiSensors activeSensors;

    [Header("Reverse")]
    [SerializeField]
    float waitToReverse = 2f;

    [SerializeField]
    float reverseFor = 1.5f;

    NavMeshPath[] waypointPathList;
    NavMeshPath   savePathList;

    float avoidSensitive = 0;

    int currWaypoint = 0;
    int currPathPoint = 0;

    Vector3 steerVector;
    //Vector3 carLastPos;

    bool inSlowZone = false;
    bool reversing = false;

    float waitToReverseTime = 0;
    float reverseTime = 0;
    int directionMov = 1;

    CarControl controller;

    public CarAIPlayer(CarControl parent)
    {
        this.controller = parent;
    }

    void OnDrawGizmos()
    {

        Vector3 startPos = this.controller.transform.position + (this.controller.transform.forward * sensorFrontStartPos);
        Vector3 rightAngle = Quaternion.AngleAxis(sensorFrontSidesStartAngle, this.controller.transform.up) * this.controller.transform.forward;
        Vector3 leftAngle = Quaternion.AngleAxis(-sensorFrontSidesStartAngle, this.controller.transform.up) * this.controller.transform.forward;

        startPos.y += 1f;

        Gizmos.DrawCube(startPos, new Vector3(0.2f, 0.2f, 0.2f));
        Gizmos.DrawRay(startPos, this.controller.transform.forward * sensorSize);

        startPos += (this.controller.transform.right * this.sensorFrontSidesStartPos);

        Gizmos.DrawCube(startPos, new Vector3(0.2f, 0.2f, 0.2f));
        Gizmos.DrawRay(startPos, this.controller.transform.forward * sensorSize);
        Gizmos.DrawRay(startPos, rightAngle * sensorSize);

        startPos = this.controller.transform.position + (this.controller.transform.forward * sensorFrontStartPos);
        startPos.y += 1f;
        startPos -= (this.controller.transform.right * this.sensorFrontSidesStartPos);

        Gizmos.DrawCube(startPos, new Vector3(0.2f, 0.2f, 0.2f));
        Gizmos.DrawRay(startPos, this.controller.transform.forward * sensorSize);
        Gizmos.DrawRay(startPos, leftAngle * sensorSize);

        startPos = this.controller.transform.position;
        startPos.y += 1f;
        Gizmos.DrawRay(startPos, this.controller.transform.right * sideSensorSize);
        Gizmos.DrawRay(startPos, -this.controller.transform.right * sideSensorSize);

        /*Gizmos.color = Color.red;
        for (int i = 0; i < this.waypointPathList[currWaypoint].corners.Length-1; i++) 
        {
            Debug.Log(this.waypointPathList[currWaypoint].corners[i] + " - " + this.waypointPathList[currWaypoint].corners[i + 1]);
            Gizmos.DrawLine(this.waypointPathList[currWaypoint].corners[i], this.waypointPathList[currWaypoint].corners[i + 1]);
        }*/
    }

	// Use this for initialization
    public void Start () 
    {
        this.waypointPathList = new NavMeshPath[ControlMaster.instance.waypointList.Length];

        Vector3 lastPos = this.controller.transform.position;

        for(int count = 0; count < this.waypointPathList.Length; count++)
        {
            this.waypointPathList[count] = new NavMeshPath();

            Vector3 destinePos = ControlMaster.instance.waypointList[count].position;

            NavMesh.CalculatePath(lastPos, destinePos, NavMesh.AllAreas, this.waypointPathList[count]);

//            Debug.Log(this.waypointPathList[count].status);

            lastPos = destinePos;
        }
	}
	
	// Update is called once per frame
	public void Update () 
    {

        this.carSensors();

        float currSteer = this.getWheelSteer();

        foreach (WheelCollider wheel in this.controller.frontWheelList)
        {
            wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, currSteer, this.controller.rb.velocity.magnitude / this.controller.lowestSteerAtSpeed);
        }

        this.checkDistance();
        this.checkReverse();

        for (int i = 0; i < this.waypointPathList[currWaypoint].corners.Length-1; i++) {
            Debug.DrawLine (this.waypointPathList[currWaypoint].corners[i], this.waypointPathList[currWaypoint].corners[i + 1], Color.red);
        }
	}

    public void FixedUpdate()
    {

        this.controller.updateRearWheel();

        if (inSlowZone)
        {
            this.controller.maxCurrentSpeed = this.controller.maxGrassSpeed;
        }

        if (this.activeSensors != aiSensors.None)
        {
            this.controller.maxCurrentSpeed = avoidSpeed;
        }

        this.controller.moveCar(this.controller.maxTorque * this.directionMov);
        this.controller.updateFrontWheel();


        foreach (WheelCollider wheel in this.controller.rearWheelList)
        {
            if ((this.activeSensors & aiSensors.Front) != aiSensors.None)
            {
                wheel.brakeTorque = this.controller.decelerationSpeed;
            }
            else
            {
                wheel.brakeTorque = 0;
            }
        }
    }

    float getWheelSteer()
    {
        if (this.activeSensors != aiSensors.None)
        {
            return this.maxSteer * this.avoidSensitive;
        }

        if (currWaypoint > this.waypointPathList.Length)
        {
            currWaypoint = 0;
        }

        NavMeshPath curNavPath = this.waypointPathList[currWaypoint];

        if (this.savePathList != null)
        {
            curNavPath = this.savePathList;
        }

        if (this.waypointPathList[currWaypoint].corners.Length > 0)
        {
            if (currPathPoint >= curNavPath.corners.Length)
            {
                currPathPoint = 0;
            }

            this.steerVector = this.controller.transform.InverseTransformPoint(curNavPath.corners[currPathPoint]);
            return this.maxSteer * (steerVector.x / steerVector.magnitude);
        }

        return 0;

    }

    void checkDistance()
    {
        if (this.steerVector.magnitude <= this.minDistance)
        {

            NavMeshPath curNavPath = this.waypointPathList[currWaypoint];

            if (this.savePathList != null)
            {
                curNavPath = this.savePathList;
            }

            this.currPathPoint++;

            if (this.currPathPoint >= curNavPath.corners.Length)
            {
                if (this.savePathList != null)
                {
                    this.savePathList = null;
                }
                else
                {
                    this.currWaypoint++;
                }

                this.currPathPoint = 0;

                if (this.currWaypoint >= this.waypointPathList.Length)
                {
                    this.currWaypoint = 0;
                }
            }
        }
    }

    void checkReverse()
    {
        if (this.controller.rb.velocity.magnitude < 1 && !this.reversing)
        {
            this.waitToReverseTime += Time.deltaTime;
        }
        else if(!this.reversing)
        {
            this.waitToReverseTime = 0;
        }

        if (this.reversing)
        {
            //this.avoidSensitive *= -1;
            this.directionMov = -1;
            this.reverseTime += Time.deltaTime;
            this.waitToReverseTime = 0;

            if (this.reverseTime > this.reverseFor)
            {
                this.reversing = false;
                this.directionMov = 1;
            }
        }

        if (this.waitToReverseTime > this.waitToReverse)
        {
            this.reversing = true;
            this.reverseTime = 0;
        }
    }

    void carSensors()
    {

        RaycastHit hit;

        this.activeSensors  = aiSensors.None;
        this.avoidSensitive = 0f;

        Vector3 startPos = this.controller.transform.position + (this.controller.transform.forward * this.sensorFrontStartPos);
        Vector3 rightAngle = Quaternion.AngleAxis(sensorFrontSidesStartAngle, this.controller.transform.up) * this.controller.transform.forward;
        Vector3 leftAngle = Quaternion.AngleAxis(-sensorFrontSidesStartAngle, this.controller.transform.up) * this.controller.transform.forward;

        startPos.y += 1f;

        //Front Right Sensor
        startPos += (this.controller.transform.right * this.sensorFrontSidesStartPos);

        if (Physics.Raycast(startPos, this.controller.transform.forward, out hit, this.sensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.FrontRight;
            this.avoidSensitive -= sensorAvoidSensitivity;

            Debug.DrawLine(startPos, hit.point, Color.red);
        }
        //Front Angled Right Sensor
        else if (Physics.Raycast(startPos, rightAngle, out hit, this.sensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.FrontRightAngled;
            this.avoidSensitive -= sensorAvoidSensitivity * 0.5f;
            Debug.DrawLine(startPos, hit.point, Color.red);
        }

        //Front Left Sensor
        startPos = this.controller.transform.position + (this.controller.transform.forward * this.sensorFrontStartPos);
        startPos.y += 1f;
        startPos -= (this.controller.transform.right * this.sensorFrontSidesStartPos);

        if (Physics.Raycast(startPos, this.controller.transform.forward, out hit, this.sensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.FrontLeft;
            this.avoidSensitive += sensorAvoidSensitivity;
            Debug.DrawLine(startPos, hit.point, Color.red);
        }
        //Front Angled Left Sensor
        else if (Physics.Raycast(startPos, leftAngle, out hit, this.sensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.FrontLeftAngled;
            this.avoidSensitive += sensorAvoidSensitivity * 0.5f;
            Debug.DrawLine(startPos, hit.point, Color.red);
        }

        //Reset Position
        startPos = this.controller.transform.position;
        startPos.y += 1f;

        //Right Sensor
        if (Physics.Raycast(startPos, this.controller.transform.right, out hit, this.sideSensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.Right;
            this.avoidSensitive -= sensorAvoidSensitivity * 0.5f;
            Debug.DrawLine(startPos, hit.point, Color.red);
        }

        //Left Sensor
        if (Physics.Raycast(startPos, -this.controller.transform.right, out hit, this.sideSensorSize, sensorLayers))
        {
            this.activeSensors |= aiSensors.Left;
            this.avoidSensitive += sensorAvoidSensitivity * 0.5f;
            Debug.DrawLine(startPos, hit.point, Color.red);
        }


        //Front Sensor
        startPos = this.controller.transform.position + (this.controller.transform.forward * this.sensorFrontStartPos);
        startPos.y += 1f;

        if (this.avoidSensitive == 0f)
        {
            if (Physics.Raycast(startPos, this.controller.transform.forward, out hit, this.sensorSize, sensorLayers))
            {
                this.activeSensors |= aiSensors.Front;

                if (hit.normal.x < 0)
                {
                    this.avoidSensitive -= sensorAvoidSensitivity;
                }
                else
                {
                    this.avoidSensitive += sensorAvoidSensitivity;
                }

                Debug.DrawLine(startPos, hit.point, Color.red);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Slowzone"))
        {
            this.inSlowZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Slowzone"))
        {
            this.inSlowZone = false;
        }
    }
}

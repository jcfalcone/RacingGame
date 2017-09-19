using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarPositionMaster : MonoBehaviour 
{


    public static CarPositionMaster instance
    {
        get { return _instance; }//can also use just get;
        set { _instance = value; }//can also use just set;
    }

    //Creates a class variable to keep track of GameManger
    static CarPositionMaster _instance = null;

    [SerializeField]
    int minDistanceWaypoint = 10;

    [SerializeField]
    int updateEveryFPS = 10;

    [SerializeField]
    Text PlayerPosition;

    int fpsCount = 0;


    public List<CarTemplate> carList;

	// Use this for initialization
	void Awake () 
    {
        this.carList = new List<CarTemplate>();
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
        if (this.fpsCount > this.updateEveryFPS)
        {
            Transform[] waypointList = ControlMaster.instance.waypointList;

            for (int countCar = 0; countCar < this.carList.Count; countCar++)
            {
                int currWaypoint = this.carList[countCar].currWaypointIndex + 1;

                if ((currWaypoint) >= waypointList.Length)
                {
                    currWaypoint = 0;
                }

                if (Vector3.Distance(waypointList[currWaypoint].position, this.carList[countCar].transform.position) < this.minDistanceWaypoint)
                {
                     this.carList[countCar].setCarWaypoint(currWaypoint, Vector3.Distance(waypointList[currWaypoint].position, this.carList[countCar].transform.position));
                }
                else
                {
                    this.carList[countCar].updateCarWaypoint(Vector3.Distance(waypointList[currWaypoint].position, this.carList[countCar].transform.position));
                }
            }

            this.carList.Sort();
            this.carList.Reverse();

            for (int countCar = 0; countCar < this.carList.Count; countCar++)
            {
                this.carList[countCar].carPosition = countCar;

                if (this.carList[countCar].localPlayer)
                {
                    this.PlayerPosition.text = (countCar + 1) + " / " + this.carList.Count;
                }

                //Debug.Log(countCar + " - " + this.carList[countCar].transform.name+" = "+ this.carList[countCar].totalScore, this.carList[countCar].gameObject);
            }

            this.fpsCount = 0;
        }

        this.fpsCount++;
    }
	
    public void addPlayer(CarTemplate car)
    {
        carList.Add(car);
    }

    public void carWaypoint(Transform waypoint, CarTemplate car)
    {
        
    }
}

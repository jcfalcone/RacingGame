using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurbo : MonoBehaviour 
{

    [Header("Turbo")]

    [SerializeField]
    float maxTurboSpeed;

    [SerializeField]
    float turboTime;

    [SerializeField]
    float turboDistance;

    [SerializeField]
    float turboZoomRatio;

    float turboTotalTimer;

    bool startTurbo = false;

    float originalZoomRatio;
    float originalDistance;

    CarCamera cameraController;
    CarTemplate controller;

	// Use this for initialization
	void Start () 
    {
        this.controller = GetComponent<CarTemplate>();

        this.cameraController = Camera.main.GetComponent<CarCamera>();

        this.originalDistance = this.cameraController.distance;
        this.originalZoomRatio = this.cameraController.zoomRatio;	
    }

    void Update()
    {
        if (startTurbo)
        {
            this.turboTotalTimer += Time.deltaTime;

            if (this.turboTotalTimer > this.turboTime)
            {
                if (this.controller.localPlayer)
                {
                    this.cameraController.distance = this.originalDistance;
                    this.cameraController.zoomRatio = this.originalZoomRatio;
                    startTurbo = false;
                }

                controller.maxCurrentSpeed = controller.maxSpeed;
                controller.resetMotorTorque = true;
            }
        }
    }

    public void AddTurbo(float turboAmount)
    {

        foreach (WheelCollider wheel in controller.rearWheelList)
        {
            wheel.motorTorque = controller.maxTorque * turboAmount;//Input.GetAxis("Vertical");

            turboTotalTimer = 0;
            startTurbo = true;

            controller.maxCurrentSpeed = this.maxTurboSpeed;

            if (this.controller.localPlayer)
            {
                this.cameraController.distance = this.turboDistance;
                this.cameraController.zoomRatio = this.turboZoomRatio;
            }
        } 

        controller.resetMotorTorque = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurbo : MonoBehaviour 
{


    [Header("Particles")]
    [SerializeField]
    ParticleSystem turboParticle;

    [Header("Turbo")]

    [SerializeField]
    float maxTurboSpeed;

    [SerializeField]
    float turboTime;

    [SerializeField]
    float turboDistance;

    [SerializeField]
    float turboZoomRatio;

    [SerializeField]
    float cooldown;

    float turboTotalTimer;

    bool startTurbo = false;
    bool canUseTurbo = true;

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

                if (this.turboParticle != null)
                {
                    this.turboParticle.Stop();
                }

                this.controller.maxCurrentSpeed = this.controller.maxSpeed;
                this.controller.resetMotorTorque = true;
            }
        }
    }

    public void AddTurbo(float turboAmount)
    {
        this.AddTurbo(turboAmount, false);
    }

    public void AddTurbo(float turboAmount, bool item)
    {
        if (!this.canUseTurbo && !item)
        {
            return;
        }

        this.canUseTurbo = false;

        this.controller.maxCurrentSpeed = this.maxTurboSpeed;
        
        foreach (WheelCollider wheel in controller.rearWheelList)
        {

            if (this.controller.currentSpeed < this.controller.maxCurrentSpeed)
            {
                //wheel.motorTorque = this.controller.maxTorque * turboAmount;//Input.GetAxis("Vertical");
            }
        } 

        turboTotalTimer = 0;
        startTurbo = true;

        if (this.controller.localPlayer)
        {
            this.cameraController.distance = this.turboDistance;
            this.cameraController.zoomRatio = this.turboZoomRatio;
        }

        this.controller.resetMotorTorque = false;

        if (this.turboParticle != null)
        {
            this.turboParticle.Play();
        }

        StartCoroutine(this.coolDownTurbo());
    }

    IEnumerator coolDownTurbo()
    {
        yield return new WaitForSeconds(this.cooldown);

        this.canUseTurbo = true;
    }
}

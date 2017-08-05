﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : CarTemplate 
{

    [Header("UI")]
    [SerializeField]
    Image itemUI;

    [Header("Speed")]

    float originalSpeed;

    [SerializeField]
    float maxBrake;

    [SerializeField]
    float lowSpeedSteerAngle = 10;

    [SerializeField]
    float highSpeedSteerAngle = 1;


    [SerializeField]
    int[] gearRatio;

    float carSidewayFriction;
    float carForwardFriction;
    float slipSidewayFriction;
    float slipForwardFriction;

    AudioSource carAudio;

    float lastHorizontalInput;
    float lastVerticalInput;

	// Use this for initialization
	override protected void Start () 
    {
        base.Start();

        this.carAudio = GetComponent<AudioSource>();

        startFriction();
	}

    void Update()
    {

        carSound();

        if (inputItem())
        {
            useItem();
        }

//        Debug.Log(transform.name+" - "+this.totalScore);
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        int count = 0;

        if (this.originalSpeed >= 0)
        {
            handBrakeCar();
        }

        this.updateRearWheel();
        this.moveCar(this.maxTorque * inputVertical());

        /*foreach (WheelCollider wheel in rearWheelList)
        {
            if (resetMotorTorque)
            {
                wheel.motorTorque = 0;
            }

            if (this.currentSpeed < this.maxCurrentSpeed && this.currentSpeed > -maxReverseSpeed && !this.braked)
            {
                wheel.motorTorque = this.maxTorque * inputVertical();//Input.GetAxis("Vertical");
            }

            wheel.brakeTorque = 0;

            if (this.lastVerticalInput == 0)
            {
                wheel.brakeTorque = this.decelerationSpeed;
            }

            count++;
        }*/

        this.originalSpeed = this.currentSpeed;
        this.currentSpeed = Mathf.Abs(this.currentSpeed);

        count = 0;
        float currentSteerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, this.rb.velocity.magnitude / this.lowestSteerAtSpeed);

        foreach (WheelCollider wheel in frontWheelList)
        {
            wheel.steerAngle = currentSteerAngle * inputHorizontal();//Input.GetAxis("Horizontal");
        }

        this.updateFrontWheel();

    }

    bool inputItem()
    {
        #if !UNITY_IOS || !UNITY_ANDROID
        if(Input.GetKeyUp(KeyCode.E))
        {
            return true;
        }
        #endif
        return false;
    }

    float inputHorizontal()
    {
        lastHorizontalInput = 0;

        #if UNITY_IOS || UNITY_ANDROID
        float.TryParse(Input.acceleration.x.ToString("N1"), out lastHorizontalInput);

        float tempHor = Mathf.Abs(lastHorizontalInput);

        if(this.originalSpeed <= 0)
        {
            this.lastHorizontalInput *= -1;
        }

        if(tempHor > 0.4f)
        {
            lastHorizontalInput = 1f * Mathf.Sign(lastHorizontalInput);
        }
        else if(tempHor > 0.3f)
        {
            lastHorizontalInput = 0.5f * Mathf.Sign(lastHorizontalInput);
        }
        else if(tempHor > 0.2f)
        {
            lastHorizontalInput = 0.3f * Mathf.Sign(lastHorizontalInput);
        }

        #else
        lastHorizontalInput = Input.GetAxis("Horizontal");
        #endif

        return lastHorizontalInput;
    }

    float inputVertical()
    {
        lastVerticalInput = 0;

        #if UNITY_IOS || UNITY_ANDROID
        if(Input.touches.Length == 1)
        {
            lastVerticalInput = 1;
        }
        else if(this.currentSpeed <= 0 && Input.touches.Length == 2)
        {
            lastVerticalInput = -1;
        }

        #else
        lastVerticalInput = Input.GetAxis("Vertical");
        #endif

        return lastVerticalInput;
    }

    bool inputBrake()
    {
        bool brakeCar = false;
        #if UNITY_IOS || UNITY_ANDROID

        if(Input.touches.Length == 2)
        {
            brakeCar = true;
        }

        #else
        brakeCar = Input.GetButton("Jump");
        #endif

        return brakeCar;
    }

    void startFriction()
    {
        this.carSidewayFriction = rearWheelList[0].sidewaysFriction.stiffness;
        this.carForwardFriction = rearWheelList[0].forwardFriction.stiffness;
        this.slipSidewayFriction = 0.2f;
        this.slipForwardFriction = 0.4f;
    }

    void setupFriction(float curForwardFriction, float curSidewayFriction)
    {
        foreach (WheelCollider wheel in rearWheelList)
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;

            forwardFriction.stiffness = curForwardFriction;
            sidewaysFriction.stiffness = curSidewayFriction;
        }
    }

    void handBrakeCar()
    {
        braked = false;

        if (inputBrake())//Input.GetButton("Jump"))
        {
            braked = true;
        }

        if (braked)
        {
            foreach (WheelCollider wheel in rearWheelList)
            {

                wheel.brakeTorque = this.maxBrake;
                wheel.motorTorque = 0;
            }

            if (this.rb.velocity.magnitude > 1f)
            {
                this.setupFriction(this.slipForwardFriction, this.slipSidewayFriction);
            }
        }
        else
        {

            this.setupFriction(this.carForwardFriction, this.carSidewayFriction);
        }
    }

    void carSound()
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

    override public void RandomItem()
    {
        if (this.currentItem == null)
        {
            base.RandomItem();
            this.itemUI.sprite = this.currentItemControl.itemUI;
        }
    }
}

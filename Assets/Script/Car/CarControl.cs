using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarControl : CarTemplate 
{

    [Header("UI")]
    [SerializeField]
    Image itemUI;

    [SerializeField]
    GameObject itemObj;

    [Header("Speed")]

    float originalSpeed;

    [SerializeField]
    float maxBrake;

    [SerializeField]
    float lowSpeedSteerAngle = 10;

    [SerializeField]
    float highSpeedSteerAngle = 1;

    float carSidewayFriction;
    float carForwardFriction;
    float slipSidewayFriction;
    float slipForwardFriction;

    float lastHorizontalInput;
    float lastVerticalInput;

    CarAIPlayer autoController;

	// Use this for initialization
	override protected void Start () 
    {
        base.Start();

        startFriction();
	}

    void Update()
    {
        carSound();

        if (this.autoController != null)
        {
            this.autoController.Update();
            return;
        }

        if (inputItem())
        {
            useItem();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        int count = 0;

        if (this.autoController != null)
        {
            this.autoController.FixedUpdate();
            return;
        }

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
        #if !UNITY_IOS || !UNITY_ANDROID || !UNITY_EDITOR
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

        #if UNITY_IOS || UNITY_ANDROID || !UNITY_EDITOR
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

        #if UNITY_IOS || UNITY_ANDROID || !UNITY_EDITOR
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
        #if UNITY_IOS || UNITY_ANDROID || !UNITY_EDITOR

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

    override public void RandomItem()
    {
        if (this.currentItem == null)
        {
            base.RandomItem();
            this.itemUI.sprite = this.currentItemControl.itemUI;
            this.itemObj.SetActive(true);
        }
    }

    override public void useItem()
    {
        base.useItem();
        this.itemObj.SetActive(false);
    }

    override public void completeTrack()
    {
        base.completeTrack();

        if (this.finalCompleted && this.localPlayer && this.autoController == null)
        {
            this.autoController = new CarAIPlayer(this);
            this.autoController.Start();
        }
    }
}

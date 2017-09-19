using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSlip : MonoBehaviour 
{
    CarTemplate controller;

	// Use this for initialization
	void Start () 
    {
        this.controller = GetComponent<CarTemplate>();
    }

    public void slipCar()
    {
        if (controller.shielded)
        {
            controller.removeShield();
            return;
        }

        StartCoroutine(slip());
    }

    IEnumerator slip()
    {
        WheelFrictionCurve curve = this.controller.rearWheelList[0].sidewaysFriction;
        float originalBrake = this.controller.rearWheelList[0].brakeTorque;
        float originalAngle = this.controller.rearWheelList[0].steerAngle;

        curve.stiffness = 0.1f;

        foreach (WheelCollider wheel in this.controller.rearWheelList)
        {

            wheel.brakeTorque = 90000;
            wheel.sidewaysFriction = curve;
            wheel.steerAngle = 10;
        }

        yield return new WaitForSeconds(1f);

        curve.stiffness = 10f;

        foreach (WheelCollider wheel in this.controller.rearWheelList)
        {
            wheel.brakeTorque = 0;
            wheel.sidewaysFriction = curve;
            wheel.steerAngle = 0;
        }
    }
}

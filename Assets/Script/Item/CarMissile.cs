using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMissile : MonoBehaviour 
{
    CarTemplate controller;

	// Use this for initialization
    void Start () 
    {
        this.controller = GetComponent<CarTemplate>();
    }

    public void misselCar()
    {
        if (controller.shielded)
        {
            controller.removeShield();
            return;
        }

        StartCoroutine(missel());
    }
	
	// Update is called once per frame
    IEnumerator missel () 
    {
        this.controller.rb.velocity = Vector3.zero;

        foreach (WheelCollider wheel in this.controller.rearWheelList)
        {
            wheel.brakeTorque = 90000;
        }

        yield return new WaitForSeconds(1f);

        foreach (WheelCollider wheel in this.controller.rearWheelList)
        {
            wheel.brakeTorque = 0;
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class CarSkid : MonoBehaviour {

    [SerializeField]
    float slipSoundAt;

    [SerializeField]
    GameObject slipPrefab;

    [SerializeField]
    float currentSlip;
    WheelCollider wheel;

	// Use this for initialization
	void Start () {
        wheel = GetComponent<WheelCollider>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        WheelHit hit;
        this.wheel.GetGroundHit(out hit);
        currentSlip = Mathf.Abs(hit.sidewaysSlip);

        if (currentSlip >= this.slipSoundAt)
        {
            GameObject slipSound = Instantiate(this.slipPrefab, hit.point, Quaternion.identity);
            Destroy(slipSound, 1f);
        }
	}
}

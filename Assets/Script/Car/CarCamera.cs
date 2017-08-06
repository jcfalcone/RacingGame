using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour {

    [SerializeField]
    Transform car;

    [SerializeField]
    Transform lookObj;

    [SerializeField]
    public float zoomRatio = 0.5f;

    float curZoomRatio;

    [SerializeField]
    float height = 1.4f;

    [SerializeField]
    public float distance = 6.4f;

    float curDistance;

    [SerializeField]
    float rotationVelocity = 3.0f;

    [SerializeField]
    float heightVelocity = 2.0f;

    Rigidbody carRb;
    Vector3 rotationVector;
    float defaultFOV;

	// Use this for initialization
	void Start () 
    {
        this.carRb = this.car.GetComponent<Rigidbody>();

        defaultFOV = Camera.main.fieldOfView;

        this.curDistance = this.distance;
        this.curZoomRatio = this.zoomRatio;
	}

    void LateUpdate()
    {
        float wantedHeight = car.position.y + height;

        float myAngle = Mathf.LerpAngle(transform.eulerAngles.y, rotationVector.y, Time.unscaledDeltaTime * rotationVelocity);
        float myHeight = Mathf.Lerp(transform.position.y, wantedHeight, Time.unscaledDeltaTime * heightVelocity);

        Vector3 newPosition = car.position;
        newPosition  -= Quaternion.Euler(0, myAngle, 0) * Vector3.forward * this.curDistance;
        newPosition.y = myHeight;

        transform.position = newPosition;
        transform.LookAt(lookObj);

        if (this.curDistance != this.distance)
        {
            this.curDistance = Mathf.Lerp(this.curDistance, this.distance, Time.unscaledDeltaTime * 1.5f);
        }

        if (this.curZoomRatio != this.zoomRatio)
        {
            this.curZoomRatio = Mathf.Lerp(this.curZoomRatio, this.zoomRatio, Time.unscaledDeltaTime * 1.5f);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        Vector3 localVelocity = car.InverseTransformDirection(this.carRb.velocity);

        localVelocity.Normalize();

        if (localVelocity.z < -0.5f)
        {
            rotationVector.y = car.eulerAngles.y + 180;
        }
        else
        {
            rotationVector.y = car.eulerAngles.y;
        }

        float velMag = this.carRb.velocity.magnitude;

        Camera.main.fieldOfView = defaultFOV + velMag * this.curZoomRatio;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour 
{

    private const float MIN_SWIPE_DISTANCE = 50f;
    private const float MAX_SWIPE_TIME = 50f;

    private Rigidbody _rb;
    private Vector3 _mouseStartPos = Vector3.zero;
    private float _elapsedTime;
    private bool _runTimer;

    bool enableFreeSwipe;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        #if UNITY_EDITOR
        SimulateMouseSwipe();
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                _runTimer = true;
                _elapsedTime = 0f;
            }

            if(touch.phase == TouchPhase.Ended && _elapsedTime < MAX_SWIPE_TIME)
            {
                SpinCube(touch.deltaPosition);
            }
        }
        #endif

        if(_runTimer)
        {
            if(_elapsedTime < MAX_SWIPE_TIME)
            {
                _elapsedTime += Time.deltaTime;
            }

            if(_elapsedTime > MAX_SWIPE_TIME)
            {
                _runTimer = false;
            }
        }
	}

    private void SimulateMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseStartPos = Input.mousePosition;
            _runTimer = true;
            _elapsedTime = 0f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _runTimer = false;
            Vector3 mouseEndPos = Input.mousePosition;
            Vector3 direction = mouseEndPos - _mouseStartPos;

            SpinCube(direction);
        }
    }

    private void SpinCube(Vector3 Direction)
    {
        float force = Direction.magnitude;

        if (Direction.magnitude > MIN_SWIPE_DISTANCE)
        {
            Direction.Normalize();

            if (!enableFreeSwipe)
            {
                if (Direction.x > 0 && Direction.y > -0.5f && Direction.y < 0.5f)
                {
                    _rb.AddTorque(new Vector3(0f, 1f, 0f)  * -force);
                }
                else if (Direction.x < 0 && Direction.y > -0.5f && Direction.y < 0.5f)
                {
                    _rb.AddTorque(new Vector3(0f, 1f, 0f)  * force);
                }
                else if (Direction.y > 0 && Direction.x > -0.5f && Direction.x < 0.5f)
                {
                    _rb.AddTorque(new Vector3(1f, 0f, 0f)  * force);
                }
                else if (Direction.y < 0 && Direction.x > -0.5f && Direction.x < 0.5f)
                {
                    _rb.AddTorque(new Vector3(1f, 0f, 0f)  * -force);
                }
            }
            else
            {
                _rb.AddTorque(new Vector3(0f, 1f, 0f) * -Direction.x * force);
                _rb.AddTorque(new Vector3(1f, 0f, 0f) * -Direction.y * force);
            }
        }
    }
}

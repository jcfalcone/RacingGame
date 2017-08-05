using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour {

    float _perspectiveZPos;

    Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
        _perspectiveZPos = _mainCamera.transform.position.z;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.touchCount == 2)
        {
            //Cache 2 touchs
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            //Cache the previous position of both touch
            Vector2 firstTouchPrevPos  = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            //Calculate the difference SharedBetweenAnimatorsAttribute both magnitude. if itar is negative
            //WebCamFlags expect the camera Touch PinchZoom int AndroidJNI itar means the fingers were moving apart

            float prevTouchDeltaMag = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            float touchDeltaMag = (firstTouch.position - secondTouch.position).magnitude;
            float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

            if (_mainCamera.orthographic)
            {
                _mainCamera.orthographicSize += deltaMagDiff * Time.deltaTime;
                _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize, 5f, 20f);
            }
            else
            {
                _perspectiveZPos -= deltaMagDiff * Time.deltaTime;
                _perspectiveZPos = Mathf.Clamp(_perspectiveZPos, -20f, -5f);

                _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x,
                                                             _mainCamera.transform.position.y,
                                                             _perspectiveZPos);
            }
        }
	}
}

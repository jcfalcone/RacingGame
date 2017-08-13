using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemShield : itemTemplate 
{
    [SerializeField]
    Vector3 rotationSpeed;

    [SerializeField]
    Transform rotateObj;

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        rotateObj.Rotate(rotationSpeed * Time.deltaTime);
	}

    override protected void itemEffect(Vector3 position)
    {
    }
}

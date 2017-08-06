using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endCheckpoint : MonoBehaviour 
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            CarTemplate temp = other.transform.parent.gameObject.GetComponent<CarTemplate>();

            temp.completeTrack();

        }
    }
}

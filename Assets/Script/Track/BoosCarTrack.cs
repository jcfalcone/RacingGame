using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosCarTrack : MonoBehaviour 
{
    [SerializeField]
    float boostAmount = 3f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            other.transform.parent.GetComponent<CarTurbo>().AddTurbo(boostAmount);
        }
    }
}

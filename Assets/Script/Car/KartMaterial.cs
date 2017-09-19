using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KartMaterial 
{
    public Material body;
    public Material aerofolio;
    public Material paralama;
    public GameObject charPrefab;
    public float maxSpeed;
    public float maxTorque;
    public float maxReverseSpeed;
    public float maxGrassSpeed;
    public float decelerationSpeed;
}

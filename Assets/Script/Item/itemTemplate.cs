using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class itemTemplate : MonoBehaviour 
{

    public enum ItemEffect
    {
        OilStan,
        Rocket,
        Turbo,
        Shield
    }

    [SerializeField]
    public ItemEffect effect;

    [SerializeField]
    protected float Speed;

    [SerializeField]
    Vector3 rotationForce;

    [SerializeField]
    public AudioClip launchSound;

    [SerializeField]
    public Sprite itemUI;

    protected Rigidbody rb;

    [System.NonSerialized]
    public Vector3 initialVelocity = Vector3.zero;

	// Use this for initialization
	protected void Start () 
    {
        this.rb = GetComponent<Rigidbody>();

        if (this.Speed != 0)
        {
            this.rb.AddForce(transform.forward * Speed);
        }
    }

    void FixedUpdate() 
    {
        if (this.rb != null)
        {
            Quaternion deltaRotation = Quaternion.Euler(this.rotationForce * Time.deltaTime);
            this.rb.MoveRotation(this.rb.rotation * deltaRotation);
        }
    }

    abstract protected void itemEffect(Vector3 position);
}

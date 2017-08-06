using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemRocket : itemTemplate 
{

    [SerializeField]
    GameObject explosionPrefab;

    [SerializeField]
    float maxTime;

    float currTime;

    public void Start()
    {
        base.Start();
        this.rb.AddForce(transform.forward * Speed);
    }

    void Update()
    {
        this.currTime += Time.deltaTime;

        if (this.currTime > this.maxTime && this.maxTime != 0)
        {
            this.itemEffect(transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        this.itemEffect(collision.contacts[0].point);
    }

    override protected void itemEffect(Vector3 position)
    {
        GameObject explosion = Instantiate(this.explosionPrefab, position, Quaternion.identity);

        Destroy(explosion, 5f);
        Destroy(gameObject);
    }
}

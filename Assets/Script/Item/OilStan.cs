using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilStan : itemTemplate 
{

    Vector3 defaultSize = Vector3.one;

    [SerializeField]
    LayerMask floorLayer;

    [SerializeField]
    float destroyTime;

    void Start()
    {
        Vector3 oilPos = transform.position;
        //oilPos.y = 0;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10f, floorLayer))
        {
            oilPos.y = hit.point.y + 0.01f;
        }

        transform.position = oilPos;

        Destroy(gameObject, destroyTime);

    }

    void Update()
    {
        if (transform.localScale != defaultSize)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, defaultSize, Time.deltaTime * 2);

            if (transform.localScale.magnitude < 1.6f)
            {
                transform.Rotate(new Vector3(0, 10, 0));
            }
        }
    }

    override protected void itemEffect(Vector3 position)
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Car"))
        {
            other.gameObject.SendMessageUpwards("slipCar");
            Destroy(gameObject);
        }
    }
}

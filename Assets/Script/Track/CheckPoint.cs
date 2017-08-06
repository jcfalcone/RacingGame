using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    [SerializeField]
    float distanceToActivate = 10;

    GameObject[] player;

    bool complete = false;

    void Start()
    {
        this.player = GameObject.FindGameObjectsWithTag("Car");
    }
	
	// Update is called once per frame
	void Update () 
    {
        for (int count = 0; count < this.player.Length; count++)
        {
            if (Vector3.Distance(this.player[count].transform.position, transform.position) < distanceToActivate)
            {
                //ControlMaster.instance.AddCheckpoint(gameObject.GetInstanceID());
                this.player[count].SendMessageUpwards("addCheckPoint", gameObject.GetInstanceID());
                complete = true;
            }
        }
	}

    public void reset()
    {
        complete = false;
    }
}

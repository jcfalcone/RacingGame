using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDisplayKart : MonoBehaviour 
{
    [SerializeField]
    float rotateSpeed;

    [SerializeField]
    Renderer[] displayRenders;
	
	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(0, Time.deltaTime * rotateSpeed, 0);
	}

    public void changeMaterialColor(Color newColor)
    {
        for (int count = 0; count < this.displayRenders.Length; count++)
        {
            this.displayRenders[count].material.color = newColor;
        }
    }
}

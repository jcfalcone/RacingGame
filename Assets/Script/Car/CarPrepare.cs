using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPrepare : MonoBehaviour {

    [SerializeField]
    Renderer kartBodyRender;

    [SerializeField]
    Renderer kartAerofolioRender;

    [SerializeField]
    Renderer kartParalamaRender;

    [SerializeField]
    Transform charParent;

    CarTemplate carController;
	
	
    public void SetUpKart(KartMaterial kartMaterial)
    {
        this.kartBodyRender.material = kartMaterial.body;
        this.kartAerofolioRender.material = kartMaterial.aerofolio;
        this.kartParalamaRender.material = kartMaterial.paralama;

        GameObject tempChar = Instantiate(kartMaterial.charPrefab, this.charParent);
        tempChar.transform.localPosition = Vector3.zero;

        this.carController = GetComponent<CarTemplate>();

        this.carController.SetUpKart(kartMaterial);
    }
}

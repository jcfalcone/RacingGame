using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData> {

    [SerializeField]
    public GameObject kartPlayerPrefab;

    [SerializeField]
    public GameObject kartAIPrefab;

    [SerializeField]
    public float initialExp = 10f;

    [SerializeField]
    public float requiredExpFactor = 2.5f;

    //[SerializeField]
    //public KartMaterial[] kartMaterials;

    public int currKart;

    public int playerExp;
    public int playerLvl;

	// Use this for initialization
	void Start () 
    {
        //check if GameManager instance already exists in Scene
        if(instance != null)
        {
            //GameManager exists,delete copy
            DestroyImmediate(gameObject);
        }
        else
        {
            //assign GameManager to variable "_instance"
            instance = this;
        }	


        if (!PlayerPrefs.HasKey("PlayerExp"))
        {
            PlayerPrefs.SetInt("PlayerExp", 0);
        }


        if (!PlayerPrefs.HasKey("PlayerLvl"))
        {
            PlayerPrefs.SetInt("PlayerLvl", 0);
        }

        this.playerExp = PlayerPrefs.GetInt("PlayerExp");
        this.playerLvl = PlayerPrefs.GetInt("PlayerLvl");
	}

    public int checkPlayerExp()
    {
        int requiredExp = (int)Mathf.Round(((float)playerLvl * initialExp) * requiredExpFactor);

        if (this.playerExp > requiredExp)
        {
            this.playerLvl = (int)Mathf.Round(requiredExp / (int)(initialExp * requiredExpFactor));
        }

        return this.playerLvl;
    }



    public int requiredLevelExp(int level)
    {
        int requiredExp = (int)Mathf.Round(((float)level * initialExp) * requiredExpFactor);

        return requiredExp;
    }

    public void saveExp()
    {
        PlayerPrefs.SetInt("PlayerExp", this.playerExp);
        PlayerPrefs.SetInt("PlayerLvl", this.playerLvl);
    }
}

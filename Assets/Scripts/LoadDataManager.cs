using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDataManager : Singleton<LoadDataManager>
{
    [SerializeField]
    public LoadDataLang[] translations;

    [SerializeField]
    public List<string> translationsKeys;

    [SerializeField]
    public List<KartMaterial> kartMaterials;

    [SerializeField]
    public LoadDataParam parameters;

    [SerializeField]
    public string excelCode;

    [SerializeField]
    public int tabCodeTranslation;

    [SerializeField]
    public int tabCodeParameters;

    [SerializeField]
    public int tabCodeKarts;

    int m_currLanguage;

    public int currLanguage
    {
        get
        {
            return m_currLanguage;
        }
        private set
        {
            m_currLanguage = value;
        }
    }

    public LoadDataLang currTranslations
    {
        get
        {
            return this.translations[this.currLanguage];
        }
    }

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

            DontDestroyOnLoad(gameObject);

            StartCoroutine(loadData());
            //assign GameManager to variable "_instance"
            instance = this;
        }
	}

    IEnumerator loadData()
    {
        yield return StartCoroutine(Datamanager.loadDataCSV(Datamanager.DataType.Translation, this.excelCode, this.tabCodeTranslation));

        if (!string.IsNullOrEmpty(Datamanager.lastError))
        {
            yield break;
        }

        this.translations = Datamanager.language.ToArray();
        this.translationsKeys = Datamanager.languageKeys;

        yield return StartCoroutine(Datamanager.loadDataCSV(Datamanager.DataType.Data, this.excelCode, this.tabCodeParameters));

        if (!string.IsNullOrEmpty(Datamanager.lastError))
        {
            yield break;
        }

        this.parameters = Datamanager.dataParam;

        if(!PlayerPrefs.HasKey("PlayerLang"))
        {
            PlayerPrefs.SetInt("PlayerLang", this.translationsKeys.IndexOf(Datamanager.currLanguage()));
        }

        this.currLanguage = PlayerPrefs.GetInt("PlayerLang");

        StartCoroutine(DataSceneManager.instance.loadMainMenu());
    }

    public void updateLang(string lang)
    {
        int indexOf = this.translationsKeys.IndexOf(lang);

        if (indexOf != -1)
        {
            PlayerPrefs.SetInt("PlayerLang", indexOf);
            this.currLanguage = indexOf;
        }
    }

    public T getParameter<T>(string paramKey, T defaultVal)
    {

        if (!parameters.hasKey(paramKey))
        {
            return defaultVal;
        }

        string type = parameters.getType(paramKey);
        string paramVal = parameters.getValue(paramKey);

        object valueObj = new object();

        switch (type)
        {
            case "INT":
                valueObj = int.Parse(paramVal);
                break;
            case "FLOAT":
                valueObj = float.Parse(paramVal);
                break;
            case "STRING":
                valueObj = (string)paramVal;
                break;
            default:
                Debug.LogWarning("Parameter Type " + type + " not supported at line ");
                break;
        }

        T value = (T)valueObj;

        return value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LoadDataManager))]
public class LoadDataManagerCustomEditor : Editor {

    protected LoadDataManager myObj;

    int currTab;
    int currIndex;
    Vector2 scrollPos;

    IEnumerator backRoutine;

    GUIStyle errorStyle;

    string lastError;

    int currDownload = 0;

    string excelCode;

    int tabCodeTranslation;

    int tabCodeParameters;

    int tabCodeKarts;

    void Awake()
    {
        this.errorStyle = EditorStyles.boldLabel;
        //this.errorStyle.normal.textColor = Color.red;
    }

    void OnEnable()
    {
        this.myObj = (LoadDataManager)target;
        this.excelCode = this.myObj.excelCode;
        this.tabCodeTranslation = this.myObj.tabCodeTranslation;
        this.tabCodeParameters = this.myObj.tabCodeParameters;
        this.tabCodeKarts = this.myObj.tabCodeKarts;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Google Data", EditorStyles.boldLabel);

        this.excelCode = EditorGUILayout.TextField("Excel Code", this.excelCode);

        this.tabCodeTranslation = EditorGUILayout.IntField("Tab Translation Id", this.tabCodeTranslation);

        this.tabCodeParameters = EditorGUILayout.IntField("Tab Parameters Id", this.tabCodeParameters);

        this.tabCodeKarts = EditorGUILayout.IntField("Tab Karts Id", this.tabCodeKarts);

        if (this.excelCode != this.myObj.excelCode)
        {
            this.myObj.excelCode = this.excelCode;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        if (this.tabCodeTranslation != this.myObj.tabCodeTranslation)
        {
            this.myObj.tabCodeTranslation = this.tabCodeTranslation;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        if (this.tabCodeParameters != this.myObj.tabCodeParameters)
        {
            this.myObj.tabCodeParameters = this.tabCodeParameters;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        if (this.tabCodeKarts != this.myObj.tabCodeKarts)
        {
            this.myObj.tabCodeKarts = this.tabCodeKarts;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        this.currTab = GUILayout.Toolbar (this.currTab, new string[] {"Translations", "Data", "Karts"});

        bool blockBtn = false;
        //Test
        if (this.backRoutine != null)
        {
            blockBtn = this.backRoutine.MoveNext();

            if (Event.current.type == EventType.Layout && !blockBtn)
            {
                if (!string.IsNullOrEmpty(Datamanager.lastError))
                {
                    this.lastError = Datamanager.lastError;
                }
                else
                {
                    this.lastError = null;
                    this.backRoutine = null;

                    Debug.Log(currDownload);

                    if (currDownload == 0)
                    {
                        this.myObj.translations = Datamanager.language.ToArray();
                        this.myObj.translationsKeys = Datamanager.languageKeys;
                    }
                    else if (currDownload == 1)
                    {
                        this.myObj.parameters = Datamanager.dataParam;
                    }
                    else if (currDownload == 2)
                    {
                        this.myObj.kartMaterials = Datamanager.karts;
                    }

                    if (currDownload == 0)
                    {
                        this.backRoutine = Datamanager.loadDataCSV(Datamanager.DataType.Data, this.excelCode, this.tabCodeParameters);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        currDownload++;
                    }
                    else if (currDownload == 1)
                    {
                        Datamanager.karts = this.myObj.kartMaterials;
                        this.backRoutine = Datamanager.loadDataCSV(Datamanager.DataType.Kart, this.excelCode, this.tabCodeKarts);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        currDownload++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        if (!string.IsNullOrEmpty(this.lastError))
        {
            this.drawError(this.lastError);
        }

        switch (this.currTab)
        {
            case 0: // Translations Tab

                if (this.myObj.translations == null || this.myObj.translations.Length <= 0|| this.myObj.translations[currIndex].translationsKey.Count <= 0)
                {
                    this.drawEmpty("No Translation found");
                }
                else
                {
                    this.drawLanguages();
                }
                break;
            case 1: // Data Tab
                if (this.myObj.parameters == null || this.myObj.parameters.paramKeys.Count <= 0)
                {
                    this.drawEmpty("No data found");
                }
                else
                {
                    this.drawParameters();
                }
                break;
            case 2:
                if (this.myObj.kartMaterials == null || this.myObj.kartMaterials.Count <= 0)
                {
                    this.drawEmpty("No Kart found");
                }
                else
                {
                    this.drawKarts();
                }
                break;
        }

        if (blockBtn)
        {
            EditorGUI.BeginDisabledGroup(true);
        }

        if (GUILayout.Button("Refresh Data"))
        {
            this.backRoutine = Datamanager.loadDataCSV(Datamanager.DataType.Translation, this.excelCode, this.tabCodeTranslation);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            this.currDownload = 0;
        }

        if (blockBtn)
        {
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.EndVertical();
    }

    public void drawError(string error)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(error, EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

    }

    public void drawEmpty(string message)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(message, EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public void drawLanguages()
    {
        EditorGUILayout.BeginHorizontal();
        this.currIndex = EditorGUILayout.Popup("Language:", currIndex, this.myObj.translationsKeys.ToArray());
        EditorGUILayout.LabelField("Total Translations:"+this.myObj.translations[currIndex].translationsKey.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator ();

        this.scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int count = 0; count < this.myObj.translations[currIndex].translationsKey.Count; count++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.myObj.translations[currIndex].translationsKey[count], EditorStyles.boldLabel);
            EditorGUILayout.TextField(this.myObj.translations[currIndex].translationsText[count]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    public void drawParameters()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Total Parameters:"+this.myObj.parameters.paramKeys.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator ();

        this.scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int count = 0; count < this.myObj.parameters.paramKeys.Count; count++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.myObj.parameters.paramKeys[count], EditorStyles.boldLabel);
            EditorGUILayout.Popup(this.myObj.parameters.Types.IndexOf(this.myObj.parameters.paramTypes[count]), this.myObj.parameters.Types.ToArray());
            EditorGUILayout.TextField(this.myObj.parameters.paramValues[count].ToString());
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    public void drawKarts()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Total Karts:"+this.myObj.kartMaterials.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator ();

        this.scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int count = 0; count < this.myObj.kartMaterials.Count; count++)
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Max Speed",this.myObj.kartMaterials[count].maxSpeed);
            EditorGUILayout.FloatField("Max Torque",this.myObj.kartMaterials[count].maxTorque);
            EditorGUILayout.FloatField("Max Reverse Speed",this.myObj.kartMaterials[count].maxReverseSpeed);
            EditorGUILayout.FloatField("Grass Speed",this.myObj.kartMaterials[count].maxGrassSpeed);
            EditorGUILayout.FloatField("Deceleration Speed",this.myObj.kartMaterials[count].decelerationSpeed);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Char", EditorStyles.boldLabel);
            this.myObj.kartMaterials[count].charPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", this.myObj.kartMaterials[count].charPrefab, typeof(GameObject));

            EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
            this.myObj.kartMaterials[count].aerofolio = (Material)EditorGUILayout.ObjectField("Aerofolio", this.myObj.kartMaterials[count].aerofolio, typeof(Material));
            this.myObj.kartMaterials[count].body      = (Material)EditorGUILayout.ObjectField("Body", this.myObj.kartMaterials[count].body, typeof(Material));
            this.myObj.kartMaterials[count].paralama  = (Material)EditorGUILayout.ObjectField("Paralama", this.myObj.kartMaterials[count].paralama, typeof(Material));

            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
}

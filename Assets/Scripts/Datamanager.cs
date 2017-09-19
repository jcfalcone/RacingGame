using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Datamanager 
{

    public enum DataType
    {
        Translation,
        Data,
        Kart
    }

    public static List<string> m_languageKeys;

    public static List<string> languageKeys
    {
        get
        {
            return m_languageKeys;
        }

        private set
        {
            m_languageKeys = value;
        }
    }

    public static List<LoadDataLang> m_language;

    public static List<LoadDataLang> language
    {
        get
        {
            return m_language;
        }

        private set
        {
            m_language = value;
        }
    }

    public static List<KartMaterial> m_karts;

    public static List<KartMaterial> karts
    {
        get
        {
            return m_karts;
        }

        set
        {
            m_karts = value;
        }
    }

    public static string lastError;

    public static LoadDataParam dataParam;

    public static IEnumerator loadDataCSV(DataType type, string excelCode, int tabCode)
    {
        return loadDataCSV(type, excelCode, tabCode.ToString());
    }

    public static void loadDataCSVNow(DataType type, string excelCode, int tabCode)
    {
        loadDataCSVNow(type, excelCode, tabCode.ToString());
    }

    public static void loadDataCSVNow(DataType type, string excelCode, string tabCode)
    {
        WWW file = new WWW(string.Format("https://docs.google.com/spreadsheets/d/{0}/export?gid={1}&format=csv", excelCode, tabCode));

        while (!file.isDone);

        if (file.error.Length > 0)
        {
            lastError = file.error;
            return;
        }

        lastError = null;

        switch (type)
        {
            case DataType.Translation:
                processTranslation(file.text);
                break;
            case DataType.Data:
                processParameters(file.text);
                break;
            default:
                Debug.LogWarning("Type " + type.ToString() + " not supported!");
                break;
        }
    }

    public static IEnumerator loadDataCSV(DataType type, string excelCode, string tabCode)
    {
        WWW file = new WWW(string.Format("https://docs.google.com/spreadsheets/d/{0}/export?gid={1}&format=csv", excelCode, tabCode));

        while (!file.isDone)
        {
            yield return null;
        }

        if (!string.IsNullOrEmpty(file.error))
        {
            lastError = file.error;
            yield break;
        }

        lastError = null;

        switch (type)
        {
            case DataType.Translation:
                processTranslation(file.text);
                break;
            case DataType.Data:
                processParameters(file.text);
                break;
            case DataType.Kart:
                processKarts(file.text);
                break;
            default:
                Debug.LogWarning("Type " + type.ToString() + " not supported!");
                break;
        }
    }

    static void processTranslation(string text)
    {
        language     = new List<LoadDataLang>();
        languageKeys = new List<string>();
        string[] lines = text.Split('\n');

        int lastIndexOf = -1;

        for (int count = 1; count < lines.Length; count++)
        {
            string[] columns = lines[count].Split(',');

            string key = null, languageSymbol = null, value = null;

            for (int countCol = 0; countCol < columns.Length; countCol++)
            {
                switch (countCol)
                {
                    case 0:
                        key = columns[countCol];
                        break;
                    case 1:
                        languageSymbol = columns[countCol].ToUpper();
                        break;
                    case 2:
                        value = columns[countCol];
                        break;
                }
            }

            lastIndexOf = languageKeys.IndexOf(languageSymbol);

            if (lastIndexOf == -1)
            {
                language.Add(new LoadDataLang());
                languageKeys.Add(languageSymbol);

                lastIndexOf = languageKeys.Count - 1;
            }

            language[lastIndexOf].addValue(key, value);
        }
    }

    static void processParameters(string text)
    {
        dataParam = new LoadDataParam();
        string[] lines = text.Split('\n');

        for (int count = 1; count < lines.Length; count++)
        {
            string[] columns = lines[count].Split(',');

            string key = null, type = null, value = null;

            for (int countCol = 0; countCol < columns.Length; countCol++)
            {
                switch (countCol)
                {
                    case 0:
                        key = columns[countCol];
                        break;
                    case 1:
                        type = columns[countCol].ToUpper();
                        break;
                    case 2:
                        switch (type)
                        {
                            case "INT":
                            case "FLOAT":
                            case "STRING":
                                value = columns[countCol];
                                break;
                            default:
                                Debug.LogWarning("Parameter Type " + type + " not supported at line " + count);
                                break;
                        }
                        break;
                }
            }

            if (dataParam.Types.IndexOf(type) == -1)
            {
                dataParam.Types.Add(type);
            }

            dataParam.addValue(key, type, value);
        }
    }

    static void processKarts(string text)
    {
        string[] lines = text.Split('\n');

        if (karts == null)
        {
            karts = new List<KartMaterial>();
        }

        for (int count = 1; count < lines.Length; count++)
        {
            string[] columns = lines[count].Split(',');

            string key = null, type = null, value = null;

            KartMaterial kartMat = new KartMaterial();

            for (int countCol = 0; countCol < columns.Length; countCol++)
            {
                switch (countCol)
                {
                    case 0:
                        float.TryParse(columns[countCol], out kartMat.maxSpeed);
                        break;
                    case 1:
                        float.TryParse(columns[countCol], out kartMat.maxTorque);
                        break;
                    case 2:
                        float.TryParse(columns[countCol], out kartMat.maxReverseSpeed);
                        break;
                    case 3:
                        float.TryParse(columns[countCol], out kartMat.decelerationSpeed);
                        break;
                    case 4:
                        float.TryParse(columns[countCol], out kartMat.maxGrassSpeed);
                        break;
                }
            }

            if (karts.Count < count)
            {
                Debug.Log("New Kart");
                karts.Add(kartMat);
            }
            else
            {
                Debug.Log(count);
                kartMat.charPrefab = karts[count - 1].charPrefab;
                kartMat.aerofolio  = karts[count - 1].aerofolio;
                kartMat.body       = karts[count - 1].body;
                kartMat.paralama   = karts[count - 1].paralama;

                karts[count - 1] = kartMat;
            }
        }
    }

    public static string currLanguage()
    {
        return Application.systemLanguage.ToString().ToUpper();
    }
}

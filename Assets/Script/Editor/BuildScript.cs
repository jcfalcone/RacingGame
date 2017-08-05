using UnityEditor;
using UnityEngine;

public class BuildScript 
{
    static string[] devScene = { "Assets/Scene/Game.unity" };
    static string[] relScene = { "Assets/Scene/Game.unity" };
	// Use this for initialization

    #region IOS Build
    [MenuItem("Build/IOs/Development", false, 1)]
    static void BuildiOSDev () 
    {
        BuildiOS (true);
    }

    [MenuItem("Build/IOs/Release", false, 2)]
    static void BuildiOSRelease () 
    {
        BuildiOS (false);
    }

    [MenuItem("Build/IOs/All", false, 14)]
    static void BuildiOSAll () 
    {
        BuildiOS (true);
        BuildiOS (false);
    }

    static void BuildiOS (bool isDev) 
    {
        string buildPath = Application.dataPath.Replace("/Assets", "/Builds")+"iOS";

        if (isDev)
            buildPath += "Dev";

        buildPath += "_"+PlayerSettings.bundleVersion.Replace(".","_");

        BuildPipeline.BuildPlayer(isDev ? devScene : relScene, buildPath, BuildTarget.iOS, isDev ? BuildOptions.Development : BuildOptions.None);
	}
    #endregion

    #region Android Build
    [MenuItem("Build/Android/Development", false, 1)]
    static void BuildAndroidDev () 
    {
        BuildAndroid (true);
    }

    [MenuItem("Build/Android/Release", false, 2)]
    static void BuildAndroidRelease () 
    {
        BuildAndroid (false);
    }

    [MenuItem("Build/Android/All", false, 14)]
    static void BuildAndroidAll () 
    {
        BuildAndroid (true);
        BuildAndroid (false);
    }

    static void BuildAndroid (bool isDev) 
    {
        string buildPath = Application.dataPath.Replace("/Assets", "/Builds")+"Android";

        buildPath += "_"+PlayerSettings.bundleVersion.Replace(".","_");

        if (isDev)
            buildPath += "Dev";

        BuildPipeline.BuildPlayer(isDev ? devScene : relScene, buildPath, BuildTarget.Android, isDev ? BuildOptions.Development : BuildOptions.None);
    }
    #endregion

    #region WebGL Build
    [MenuItem("Build/WebGL/Development", false, 1)]
    static void BuildWebDev () 
    {
        BuildWeb (true);
    }

    [MenuItem("Build/WebGL/Release", false, 2)]
    static void BuildWebRelease () 
    {
        BuildWeb (false);
    }

    [MenuItem("Build/WebGL/All", false, 14)]
    static void BuildWebAll () 
    {
        BuildWeb (true);
        BuildWeb (false);
    }

    static void BuildWeb (bool isDev) 
    {
        string buildPath = Application.dataPath.Replace("/Assets", "/Builds")+"WebGL";

        if (isDev)
            buildPath += "Dev";

        buildPath += "_"+PlayerSettings.bundleVersion.Replace(".","_");

        BuildPipeline.BuildPlayer(isDev ? devScene : relScene, buildPath, BuildTarget.WebGL, isDev ? BuildOptions.Development : BuildOptions.None);
    }
    #endregion


    [MenuItem("Build/Build All", false, 14)]
    static void BuildAll () 
    {
        BuildiOSAll();
        BuildAndroidAll();
        BuildWebAll();
    }
}

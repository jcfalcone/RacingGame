using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenShoot : EditorWindow {


    string screenShootPath;
    string folderPath;

    int count = 0;

    [MenuItem("Util/Take ScreenShoot %#p")]
    void takeScreenShoot()
    {
        string mask = string.Format("screenshoot_{0}.png", count);

        System.IO.Directory.CreateDirectory(screenShootPath + "/" + folderPath);
        ScreenCapture.CaptureScreenshot(screenShootPath + "/" + folderPath + "/"+mask);
        count++;

        Debug.Log("Screenshoot created "+screenShootPath + "/" + folderPath);
    }

    [MenuItem("Window/ScreenShoot")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ScreenShoot>("ScreenShoot");
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        screenShootPath = EditorGUILayout.TextField("Path", screenShootPath);

        if (GUILayout.Button("..."))
        {
            screenShootPath = EditorUtility.OpenFolderPanel("Path to Screenshoots", "", "");
        }
        GUILayout.EndHorizontal();

        folderPath = EditorGUILayout.TextField("Folder Name", folderPath);

        if (GUILayout.Button("Take ScreenShoot"))
        {
            takeScreenShoot();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAnimationEnd : MonoBehaviour {

    public void OnEndAnimation()
    {
        Debug.Log("PT1");
        UIManager.instance.ShowStartNumber();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialHint {

    public GameObject hint;
    public CanvasGroup hintGroup;
    public int waitTime;
    public bool hasSubHints;
    public bool useOverlay;
    public Vector2 overlayPos;
    public Vector3 overlayScale;
    public CanvasGroup[] subHintList;
    public int[] subHintWaitTime;
}

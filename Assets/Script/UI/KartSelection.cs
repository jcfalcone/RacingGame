using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KartSelection : MonoBehaviour 
{
    [SerializeField]
    Vector3 startPos;

    [SerializeField]
    Vector3 displayPos;

    [SerializeField]
    Vector3 endPos;


    [SerializeField]
    float speed;

    [SerializeField]
    Button selectCharBtn;

    [SerializeField]
    Image lockedImage;

    [SerializeField]
    TextMeshProUGUI lockedTxt;

    [SerializeField]
    MainMenuDisplayKart[] kartDisplay;


    bool changeKart = false;
    bool dirKart = true;
    int currKart = 0;
    public int selectKart = 0;

    [SerializeField]
    int nextKart = 1;

    [SerializeField]
    Color normalColor;

    [SerializeField]
    Color blockedColor;

    private const float MIN_SWIPE_LENGTH = 50f;
    private const float MAX_SWIPE_TIME = 0.35f;
    private Vector3 _mouseStartPos;
    private float _elapsedTime;
    private bool _startTimer;


    private string defaultBlockTxt;

	// Use this for initialization
	void Start () 
    {
        this.kartDisplay[this.currKart].gameObject.SetActive(true);
        this.kartDisplay[this.currKart].transform.localPosition = this.displayPos;

        this.defaultBlockTxt = this.lockedTxt.text;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (this.changeKart)
        {
            this.kartDisplay[this.currKart].transform.localPosition = Vector3.Lerp(  this.kartDisplay[this.currKart].transform.localPosition,
                                                                                (this.dirKart) ? this.endPos : this.startPos,
                                                                                Time.deltaTime * this.speed);
            
            this.kartDisplay[this.nextKart].transform.localPosition = Vector3.Lerp(  this.kartDisplay[this.nextKart].transform.localPosition,
                                                                                this.displayPos,
                                                                                Time.deltaTime * this.speed);

            if (Vector3.Distance(this.kartDisplay[this.currKart].transform.localPosition, (this.dirKart) ? this.endPos : this.startPos) < 0.1f)
            {
                this.kartDisplay[this.currKart].gameObject.SetActive(false);
                this.changeKart = false;
                this.currKart = this.nextKart;
            }
        }

        checkSwip();
    }

    void checkSwip()
    {
        #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        this.mobileSwipe();
        #else
        this.SimulateMouseSwipe();
        #endif
    }

    private void mobileSwipe()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startTimer = true;
                _elapsedTime = 0f;
            }

            if (touch.phase == TouchPhase.Ended && _elapsedTime < MAX_SWIPE_TIME)
            {

                this.changeCar(touch.deltaPosition);
            }
        }
    }

    private void SimulateMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseStartPos = Input.mousePosition;
            _startTimer = true;
            _elapsedTime = 0f;
        }

        if (Input.GetMouseButtonUp(0) && _elapsedTime < MAX_SWIPE_TIME)
        {
            _startTimer = false;

            Vector3 mouseEndPos = Input.mousePosition;
            Vector3 direction = (mouseEndPos - _mouseStartPos);

            this.changeCar(direction);
        }
    }

    private void changeCar(Vector3 pDirection)
    {
        // Allows only for swipes Up, Down, Left and Right
        if (pDirection.x > 0 && pDirection.y > -50f && pDirection.y < 50f)
        {
            this.changeNextKart();
        }
        else if (pDirection.x < 0 && pDirection.y > -50f && pDirection.y < 50f)
        {
            this.changeNextKart();
        }
    }

    public void changePrevKart()
    {
        if (nextKart > 0)
        {
            if (this.changeKart)
            {
                this.kartDisplay[this.currKart].gameObject.SetActive(false);
            }

            this.changeKart = true;
            this.dirKart = false;

            this.currKart = this.nextKart;

            nextKart--;
        }

        if (nextKart < 0)
        {
            nextKart = 0;
        }

        if (this.changeKart)
        {
            this.selectKart = this.nextKart;
            this.kartDisplay[this.nextKart].gameObject.SetActive(true);
        }

        if (this.nextKart > PlayerData.instance.playerLvl)
        {
            this.lockedImage.gameObject.SetActive(true);
            this.lockedTxt.text  = this.defaultBlockTxt.Replace("[REQLVL]", (this.nextKart + 1).ToString());
            this.selectCharBtn.interactable = false;
            //this.kartDisplay[this.nextKart].changeMaterialColor(blockedColor);
        }
        else
        {
            this.lockedImage.gameObject.SetActive(false);
            this.selectCharBtn.interactable = true;
            //this.kartDisplay[this.nextKart].changeMaterialColor(normalColor);
        }
    }

    public void changeNextKart()
    {

        if (nextKart < this.kartDisplay.Length - 1)
        {

            if (this.changeKart)
            {
                this.kartDisplay[this.currKart].gameObject.SetActive(false);
            }

            this.changeKart = true;
            this.dirKart = true;

            this.currKart = this.nextKart;

            nextKart++;
        }

        if (nextKart >= this.kartDisplay.Length)
        {
            nextKart = this.kartDisplay.Length - 1;
        }

        if (this.changeKart)
        {
            this.selectKart = this.nextKart;
            this.kartDisplay[this.nextKart].gameObject.SetActive(true);
        }

        if (this.nextKart > PlayerData.instance.playerLvl)
        {
            this.lockedImage.gameObject.SetActive(true);
            this.lockedTxt.text  = this.defaultBlockTxt.Replace("[REQLVL]", (this.nextKart + 1).ToString());
            this.selectCharBtn.interactable = false;
            //this.kartDisplay[this.nextKart].changeMaterialColor(blockedColor);
        }
        else
        {
            this.lockedImage.gameObject.SetActive(false);
            this.selectCharBtn.interactable = true;
            //this.kartDisplay[this.nextKart].changeMaterialColor(normalColor);
        }
    }
}

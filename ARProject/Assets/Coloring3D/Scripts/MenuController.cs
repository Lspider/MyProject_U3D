using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vuforia;

public class MenuController : MonoBehaviour {
    public Text pinText;
    public Text titleText;

    [SerializeField]
    private GameObject imageTarget;

    #region properity
    private bool pin;
    public bool Pin
    {
        get
        {
            return pin;
        }
        set
        {
            pin = value;
        }
    }

    private bool trackingFound;
    public bool TrackingFound
    {
        get
        {
            return trackingFound;
        }
        set
        {
            trackingFound = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        pin = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnPin()
    {
        if (pin)//取消了 
        {
            pin = false;
            pinText.text = "锁定";
            imageTarget.GetComponent<ImageTargetBehaviour>().enabled = true;
        }
        else if (Pin == false && TrackingFound)//可以锁定
        {
            pin = true;
            pinText.text = "取消";
            imageTarget.GetComponent<ImageTargetBehaviour>().enabled = false;
        }

        
    }

    public virtual void SetTitle(string text)
    {
        titleText.text = text;

        Invoke("ResetTitle", 5.0f);
    }

    private void ResetTitle()
    {
        //guideText.text = "";
    }
}

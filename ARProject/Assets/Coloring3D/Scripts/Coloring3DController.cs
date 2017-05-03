using UnityEngine;
using System.Collections;

public class Coloring3DController : MonoBehaviour {
    public GameObject target;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTrackingFound()
    {
        this.GetComponent<MenuController>().TrackingFound = true;
        target.SetActive(true);
    }

    public void OnTrackingLost()
    {
        this.GetComponent<MenuController>().TrackingFound = false;
        if (!this.GetComponent<MenuController>().Pin)
        {
            target.SetActive(false);
        }
    }
}

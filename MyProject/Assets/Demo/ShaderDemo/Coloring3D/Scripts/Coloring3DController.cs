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
       
        target.SetActive(true);
    }

    public void OnTrackingLost()
    {
        target.SetActive(false);
    }
}

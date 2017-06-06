using UnityEngine;
using System.Collections;

public class PluginTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            UnityPluginUtils.ShowToast(".......！", false);
        }
    }
}

using UnityEngine;
using System.Collections;

public class MessageBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnFinished()
    {
        Debug.Log("hello world!");
    }

    void OnFinished(bool ret) {
        Debug.Log("hello world!" + ret.ToString());
    }

    public void OnAlertBox() {
        System.Action action = OnFinished;
        MessageBox.ShowAlertBox("message", "title", action);
    }

    public void OnConfirmBox()
    {
        System.Action<bool> action = OnFinished;
        MessageBox.ShowConfirmBox("message", "title", action);
    }
}

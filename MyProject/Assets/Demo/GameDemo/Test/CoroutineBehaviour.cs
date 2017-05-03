using UnityEngine;
using System.Collections;

public class CoroutineBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //test
        // 协同程序WaitAndPrint在Start函数内执行,可以视同于它与Start函数同步执行.
        StartCoroutine(WaitAndPrint(2.0f));
        print("Before WaitAndPrint Finishes " + Time.time);
    }

    IEnumerator WaitAndPrint(float waitTime)
    {
        // 暂停执行waitTime秒
        yield return new WaitForSeconds(waitTime);
        print("WaitAndPrint " + Time.time);
    }

    // Update is called once per frame
    void Update () {
	
	}
}

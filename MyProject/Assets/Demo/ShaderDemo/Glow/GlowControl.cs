using UnityEngine;
using System.Collections;

public class GlowControl : MonoBehaviour {
    private Material mat;
    private float value;
    [SerializeField]
    float speed=1;

	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
	
	}
	
	// Update is called once per frame
	void Update () {

        value = Mathf.PingPong(Time.time * speed, 5);
      ///  Debug.Log(value);
        mat.SetFloat("_Strength", value);
	
	}
}

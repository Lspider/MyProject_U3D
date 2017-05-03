using UnityEngine;
using System.Collections;

public class Gizmos_Color : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDrawGizmosSelected()
    {
        //在物体的前方绘制一个5米长的线
        Gizmos.color = Color.red;
        var direction = transform.TransformDirection(Vector3.forward) * 5;  
        Gizmos.DrawRay(transform.position, direction);

        //在变换位置处绘制一个变透明的蓝色立方体
        Gizmos.color = new Color(1, 0, 0, 5);    
        Gizmos.DrawCube(transform.position, new Vector3(2, 2, 2));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;//为随后绘制的gizmos设置颜色。
        Gizmos.DrawWireSphere(transform.position, 2.25f);//使用center和radius参数，绘制一个线框球体。
    }
}

using UnityEngine;
using System.Collections;

public class GyroBehaviour : MonoBehaviour {

    bool draw = false;
    bool gyinfo;
    Gyroscope go;

    // Use this for initialization
    void Start () {
        gyinfo = SystemInfo.supportsGyroscope;
        go = Input.gyro;
        go.enabled = true;

        
    }
	
	// Update is called once per frame
	void Update () {
        if (gyinfo)
        {
            Vector3 a = go.attitude.eulerAngles;
            a = new Vector3(-a.x, -a.y, a.z); //直接使用读取的欧拉角发现不对，于是自己调整一下符号  
            this.transform.eulerAngles = a;
            this.transform.Rotate(Vector3.right * 90, Space.World);
            draw = false;
        }
        else
        {
            draw = true;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, 100, 500, 100), "Attitude : " + Input.gyro.attitude.x + "       " + Input.gyro.attitude.y + "         " + Input.gyro.attitude.z);
        GUI.Label(new Rect(50, 250, 500, 100), "Gravity : " + Input.gyro.gravity.x + "       " + Input.gyro.gravity.y + "         " + Input.gyro.gravity.z);
        GUI.Label(new Rect(50, 350, 500, 60), "userAcceleration : " + Input.gyro.userAcceleration.x + "       " + Input.gyro.userAcceleration.y + "         " + Input.gyro.userAcceleration.z);

    }

   
}

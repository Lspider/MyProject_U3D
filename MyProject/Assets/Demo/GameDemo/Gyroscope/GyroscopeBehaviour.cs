using UnityEngine;
using System.Collections;

public class GyroscopeBehaviour : MonoBehaviour {
    private const float lowPassFilterFactor = 0.2f;
    private Quaternion rotFix;

    //mouse
    public float speedX;
    public float speedY;

    private float maxRotate = 90;
    private float minRotate = -90;

    private Quaternion currentRotation;

    protected void Start()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        //设置设备陀螺仪的开启/关闭状态，使用陀螺仪功能必须设置为 true  
        Input.gyro.enabled = true;
        //获取设备重力加速度向量  
        Vector3 deviceGravity = Input.gyro.gravity;
        //设备的旋转速度，返回结果为x，y，z轴的旋转速度，单位为（弧度/秒）  
        Vector3 rotationVelocity = Input.gyro.rotationRate;
        //获取更加精确的旋转  
        Vector3 rotationVelocity2 = Input.gyro.rotationRateUnbiased;
        //设置陀螺仪的更新检索时间，即隔 0.1秒更新一次  
        Input.gyro.updateInterval = 0.1f;
        //获取移除重力加速度后设备的加速度  
        Vector3 acceleration = Input.gyro.userAcceleration;

        //绕z轴旋转90度，Z处sin90为1，W处cos90为0
        rotFix = new Quaternion(0, 0, 1f, 0f);
        //给摄像机一个父物体，X的90度可以让移动设备端平面朝自己的时候是正角度
        transform.parent.transform.eulerAngles = new Vector3(90, 0, 0);
#else

#endif
    }

    protected void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if(Input.touchCount==1)  
        {  
            if(Input.touches[0].phase==TouchPhase.Moved)  
            {  
                var horizontal = Input.GetAxis("Mouse X") * speedX;  
                var vertical = Input.GetAxis("Mouse Y") * speedY * -1;
                vertical = ClampAngle(vertical, minRotate, maxRotate);

                transform.Rotate(vertical, horizontal, 0);
                currentRotation = transform.localRotation;
            }  
        }
        else{
            //Input.gyro.attitude 返回值为 Quaternion类型，即设备旋转欧拉角
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Input.gyro.attitude * rotFix, lowPassFilterFactor);
        }  
#else
        var horizontal = Input.GetAxis("Mouse X") * speedX;
        var vertical = Input.GetAxis("Mouse Y") * speedY * -1;

        vertical = ClampAngle(vertical, minRotate, maxRotate);

        //旋转  
        transform.Rotate(vertical, horizontal, 0);
        currentRotation = transform.localRotation;
#endif
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, 100, 500, 20), "Label : " + Input.gyro.attitude.x + "       " + Input.gyro.attitude.y + "         " + Input.gyro.attitude.z);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

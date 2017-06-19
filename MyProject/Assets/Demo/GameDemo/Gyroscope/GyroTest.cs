using UnityEngine;
using System.Collections.Generic;

public class GyroTest : MonoBehaviour
{
    Quaternion origin = Quaternion.identity;
    bool once;
    Vector3 fix, angle;
    Quaternion attitude;

    Queue<Quaternion> averageList;
    Quaternion average;

    void Start()
    {
        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.01f;
        //fix = new Vector3 (360f, 360f, 360f);

        averageList = new Queue<Quaternion>();
    }


    void Update()
    {
        attitude = Input.gyro.attitude;
        if (!once)
        {
            origin = new Quaternion(1, 1, 1, 1);
            once = true;
        }

        //Average of quaternions
        //Global variable which holds the amount of rotations which
        //need to be averaged.
        int addAmount = 0;

        //Global variable which represents the additive quaternion
        Quaternion addedRotation = Quaternion.identity;

        //The averaged rotational value
        Quaternion averageRotation = Input.gyro.attitude;

        //Loop through all the rotational values.
        if (averageList.Count == 0)
        {
            averageRotation = Input.gyro.attitude;
        }
        else
        {
            averageList.Enqueue(Input.gyro.attitude);
            if (averageList.Count > 240)
                averageList.Dequeue();
            foreach (Quaternion singleRotation in averageList)
            {

                //Temporary values
                float w;
                float x;
                float y;
                float z;

                //Amount of separate rotational values so far
                addAmount++;

                Quaternion item = singleRotation;

                if (AreQuaternionsClose(singleRotation, averageList.Peek()))
                {
                    item = InverseSignQuaternion(singleRotation);
                }

                float addDet = 1.0f / (float)addAmount;
                addedRotation.w += item.w;
                w = addedRotation.w * addDet;
                addedRotation.x += item.x;
                x = addedRotation.x * addDet;
                addedRotation.y += item.y;
                y = addedRotation.y * addDet;
                addedRotation.z += item.z;
                z = addedRotation.z * addDet;

                //Normalize. Note: experiment to see whether you
                //can skip this step.
                float D = 1.0f / (w * w + x * x + y * y + z * z);
                w *= D;
                x *= D;
                y *= D;
                z *= D;

                //The result is valid right away, without
                //first going through the entire array.
                averageRotation = new Quaternion(x, y, z, w);
            }
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, new Quaternion(averageRotation.x, averageRotation.y, -averageRotation.z, -averageRotation.w), Time.deltaTime * 4f);

        //transform.rotation = Quaternion.Inverse(origin)*Input.gyro.attitude;
        //transform.eulerAngles = fix - transform.eulerAngles;
    }

    public static Quaternion InverseSignQuaternion(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, -q.z, -q.w);
    }

    public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
    {
        float dot = Quaternion.Dot(q1, q2);
        if (dot < 0.0f)
            return false;
        else
            return true;
    }


    void OnGUI()
    {
        //        GUILayout.Label(origin.eulerAngles+" <- origin");
        GUILayout.Label(Input.gyro.attitude.eulerAngles + " <- gyro");
        GUILayout.Label(origin + " <- origin");
        GUILayout.Label(attitude + " <- raw gyro");
        GUILayout.Label(transform.localRotation.eulerAngles + " <- localRotation");
        GUILayout.Label(Input.gyro.rotationRateUnbiased + " <- rotation rate");
        GUILayout.Label(transform.eulerAngles.x + " <- camera angle X");
        GUILayout.Label(transform.eulerAngles.y + " <- camera angle Y");
        GUILayout.Label(transform.eulerAngles.z + " <- camera angle Z");
    }

}
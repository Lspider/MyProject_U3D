using UnityEngine;
using System.Collections;

public class ShowFPS : MonoBehaviour {

    public float fUpdateInterval = 0.5F;

    private float fLastInterval;
    private int nFrames = 0;
    private float fFps;

    void Start() 
    {
		//Application.targetFrameRate=60;
        fLastInterval = Time.realtimeSinceStartup;
        nFrames = 0;
    }

    void OnGUI() 
    {
        GUI.Label(new Rect(0, 100, 200, 200), "FPS:" + fFps.ToString("f2"));
    }

    void Update() 
    {
        ++nFrames;
        if (Time.realtimeSinceStartup > fLastInterval + fUpdateInterval) 
        {
            fFps = nFrames / (Time.realtimeSinceStartup - fLastInterval);
            nFrames = 0;
            fLastInterval = Time.realtimeSinceStartup;
        }
    }
}

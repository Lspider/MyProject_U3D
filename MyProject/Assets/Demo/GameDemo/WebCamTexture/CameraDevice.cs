﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using Common;

public class CameraDevice : MonoBehaviour
{
    //public RenderTexture renderTexture;
    public GameObject target;

    private WebCamTexture webCamTexture;
    private string deviceName;
    private string currentTexture;

    void Start()
    {

    }
    void Update()
    {

    }

    //绘制按钮  
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 20, 100, 40), "开启摄像头"))
        {
            // 调用摄像头
            StartCoroutine(StartCamera());
        }

        if (GUI.Button(new Rect(10, 70, 100, 40), "捕获照片"))
        {
            //捕获照片
            webCamTexture.Pause();
            StartCoroutine(GetTexture());
        }

        if (GUI.Button(new Rect(10, 120, 100, 40), "再次捕获"))
        {
            //重新开始
            webCamTexture.Play();
        }

        if (GUI.Button(new Rect(120, 20, 80, 40), "录像"))
        {
            //录像
            StartCoroutine(SeriousPhotoes());
        }

        if (GUI.Button(new Rect(10, 170, 100, 40), "停止"))
        {
            //停止捕获镜头
            webCamTexture.Stop();
            StopAllCoroutines();
        }

        if (webCamTexture != null)
        {
            // 捕获截图大小			   —距X左屏距离   |   距Y上屏距离  
            GUI.DrawTexture(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 190, 280, 200), webCamTexture);
        }
    }

    ///  
    ///调用摄像头  
    ///  
    IEnumerator StartCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            deviceName = devices[0].name;
            //设置摄像机摄像的区域    
            webCamTexture = new WebCamTexture(deviceName, 300, 300, 12);
            webCamTexture.Play();//开始摄像    
        }
    }
    /// <summary>
    /// 获取截图
    /// </summary>
    /// <returns>The texture.</returns>
    public IEnumerator GetTexture()
    {
        yield return new WaitForEndOfFrame();

        //Texture2D texture = new Texture2D(400, 300);
        //texture.ReadPixels(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 360, 300), 0, 0, false);
        ////距X左的距离		距Y屏上的距离
        //// t.ReadPixels(new Rect(220, 180, 200, 180), 0, 0, false);
        //texture.Apply();
        //byte[] data = texture.EncodeToPNG();

        //string filePath = FileUtils.GetWritePath() + "/Photos/";
        //if (!Directory.Exists(filePath))
        //{
        //    Directory.CreateDirectory(filePath);
        //    File.WriteAllBytes(filePath + Time.time + ".jpg", data);
        //}

        Texture2D texture = new Texture2D(webCamTexture.width, webCamTexture.height);
        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();
        byte[] data = texture.EncodeToPNG();

        string filePath = FileUtils.GetWritePath() + "/Photos/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        File.WriteAllBytes(filePath + Time.time + ".png", data);
        webCamTexture.Play();

       target.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }

    /// <summary>
    /// 连续捕获照片
    /// </summary>
    /// <returns>The photoes.</returns>
    public IEnumerator SeriousPhotoes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D(400, 300, TextureFormat.RGB24, true);
            t.ReadPixels(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 50, 360, 300), 0, 0, false);
            t.Apply();
            print(t);
            byte[] byt = t.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png", byt);
            Thread.Sleep(300);
        }
    }

    /// <summary>
    /// 从文件中加载纹理
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    IEnumerator LoadTexture(string filePath)
    {
        //请求WWW
        WWW www = new WWW(filePath);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //获取Texture
            Texture2D texture = www.texture;

            target.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        }
    }

}
using UnityEngine;
using System.Collections;

public class MoviePlay : MonoBehaviour {

    //视频纹理   
    protected MovieTexture movTexture;

    AudioClip audio;
    AudioSource AudioSource1;

    void Start()
    {
        StartCoroutine(DownLoadMovie());
    }

    void OnGUI()
    {
        if (GUILayout.Button("播放/继续"))
        {
            //播放/继续播放视频   
            if (!movTexture.isPlaying)
            {
                movTexture.Play();
                AudioSource1.Play();
            }
        }

        if (GUILayout.Button("暂停播放"))
        {
            //暂停播放   
            movTexture.Pause();
            AudioSource1.Pause();
        }

        if (GUILayout.Button("停止播放"))
        {
            //停止播放   
            movTexture.Stop();
            AudioSource1.Stop();
        }
    }

    IEnumerator DownLoadMovie()
    {
        WWW www = //new WWW("http://127.0.0.1/mov_bbb.ogg");
        new WWW("file://" + Application.dataPath + "/Resources/mov_bbb.ogg");  
        yield return www;
        movTexture = www.movie;

        //获取主相机的声源
        AudioSource1 = Camera.main.GetComponent(typeof(AudioSource)) as AudioSource;
        //获取视频的声音设置到声源上
        AudioSource1.clip = movTexture.audioClip;
        audio = AudioSource1.clip;


        //设置当前对象的主纹理为电影纹理   
        GetComponent<Renderer>().material.mainTexture = movTexture;
        //设置电影纹理播放模式为循环 
        movTexture.loop = true;
    }
}

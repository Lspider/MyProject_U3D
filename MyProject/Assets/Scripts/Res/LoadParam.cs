using UnityEngine;
using System.Collections;

public class LoadParam {
    // 加载的文件类型  
    public string fileType;
    // 加载的路径  
    public string url;

    public int priotiry;

    // 自定义参数  
    public object param = null;
    // 图片  
    public Texture2D texture2D;
    // 文本  
    public string text = "";
    // unity3d格式文件，目前针对场景打包的unity3d格式文件  
    public AssetBundle assetBundle = null;
    // json文件  
    public string jsonData;
    // 二进制文件  
    public byte[] byteArr;
    // 音频文件  
    public AudioClip audioClip;
    // 模块资源打包格式文件  
    //public UIAtlas uiAtlas;
    // fbx打包的文件对象  
    public UnityEngine.Object mainAsset;
    // font文件  
    //public UIFont font;
}

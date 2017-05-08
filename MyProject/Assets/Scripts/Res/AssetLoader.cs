#define USE_UNITY5_X_BUILD
// #define USE_LOWERCHAR
#define USE_HAS_EXT
#define USE_DEP_BINARY
#define USE_DEP_BINARY_AB
#define USE_ABFILE_ASYNC

// 是否使用LoadFromFile读取压缩AB
#define USE_LOADFROMFILECOMPRESS

//#define USE_WWWCACHE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common;
using System.Xml;

class AssetBundleInfo
{
	public string bundleName;
	public string assetName;
	public List<string> dependencies;

	public void AddDependence(string dep)
	{
		if (dependencies == null) {
			dependencies = new List<string> ();
		}
		dependencies.Add (dep);
	}
}

/// <summary>
/// Asset loader 动更资源加载器
/// </summary>
public class AssetLoader : IResourceLoader
{
    private Dictionary<string, AssetBundleInfo> fileMap = new Dictionary<string, AssetBundleInfo>();

    private string assetPath;
    /// <summary>
    ///  初始化加载器
    /// </summary>
    /// <param name="fileList">文件列表</param>
    /// <param name="initOK">初始化完成回调</param>
    public void Initialize(string resoucePath, string fileList, Action initOK)
    {
        assetPath = resoucePath;
        // 
        string fileName = "StreamingAssets.xml";
        string path = Path.Combine(assetPath, fileName);
        LoadAssetXml(path);
    }

    /// <summary>
    /// 同步加载动更资源
    /// </summary>
    /// <returns>资源内容</returns>
    /// <param name="fileName">文件名称</param>
    /// <typeparam name="T">资源类型</typeparam>
    public override T LoadAsset<T>(string fileName)
    {
        AssetBundleInfo info = null;
        fileMap.TryGetValue(fileName, out info);
        if (null == info)
        {
            return null;
        }

        T t = null;
        string path = Path.Combine(assetPath, info.bundleName);
#if UNITY_ANDROID && !UNITY_EDITOR
        List<string> pathList = new List<string>();
        pathList.Add(path);
        if (null != info.dependencies)
        {
            foreach (string dep in info.dependencies)
            {
               path = Path.Combine(assetPath, dep);
               pathList.Add(path);
            }
        }
        ResourceManager.Instance.LoadResource(pathList);
#else
        AssetBundle asset = LoadAssetBundle(path);
        if (null == asset)
        {
            return null;
        }

        if (null != info.dependencies)
        {
            foreach (string dep in info.dependencies)
            {
                try
                {
                    AssetBundle _asset = LoadAssetBundle(path);
                }
                catch
                {
                    continue;
                }
            }
        }
        
        try
        {
            t = asset.LoadAsset<T>(info.assetName);
        }
        catch { }
#endif

        return t;
    }

    /// <summary>
    /// 异步加载动更资源
    /// </summary>
    /// <returns>资源内容</returns>
    /// <param name="fileName">文件名称</param>
    /// <typeparam name="T">资源类型</typeparam>
    public override T LoadAssetAsync<T>(string fileName)
    {
        string path = Path.Combine(assetPath, fileName);
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        if (request.isDone)
        {
            T t = null;
            try
            {
                t = request.assetBundle.LoadAsset<T>(fileName);
            }
            catch { }

            return t;
        }

        return null;
    }

    /// <summary>
    /// 读取文本文件内容
    /// </summary>
    /// <returns>The text.</returns>
    /// <param name="fileName">File name.</param>
    public override string LoadText(string fileName)
    {
        throw new NotImplementedException();
    }


    void LoadAssetXml(string filePath)
    {
        XmlDocument doc = new XmlDocument();
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("++++++++++++LoadAssetXml");
        WWW www = new WWW(filePath);
        while (!www.isDone) { }
        doc.LoadXml(www.text);
#else
        if (!System.IO.File.Exists(filePath))
        {
            return;
        }

        doc.Load(filePath);
#endif
        XmlNodeList nodeList = doc.SelectSingleNode("files").ChildNodes;
        foreach (XmlElement xe in nodeList)
        {
            AssetBundleInfo info = new AssetBundleInfo();
            string _fileName = xe.SelectSingleNode("fileName").InnerText;
            info.assetName = xe.SelectSingleNode("assetName").InnerText;
            info.bundleName = xe.SelectSingleNode("bundleName").InnerText;
            XmlNode deps = xe.SelectSingleNode("deps");
            if (null != deps)
            {
                XmlNodeList depList = deps.ChildNodes;
                foreach (XmlElement _xe in depList)
                {
                    info.AddDependence(_xe.InnerText);
                }
            }
            fileMap.Add(_fileName.Substring("Assets/Resources/".Length), info);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    AssetBundle LoadAssetBundle(string filePath) {
        AssetBundle asset = null;
#if UNITY_ANDROID && !UNITY_EDITOR
        //TODO:
#else
        if (!System.IO.File.Exists(filePath))
        {
            return null;
        }

        asset = AssetBundle.LoadFromFile(filePath);
#endif

        return asset;
    }
}
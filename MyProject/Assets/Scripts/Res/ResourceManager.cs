using System;
using Common;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

//资源管理类型
public enum ResManagerType
{
    NO,               //无
    ABB,              //Asset Bundle Build
    ABB_UPDATE,       //Asset Bundle Build and Compress to update
    All_UPDATE        //All can update(res and code)
}

public class ResourceManager:Singleton<ResourceManager>
{

    //temp for andorid
    public delegate void LoadResouceDelegate(List<string> pathList);
    public static event LoadResouceDelegate loadResouce; //委托的事件

    private AssetLoader m_assetLoader = new AssetLoader();
	private ResourcesLoader m_resourceLoader = new ResourcesLoader();

    private ResManagerType resManagerType;
    public ResManagerType ManagerType {
        get{ return resManagerType; }
    }

    private string resourcePath;
    public string ResourcePath {
        get { return resourcePath; }
    }

    public ResourceManager ()
	{
        resManagerType = ResManagerType.ABB;
        Initialize();
        if (resManagerType != ResManagerType.NO)
        {
            m_assetLoader.Initialize(resourcePath, "", null);
        }
    }

    void Initialize()
    {
        if (resManagerType == ResManagerType.ABB)
        {
            resourcePath = FileUtils.GetStreamingAssetsPath();//Application.streamingAssetsPath;// 
        }
        else if (resManagerType == ResManagerType.ABB_UPDATE)
        {
            resourcePath = Path.Combine(FileUtils.GetWritePath(), "StreamingAssets");

            //文件解压
            if (!Directory.Exists(resourcePath))
            {
                Directory.CreateDirectory(resourcePath);
                string streamingAssets = Application.streamingAssetsPath;
                string[] files = Directory.GetFiles(streamingAssets, "*", SearchOption.AllDirectories);
                foreach (string filePath in files)
                {
                    string path = filePath.Replace("\\", "/");
                    string fileName = filePath.Substring(streamingAssets.Length + 1);
                    if (fileName.EndsWith(".zip"))
                    {
                        int lastDotIndex = fileName.LastIndexOf(".");
                        string outputName = fileName.Substring(0, lastDotIndex + 1) + "unity3d";
                        FileUtils.DecompressFileLZMA(filePath, Path.Combine(resourcePath, outputName));
                    }
                    else if (fileName.EndsWith("StreamingAssets.xml"))
                    {
                        File.Copy(filePath, Path.Combine(resourcePath, fileName));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 同步加载动更资源
    /// </summary>
    /// <returns>资源内容</returns>
    /// <param name="fileName">文件名称</param>
    /// <typeparam name="T">资源类型</typeparam>
    public T LoadAsset<T> (string fileName) where T: UnityEngine.Object
	{
		T rtn = m_assetLoader.LoadAsset<T>(fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadAsset<T>(fileName);
	}

	/// <summary>
	/// 异步加载动更资源
	/// </summary>
	/// <returns>资源内容</returns>
	/// <param name="fileName">文件名称</param>
	/// <typeparam name="T">资源类型</typeparam>
	public T LoadAssetAsync<T> (string fileName) where T: UnityEngine.Object
	{
		T rtn = m_assetLoader.LoadAssetAsync<T> (fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadAssetAsync<T> (fileName);
	}

	/// <summary>
	/// 读取文本文件内容
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="fileName">File name.</param>
	public string LoadText (string fileName)
	{
		string rtn = m_assetLoader.LoadText (fileName);
		if (null != rtn) {
			return rtn;
		}

		return m_resourceLoader.LoadText (fileName);
	}

    //temp
    public void LoadResource(List<string> pathList) {
        loadResouce(pathList);
    }
}


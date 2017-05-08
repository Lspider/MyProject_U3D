using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LoadManager {

    public static LoadManager instance;
    public int MAX_LOAD_REQUEST = 2;

    private Dictionary<string, LoadRequest> loadDict = new Dictionary<string, LoadRequest>();
    private Dictionary<string, LoadRequest> waitDict = new Dictionary<string, LoadRequest>();
    private Dictionary<string, LoadParam> completeDict = new Dictionary<string, LoadParam>();

    private List<string> priorityList = new List<string>();

    private bool isLoading;
    public LoadManager()
    {
        loadDict.Clear();
        waitDict.Clear();
        completeDict.Clear();

        Application.backgroundLoadingPriority = ThreadPriority.Low;
        // add timer to check download queue  
        //maxx-m
        //FrameTimerManager.getInstance().add(1, 0, CheckQueue);
    }

    /// <summary>  
    ///  返回实例  
    /// </summary>  
    /// <returns></returns>  
    public static LoadManager getInstance()
    {
        if (instance == null)
        {
            instance = new LoadManager();
        }
        return instance;
    }

    /// <summary>  
    /// 根据优先级，从等待队列里面移除一个任务到下载队列里  
    /// </summary>  
    public void MoveRequestFromWaitDictToLoadDict()
    {
        isLoading = loadDict.Count > 0;
        if (priorityList.Count > 0)
        {
            if (waitDict.ContainsKey(priorityList[0]))
            {
                LoadRequest request = waitDict[priorityList[0]];
                waitDict.Remove(priorityList[0]);
                priorityList.RemoveAt(0);
                Load(request.requestURL, request.completeFunction, request.customParams, request.fileType, request.priotiry, request.errorFunction, request.processFunction);
            }
        }
    }

    /// <summary>  
    /// 读取资源  
    /// </summary>  
    public void Load(string url, 
        LoadRequest.DownCompleteDelegate completeFunc, 
        object customParam = null, 
        string fileType = "", 
        int priority = 2, 
        LoadRequest.ErrorDelegate errorFunc = null, 
        LoadRequest.ProcessDelegate processFunc = null)
    {
        url = url.Trim();
        if (string.IsNullOrEmpty(url)) return;

        if (completeDict.ContainsKey(url))
        {
            // 已下载资源，直接调用回调函数  
            if (customParam != null)
            {
                completeDict[url].param = customParam;
            }
            try
            {
                completeFunc.Invoke(completeDict[url]);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("exception:" + exception.Message);
            }
        }
        else if (loadDict.ContainsKey(url))
        {
            // 已经提交相同请求，但是没有下载完成  
            loadDict[url].completeFunction += completeFunc;
            loadDict[url].processFunction += processFunc;
            loadDict[url].errorFunction += errorFunc;
            loadDict[url].customParams.Add(customParam);
        }
        else if (waitDict.ContainsKey(url))
        {
            // 已经提交相同请求，但是还没轮到加载  
            loadDict[url].completeFunction += completeFunc;
            loadDict[url].processFunction += processFunc;
            loadDict[url].errorFunction += errorFunc;
            loadDict[url].customParams.Add(customParam);
        }
        else
        {
            // 未加载过的  
            if (loadDict.Count < MAX_LOAD_REQUEST)
            {
                isLoading = true;
                LoadRequest loadRequest = new LoadRequest(url, customParam, fileType, completeFunc, errorFunc, processFunc);
                if (customParam != null && customParam.GetType().ToString() == "System.Collections.Generic.List`1[System.Object]")
                {
                    loadRequest.customParams = (List<object>)customParam;
                }
                loadDict.Add(url, loadRequest);
            }
            else
            {
                // 已达到最大加载数目，加入等待队列  
                LoadRequest loadRequest = new LoadRequest();
                loadRequest.requestURL = url;
                loadRequest.completeFunction = completeFunc;
                loadRequest.errorFunction = errorFunc;
                loadRequest.processFunction = processFunc;
                loadRequest.customParams.Add(customParam);
                loadRequest.fileType = fileType;
                loadRequest.priotiry = priority;
                waitDict.Add(url, loadRequest);
                priorityList.Add(url);
                //maxx-m
                //priorityList = priorityList.OrderBy(s => waitDict[s].priotiry).ToList();
            }
        }
    }

    /// <summary>  
    ///  下载错误回调  
    /// </summary>  
    public void ErrorDelegateHandle(LoadRequest request)
    {
        if (request.errorFunction != null)
        {
            int count = request.errorFunction.GetInvocationList().GetLength(0);
            for (int i = 0; i < count; i++)
            {
                LoadRequest.ErrorDelegate errorFunc = (LoadRequest.ErrorDelegate)request.errorFunction.GetInvocationList()[i];
                try
                {
                    errorFunc.Invoke(request);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("exception:" + e.Message);
                }
            }
        }
    }

    /// <summary>  
    ///  下载进度回调  
    /// </summary>  
    public void ProcessDelegateHandle(LoadRequest request)
    {
        if (request.processFunction != null)
        {
            int count = request.processFunction.GetInvocationList().GetLength(0);
            for (int i = 0; i < count; i++)
            {
                LoadRequest.ProcessDelegate processFunc = (LoadRequest.ProcessDelegate)request.processFunction.GetInvocationList()[i];
                try
                {
                    processFunc.Invoke(request.wwwObject.progress, request.wwwObject.bytesDownloaded);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("exception:" + e.Message);
                }
            }
        }
    }

    /// <summary>  
    ///  任务下载完成回调  
    /// </summary>  
    /// <param name="request"></param>  
    /// <param name="param"></param>  
    public void CompleteDelegateHandle(LoadRequest request, LoadParam param)
    {
        if (request.completeFunction != null)
        {
            int count = request.completeFunction.GetInvocationList().GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (i < request.customParams.Count)
                {
                    param.param = request.customParams[i];
                }
                LoadRequest.DownCompleteDelegate completeFunc = (LoadRequest.DownCompleteDelegate)request.completeFunction.GetInvocationList()[i];
                try
                {
                    completeFunc.Invoke(param);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("exception:" + exception.Message);
                }
            }
        }
    }

    /// <summary>  
    ///  解析下载内容  
    /// </summary>  
    public LoadParam ParseLoadParamFromLoadRequest(LoadRequest request)
    {
        LoadParam param = new LoadParam();
        param.url = request.requestURL;
        param.priotiry = request.priotiry; // 为何param需要记录优先级？  
        param.fileType = request.fileType;

        switch (request.fileType)
        {
            case LoadFileType.IMAGE:
                try
                {
                    param.texture2D = request.wwwObject.texture;
                    param.texture2D.Compress(false);    // compress 有何影响  
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read texture2d error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.TXT:
                try
                {
                    param.text = request.wwwObject.text;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read text error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.UNITY3D:
                try
                {
                    if (request.wwwObject.assetBundle != null)
                    {
                        param.assetBundle = request.wwwObject.assetBundle;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read assetBundle error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.MODULE_RESOURCE:
                try
                {
                    //UnityEngine.Object[] data = request.wwwObject.assetBundle.LoadAllAssets();
                    //int length = data.Length;
                    //for (int i = 0; i < length; i++)
                    //{
                    //    if (data[i] is GameObject)
                    //    {
                    //        param.uiAtlas = (data[i] as GameObject).GetComponent<UIAtlas>();
                    //        break;
                    //    }
                    //}
                    //request.wwwObject.assetBundle.Unload(false);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read uiatlas error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.JSON:
                try
                {
                    param.jsonData = request.wwwObject.text.Trim();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read  json error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.FBX:
                try
                {
                    param.mainAsset = request.wwwObject.assetBundle.mainAsset;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read  fbx error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.BINARY:
            case LoadFileType.BINARY_BG:
                try
                {
                    param.byteArr = request.wwwObject.bytes;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read  binary error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.AUDIO:
                try
                {
                    UnityEngine.Object[] data = request.wwwObject.assetBundle.LoadAllAssets();
                    int length = data.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (data[i] is AudioClip)
                        {
                            param.audioClip = data[i] as AudioClip;
                            break;
                        }
                    }
                    request.wwwObject.assetBundle.Unload(false);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read audio error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
            case LoadFileType.FONT:
                try
                {
                    //UnityEngine.Object[] data = request.wwwObject.assetBundle.LoadAllAssets();
                    //int length = data.Length;
                    //for (int i = 0; i < length; i++)
                    //{
                    //    if (data[i] is UnityEngine.Transform)
                    //    {
                    //        param.font = (data[i] as UnityEngine.Transform).GetComponent<UIFont>();
                    //        break;
                    //    }
                    //}
                    //request.wwwObject.assetBundle.Unload(false);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("read  font error:" + request.requestURL + "\n" + exception.Message);
                }
                break;
        }
        return param;
    }

    /// <summary>  
    ///  定时器下，在每帧对下载队列进行检测  
    ///  如果下载有问题，或者超时，则清除  
    ///  如果下载完成，则解析下载结果，并进入completeDict中  
    /// </summary>  
    public void CheckQueue()
    {
        if (!isLoading) return;
        foreach (KeyValuePair<string, LoadRequest> pair in loadDict)
        {
            LoadRequest request = pair.Value;
            request.loadTotalFrames++;
            
            // deal error  
            if ((request.wwwObject != null && request.wwwObject.error != null) || request.isTimeOut)
            {
                if (request.requestURL.Contains(".apk") || request.requestURL.Contains(".ipa"))
                {
                    return;
                }

                request.alreadyDeal = true;
                loadDict.Remove(request.requestURL);
                ErrorDelegateHandle(request);
                if (request.isTimeOut)
                {
                    Debug.LogWarning("Load time out:" + request.requestURL);
                }
                else
                {
                    Debug.LogWarning("Load error:" + request.requestURL);
                }

                MoveRequestFromWaitDictToLoadDict();
                break;
            }

            //   
            if (!request.alreadyDeal)
            {
                ProcessDelegateHandle(request);
                // if done  
                if (request.wwwObject != null && request.wwwObject.isDone)
                {
                    LoadParam param = ParseLoadParamFromLoadRequest(request);
                    if (request.fileType != LoadFileType.BINARY)
                    {
                        completeDict.Add(request.requestURL, param);
                    }
                    //  
                    CompleteDelegateHandle(request, param);
                    //  
                    request.alreadyDeal = true;
                    loadDict.Remove(request.requestURL);
                    MoveRequestFromWaitDictToLoadDict();
                    break;
                }
            }

        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LoadPriority {
    NO,
    LOW,
    NORMAL,
    HIGHT,
}

public class LoadRequest {

    public delegate void DownCompleteDelegate(LoadParam param);
    public delegate void ErrorDelegate(LoadRequest request);
    public delegate void ProcessDelegate(float processValue, int fileTotalSize = 0);


    public DownCompleteDelegate completeFunction;
    public ErrorDelegate errorFunction;
    public ProcessDelegate processFunction;


    public const int TIME_OUT_FRAMES = 300;
    private int _loadTotalFrames = 0; // 加载的总帧数  
    public bool isTimeOut = false;
    public bool alreadyDeal = false;

    public string requestURL;
    public string fileType;
    public WWW wwwObject = null;
    public List<object> customParams = new List<object>();
    public int priotiry = (int)LoadPriority.NORMAL;

    public LoadRequest() {
    }

    public LoadRequest(string url, object customParam = null, string type = "", DownCompleteDelegate completeFunc = null, ErrorDelegate errorFunc = null, ProcessDelegate processFunc = null)
    {
        requestURL = url;
        fileType = type;

        completeFunction = completeFunc;
        if (completeFunc != null)
            customParams.Add(customParam);
        if (errorFunc != null)
            errorFunction = errorFunc;
        if (processFunc != null)
            processFunction = processFunc;

        wwwObject = new WWW(requestURL);
        wwwObject.threadPriority = ThreadPriority.Normal;
    }

    public int loadTotalFrames
    {
        get
        {
            return _loadTotalFrames;
        }
        set
        {
            _loadTotalFrames = value;
            if (_loadTotalFrames > LoadRequest.TIME_OUT_FRAMES)
                isTimeOut = true;
        }
    }
}

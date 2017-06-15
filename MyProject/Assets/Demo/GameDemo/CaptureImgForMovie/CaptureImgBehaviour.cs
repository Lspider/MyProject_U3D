using UnityEngine;
using System.Collections;
using System.IO;
using Common;

public class CaptureImgBehaviour : MonoBehaviour {
    public int frameRate = 30;
    public string exportPath = "D://Capture/";
    public Vector2 imgSize = new Vector2(1000, 1000);

    public Camera camera;
    private int nIndex = 0;
    // Use this for initialization
    void Start () {
        float interval = 1.0f / frameRate;

        //if (!cam)
        //{
        //    GameObject go = new GameObject("__cam");
        //    cam = go.AddComponent<Camera>();
        //    go.transform.parent = transform.parent;
        //    cam.hideFlags = HideFlags.HideAndDontSave;
        //}
        //cam.CopyFrom(Camera.main);
        //cam.depth = 0;
        //cam.cullingMask = 31;

        //if (!renderTexture)
        //{
        //    renderTexture = new RenderTexture(Screen.width, Screen.height, -50);
        //}
        //cam.targetTexture = renderTexture;
        //cam.Render();

        if (camera == null) {
            camera = Camera.main;
        }

        nIndex = 0;
        InvokeRepeating("CaptureImg", 1.0f, 0.1f);
    }

    void CaptureImg() {
        //Camera.main.enabled = false;
        //string filePath = FileUtils.GetWritePath() + "/Photos/";
        string filePath = exportPath;
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        string fileName = filePath + nIndex.ToString("D8") + ".PNG";
        StartCoroutine(CaptureByCamera(camera, new Rect(0, 0, imgSize.x, imgSize.y), fileName));

        nIndex++;
    }

    private IEnumerator CaptureByCamera(Camera mCamera, Rect mRect, string mFileName)
    {
        //等待渲染线程结束  
        yield return new WaitForEndOfFrame();

        //初始化RenderTexture  
        RenderTexture mRender = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
        //设置相机的渲染目标  
        mCamera.targetTexture = mRender;
        //开始渲染  
        mCamera.Render();

        //激活渲染贴图读取信息  
        RenderTexture.active = mRender;

        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.ARGB32, false);
        //读取屏幕像素信息并存储为纹理数据  
        mTexture.ReadPixels(mRect, 0, 0);
        //应用  
        mTexture.Apply();

        //释放相机，销毁渲染贴图  
        mCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(mRender);

        //将图片信息编码为字节信息  
        byte[] bytes = mTexture.EncodeToPNG();
        //保存  
        System.IO.File.WriteAllBytes(mFileName, bytes);

        //如果需要可以返回截图  
        //return mTexture;  
    }
}

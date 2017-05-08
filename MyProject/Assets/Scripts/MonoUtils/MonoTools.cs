using UnityEngine;
using System.Collections;

public class MonoTools : MonoBehaviour
{
    #region 截图
    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    Texture2D CaptureCamera(Camera camera, Rect rect)
    {
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        camera.targetTexture = rt;
        camera.Render();
        //多个相机混合 
        //camera2.targetTexture = rt;  
        //camera2.Render();  

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        //保存
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏: {0}", filename));

        return screenShot;
    }

    #endregion

   
}
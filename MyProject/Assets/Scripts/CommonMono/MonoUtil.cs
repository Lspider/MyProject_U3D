using UnityEngine;
using System.Collections;

public class MonoUtil : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
       
    }

    #region 创建多边形
    /// <summary>
    /// 创建多边形
    /// BetterCreateMul(16, 1);
    /// CreateMul(6, 1);
    /// 
    /// </summary>
    /// <param name="len"></param>
    /// <param name="weith"></param>
    /// 
    private Material material;
    void CreateMul(int len, float weith)
    {
        GameObject obj = new GameObject("six");
        float angle = 360.0f / len;
        Vector3 dir = Vector3.zero;
        Vector3 forward = Vector3.forward;
        Vector3[] vertices = new Vector3[len + 1];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < vertices.Length; i++)
        {
            var rot = Quaternion.Euler(0, angle * (i - 1), 0);
            print(rot.eulerAngles);
            dir = rot * forward;
            vertices[i] = dir * weith;
            //print(Vector3.Angle(Vector3.forward,dir));
        }
        int[] triangles = new int[(vertices.Length - 1) * 3];
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = 0;
            }
            else
            {
                int z = i / 3;
                int y = i % 3;
                int f = z + y;
                if (f > vertices.Length - 1)
                {
                    f = 1;
                }
                triangles[i] = f;
            }
        }
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        obj.AddComponent<MeshRenderer>().material = material;
        mf.sharedMesh = mesh;
    }
    #endregion

    #region 创建多边形
    /// <summary>
    /// 创建多边形
    /// </summary>
    /// <param name="len"></param>
    /// <param name="weith"></param>
    void BetterCreateMul(int len, float weith)
    {
        GameObject obj = new GameObject("six");
        float angle = 360.0f / len;
        Vector3 dir = Vector3.zero;
        Vector3 forward = Vector3.forward;
        Vector3[] vertices = new Vector3[len];
        for (int i = 0; i < vertices.Length; i++)
        {
            var rot = Quaternion.Euler(0, angle * i, 0);
            dir = rot * forward;
            vertices[i] = dir * weith;
        }
        int[] triangles = new int[(vertices.Length - 2) * 3];
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = vertices.Length - 1;

            }
            else
            {
                int z = (i - 1) / 3;
                int y = (i - 1) % 3;
                int f = z + y;
                triangles[i] = f;
            }
        }
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        obj.AddComponent<MeshRenderer>().material = material;
        mf.sharedMesh = mesh;
    }
    #endregion

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

    #region Tap Count Check
    private int mTapCount = 0;
    private float mTimeSinceLastTap = 0;
    private const float DOUBLE_TAP_MAX_DELAY = 0.5f;
    private int  HandleTap()
    {
        int nTapCount = 0;
        if (mTapCount == 1)
        {
            mTimeSinceLastTap += Time.deltaTime;
            if (mTimeSinceLastTap > DOUBLE_TAP_MAX_DELAY)
            {
                // too late for double tap, 
                // we confirm it was a single tap
                nTapCount = mTapCount;

                // reset touch count and timer
                mTapCount = 0;
                mTimeSinceLastTap = 0;
            }
        }
        else if (mTapCount == 2)
        {
            // we got a double tap
            nTapCount = mTapCount;

            // reset touch count and timer
            mTimeSinceLastTap = 0;
            mTapCount = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mTapCount++;
            if (mTapCount == 1)
            {
                nTapCount = mTapCount;
            }
        }

        return mTapCount;
    }
    #endregion
}
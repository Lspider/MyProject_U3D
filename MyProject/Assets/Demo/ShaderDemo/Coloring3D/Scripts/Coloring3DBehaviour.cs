using UnityEngine;
///maxx-m using Vuforia;

public class Coloring3DBehaviour : MonoBehaviour
{
    Camera cam;
    RenderTexture renderTexture;
    ///maxx-m ImageTargetBehaviour targetBehaviour;

    void Start()
    {
        ///maxx-m targetBehaviour = GameObject.Find("ImageTarget").GetComponent<ImageTargetBehaviour>(); //GetComponentInParent<ImageTargetBehaviour>();
        gameObject.layer = 31;
    }

    void Renderprepare()
    {
        if (!cam)
        {
            GameObject go = new GameObject("__cam");
            cam = go.AddComponent<Camera>();
            go.transform.parent = transform.parent;
            cam.hideFlags = HideFlags.HideAndDontSave;
        }
        cam.CopyFrom(Camera.main);
        cam.depth = 0;
        cam.cullingMask = 31;

        if (!renderTexture)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, -50);
        }
        cam.targetTexture = renderTexture;
        cam.Render();
        GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);
    }

    void OnWillRenderObject()
    {
        ///maxx-m if (!targetBehaviour || targetBehaviour.ImageTarget == null)
        ///maxx-m return;
        ///maxx-m Vector2 halfSize = targetBehaviour.GetSize() * 0.5f;//(0.5,0.3)
        //Vector3 targetAnglePoint1 = transform.parent.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.y));
        //Vector3 targetAnglePoint2 = transform.parent.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.y));
        //Vector3 targetAnglePoint3 = transform.parent.TransformPoint(new Vector3(halfSize.x, 0, halfSize.y));
        //Vector3 targetAnglePoint4 = transform.parent.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.y));
        //Renderprepare();
        //GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        //GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        //GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        //GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
    }

    void OnDestroy()
    {
        if (renderTexture)
            DestroyImmediate(renderTexture);
        if (cam)
            DestroyImmediate(cam.gameObject);
    }
}


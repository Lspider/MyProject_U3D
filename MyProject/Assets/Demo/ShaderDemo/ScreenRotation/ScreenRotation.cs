using UnityEngine;
using System.Collections;
//using UnityStandardAssets.ImageEffects;

public class ScreenRotation : MonoBehaviour
{
    public Material material;
    public float rotation;

    // Update is called once per frame
    void Update()
    {
        rotation += 0.1f;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (rotation == 0.0)
            return;

        material.SetFloat("_Rotation", rotation);

        Graphics.Blit(source, destination, material);
    }

}
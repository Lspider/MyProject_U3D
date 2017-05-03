using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageClip : MonoBehaviour
{
    public Image image;

    private Material material;
    // Use this for initialization
    void Start()
    {
        //material = GetComponent<Image>().material;
        material = image.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int col = material.GetInt("_ColIndex");

            col++;
            if (col >= 4)
                col = 0;

            material.SetInt("_ColIndex", col);
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageClipAnimation : MonoBehaviour
{
    public Image image;

    private Material material;
    private int rowCount;
    private int colCount;

    private int col = 0;
    private int row = 0;
    private float time = 0;
    // Use this for initialization
    void Start()
    {
        //material = GetComponent<Image>().material;
        material = image.material;
        rowCount = material.GetInt("_RowCount");
        colCount = material.GetInt("_ColCount");
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.1)
        {
            time = 0;
            ChangeImage();
        }
    }

    void ChangeImage()
    {
        material.SetInt("_ColIndex", col);
        material.SetInt("_RowIndex", row);

        col++;
        if (col >= colCount)
        {
            col = 0;

            row++;
            if (row >= rowCount)
            {
                row = 0;
            }
        }
    }
}
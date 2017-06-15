using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateBehaviour : MonoBehaviour {

    class ConstNumber {
        public static int PointSum = 6;
        public static int XLength = 6;
        public static int YLength = 6;
    }

    // 顶点集合
    private Vector3[] vertices = new Vector3[ConstNumber.PointSum];

    // 三角形索引 一共(ConstNumber.XLength - 1) * (ConstNumber.YLength - 1)个矩形 ，每个矩形6个点
    private int[] triangles = new int[6 * (ConstNumber.XLength - 1) * (ConstNumber.YLength - 1)];

    private Color[] colors = new Color[ConstNumber.PointSum];

    // Use this for initialization
    void Start()
    {
        CreateMeshFun();
    }

    public void CreateMeshFun()
    {
        Mesh mesh = new Mesh();
        mesh = CreateMeshObject();
        AssetDatabase.CreateAsset(mesh, "Assets/" + "Pollutant" + ".asset");

        //打印新建资源的路径
        // Debug.Log(AssetDatabase.GetAssetPath(mesh));
    }

    private Mesh CreateMeshObject()
    {
        Mesh mesh = new Mesh();

        // 初始化顶点位置
        initVertexPos();

        //三角形索引
        initTriangles();

        //初始化颜色
        initColors();

        // 顶点赋值
        mesh.vertices = vertices;
        // 索引赋值
        mesh.triangles = triangles;

        // 对顶点着色
        mesh.colors = colors;
        // 重新计算网格的法线
        mesh.RecalculateNormals();
        return mesh;
    }

    // 初始化顶点位置
    private void initVertexPos()
    {
        int currentIndex = 0;

        for (int i = 0; i < ConstNumber.YLength; i++)
        {
            for (int j = 0; j < ConstNumber.XLength; j++)
            {
                vertices[currentIndex] = new Vector3(j, 0, i);
                currentIndex++;
            }
        }
    }

    // 初始化三角形索引
    private void initTriangles()
    {
        // 代表triangl数组当前索引值，每放入数组中一个值，currentIndex都增1
        int currentIndex = 0;

        for (int i = 0; i < ConstNumber.YLength - 1; i++)
        {
            for (int j = 0; j < ConstNumber.XLength - 1; j++)
            {
                // 顺时针画左上角三角形
                triangles[currentIndex++] = (i + 0) * ConstNumber.XLength + (j + 0);
                triangles[currentIndex++] = (i + 1) * ConstNumber.XLength + (j + 0);
                triangles[currentIndex++] = (i + 1) * ConstNumber.XLength + (j + 1);

                // 顺时针画右下角三角形
                triangles[currentIndex++] = (i + 0) * ConstNumber.XLength + (j + 0);
                triangles[currentIndex++] = (i + 1) * ConstNumber.XLength + (j + 1);
                triangles[currentIndex++] = (i + 0) * ConstNumber.XLength + (j + 1);
            }
        }

    }

    // 初始化颜色
    private void initColors()
    {
        for (int i = 0; i < ConstNumber.PointSum; i++)
        {
            Color color = new Color(0, 0, 0);
            colors[i] = color;
        }
    }
}

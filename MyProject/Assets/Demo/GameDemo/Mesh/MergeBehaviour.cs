using UnityEngine;
using System.Collections;

public class MergeBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //获取MeshRender
        MeshRenderer[] meshRenders = GetComponentsInChildren<MeshRenderer>();

        //材质
        Material[] mats = new Material[meshRenders.Length];
        for (int i = 0; i < meshRenders.Length; i++)
        {
            mats[i] = meshRenders[i].sharedMaterial;
        }

        //合并Mesh
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        transform.gameObject.AddComponent<MeshRenderer>();
        transform.gameObject.AddComponent<MeshFilter>();
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
        transform.gameObject.SetActive(true);

        transform.GetComponent<MeshRenderer>().sharedMaterials = mats;
    }
}

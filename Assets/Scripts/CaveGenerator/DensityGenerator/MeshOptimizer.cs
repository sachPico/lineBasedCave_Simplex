using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshOptimizer
{
    public static void CombineMesh(GameObject holderParent)
    {
        MeshFilter[] meshFilters = holderParent.transform.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            meshFilters[i].gameObject.SetActive(false);
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(true);

            i++;
        }
        holderParent.transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        holderParent.transform.GetComponent<MeshFilter>().sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        holderParent.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        //holderParent.transform.gameObject.SetActive(true);
        int iteration = holderParent.transform.childCount;
        for(i=0 ; i<iteration; i++)
        {
            MonoBehaviour.DestroyImmediate(holderParent.transform.GetChild(0).gameObject);
        }
        holderParent.GetComponent<MeshCollider>().sharedMesh = holderParent.GetComponent<MeshFilter>().sharedMesh;
    }
}
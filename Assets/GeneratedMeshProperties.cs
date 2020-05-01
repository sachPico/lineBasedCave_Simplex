using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedMeshProperties : MonoBehaviour
{
    public Mesh _generatedMesh;
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;

    public void Initiate(Material mat)
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        if(_meshFilter==null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if(_meshRenderer==null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        //_meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //_generatedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _generatedMesh = _meshFilter.sharedMesh;
        _meshRenderer.material = mat;
    }

    public void RefreshMeshFilter(Mesh newMesh)
    {
        if(newMesh!=null)   _meshFilter.sharedMesh = newMesh;
    }
}

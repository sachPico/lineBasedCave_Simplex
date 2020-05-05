using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedMeshProperties : MonoBehaviour
{
    public Mesh _generatedMesh;
    MeshCollider _collider;
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;

    public void Initiate(Material mat)
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();

        if(_collider==null)
        {
            _collider = gameObject.AddComponent<MeshCollider>();
        }
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
        _collider.sharedMesh = _generatedMesh;
    }

    public void RefreshMeshFilter(Mesh newMesh)
    {
        _collider.enabled = false;
        if(newMesh!=null)
        {
            _meshFilter.sharedMesh = newMesh;
        }
        _collider.enabled = true;
    }
}

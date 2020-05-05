using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshOptimizer
{
    public static Mesh CombineMesh(Mesh mA, Mesh mB)
    {
        Mesh combinedMesh = new Mesh();
        if(mA.vertices.Length + mB.vertices.Length < Mathf.Pow(2,16))
        {
            CombineInstance[] _combine = new CombineInstance[2];
            _combine[0].mesh = mA;
            _combine[1].mesh = mB;
            combinedMesh.CombineMeshes(_combine);
            return combinedMesh;
        }
        else
        {
            return null;
        }
    }
}
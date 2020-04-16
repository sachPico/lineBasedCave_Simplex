using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePoints : MonoBehaviour
{
    public List<Vector3> samplePoints, inRangePoints;
    public LineGenerator _lineGenerator;
    public float voxelSize, boundaryLength, boundaryHeight, boundaryWidth;
    public int totalVoxel;

    void OnEnable()
    {
        _lineGenerator = gameObject.GetComponent<LineGenerator>();
        totalVoxel = (int)(boundaryLength/voxelSize)*(int)(boundaryWidth/voxelSize)*(int)(boundaryHeight/voxelSize);
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for(int i=0; i<(int)boundaryLength/voxelSize; i++)
        {
            for(int j=0; j<(int)boundaryHeight/voxelSize; j++)
            {
                for(int k=0; k<(int)boundaryWidth/voxelSize; k++)
                {
                    Gizmos.DrawCube(transform.position+new Vector3(
                        (int)(boundaryLength/voxelSize)*i,
                        (int)(boundaryHeight/voxelSize)*j,
                        (int)(boundaryWidth/voxelSize)*k)
                        , new Vector3(voxelSize*.75f,voxelSize*.75f,voxelSize*.75f)
                    );
                }
            }
        }
    }*/

    void OnValidate()
    {
        voxelSize = Mathf.Clamp(voxelSize,.00000001f, 1f);
        //CreateSamplePoint();
    }
}

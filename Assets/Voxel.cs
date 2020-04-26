﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel:MonoBehaviour
{
    static uint maxVertex = 858993459 ;
    public float size;
    public int voxelsPerAxis;
    [Header("Unit voxel position")]
    public Vector3 voxelPosition;
    public Vector3Int voxelGrid;
    [Header("Voxel boundary center")]
    public Vector3 boundaryCenter;
    public Vector3 boundaryWorldPos;
    //[Header("Voxels size")]
    //public Vector3Int numVoxel;
    public Vector3Int numVertex;
    [Space()]
    public long totalVoxel;
    public int totalVertex;
     [Space()]
    public bool showGizmos;
    [HideInInspector]
    public Node[] voxelNodes = new Node[8];
    [HideInInspector]
    public int totalVertexLayer;
    

    private Vector3Int oldNumVoxel;

    public void SetBoundaryCenter()
    {
        boundaryCenter.x = size/2f*voxelsPerAxis;
        boundaryCenter.y = size/2f*voxelsPerAxis;
        boundaryCenter.z = size/2f*voxelsPerAxis;
    }

    public void InitiateVoxelNodes()
    {
        voxelPosition.x = size*voxelGrid.x;
        voxelPosition.y = size*voxelGrid.y;
        voxelPosition.z = size*voxelGrid.z;

        voxelNodes[0].SetPos(voxelPosition);
        voxelNodes[1].SetPos(voxelPosition+(Vector3.right*size));
        voxelNodes[2].SetPos(voxelPosition+(Vector3.forward*size)+(Vector3.right*size));
        voxelNodes[3].SetPos(voxelPosition+(Vector3.forward*size));
        voxelNodes[4].SetPos(voxelPosition+(Vector3.up*size));
        voxelNodes[5].SetPos(voxelPosition+(Vector3.up*size)+(Vector3.right*size));
        voxelNodes[6].SetPos(voxelPosition+(Vector3.up*size)+(Vector3.forward*size)+(Vector3.right*size));
        voxelNodes[7].SetPos(voxelPosition+(Vector3.up*size)+(Vector3.forward*size));

        /*for(int i=0; i<voxelNodes.Length; i++)
        {
            voxelNodes[i]+=(Vector3.right*voxelGrid.x*size)+(Vector3.up*voxelGrid.y*size)+(Vector3.forward*voxelGrid.z*size);
        }*/

        boundaryWorldPos.x = size*numVertex.x;
        boundaryWorldPos.y = size*numVertex.y;
        boundaryWorldPos.z = size*numVertex.z;
    }

    void OnDrawGizmos()
    {
        if(showGizmos)
        {
            Gizmos.color = new Color(0,0,1,.5f);
            Gizmos.DrawCube(
                new Vector3
                (
                    (voxelGrid.x*size)+(size/2),
                    (voxelGrid.y*size)+(size/2),
                    (voxelGrid.z*size)+(size/2)
                )
                ,Vector3.one*size
            );
            for(int i=1; i<numVertex.x; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,0), new Vector3(i*size,0,numVertex.z*size));
            }
            for(int i=1; i<numVertex.z; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(numVertex.x*size,0,i*size));
            }
            for(int i=1; i<numVertex.x; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,numVertex.z*size), new Vector3(i*size,numVertex.y*size,numVertex.z*size));
            }
            for(int i=1; i<numVertex.y; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,numVertex.z*size), new Vector3(numVertex.x*size,i*size,numVertex.z*size));
            }
            for(int i=1; i<numVertex.z; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(0,numVertex.y*size,i*size));
            }
            for(int i=1; i<numVertex.y; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,0), new Vector3(0,i*size,numVertex.z*size));
            }
            //Draw boundary box gizmos
            
            //Gizmos.color = Color.black;
            //Gizmos.DrawWireCube(boundaryCenter, boundaryCenter*2);
            //Gizmos.DrawSphere(boundaryCenter,.1f);
        }
    }

    void OnValidate()
    {
        voxelGrid.x = Mathf.Clamp(voxelGrid.x, 0, voxelsPerAxis-1);
        voxelGrid.y = Mathf.Clamp(voxelGrid.y, 0, voxelsPerAxis-1);
        voxelGrid.z = Mathf.Clamp(voxelGrid.z, 0, voxelsPerAxis-1);
        numVertex.x = Mathf.Clamp(voxelsPerAxis+1, 2, 65535);
        numVertex.y = Mathf.Clamp(voxelsPerAxis+1, 2, 65535);
        numVertex.z = Mathf.Clamp(voxelsPerAxis+1, 2, 65535);
        InitiateVoxelNodes();
        SetBoundaryCenter();
        totalVoxel = voxelsPerAxis*voxelsPerAxis*voxelsPerAxis;
        totalVertexLayer = numVertex.x*numVertex.z;
        totalVertex = numVertex.y*totalVertexLayer;
        if((uint)totalVertex >= maxVertex)
        {
            Debug.LogError("Warning! Total vertex must not exceed "+(maxVertex-1)+"! Current number of vertex is = "+ (uint)totalVertex+". Number of vertex in y-dimension will be adjusted!");
        }

        var dg = this.gameObject.GetComponent<DensityGenerator>();
        dg.numThreadGroup.x = Mathf.CeilToInt(numVertex.x/8)+1;
        dg.numThreadGroup.y = Mathf.CeilToInt(numVertex.y/8)+1;
        dg.numThreadGroup.z = Mathf.CeilToInt(numVertex.z/8)+1;

    }
}

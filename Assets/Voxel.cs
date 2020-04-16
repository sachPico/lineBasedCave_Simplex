using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel:MonoBehaviour
{
    static uint maxVertex = 858993459 ;
    public float size;
    [Header("Unit voxel position")]
    public Vector3 voxelPosition;
    public int xGrid, yGrid, zGrid;
    [Header("Voxel boundary center")]
    public Vector3 boundaryCenter;
    public Vector3 boundaryWorldPos;
    [Header("Voxels size")]
    public int numVoxelX;
    public int numVoxelY;
    public int numVoxelZ;
    [Space()]
    public long totalVoxel;
    public int totalVertex;
     [Space()]
    public bool showGizmos;
    [HideInInspector]
    public Node[] voxelNodes = new Node[8];
    [HideInInspector]
    public int totalVertexLayer, numVertexX, numVertexY, numVertexZ;

    private Vector3Int oldNumVoxel;

    public void SetBoundaryCenter()
    {
        boundaryCenter.x = size/2f*numVoxelX;
        boundaryCenter.y = size/2f*numVoxelY;
        boundaryCenter.z = size/2f*numVoxelZ;
    }

    public void InitiateVoxelNodes()
    {
        voxelPosition.x = size*xGrid;
        voxelPosition.y = size*yGrid;
        voxelPosition.z = size*zGrid;

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
            voxelNodes[i]+=(Vector3.right*xGrid*size)+(Vector3.up*yGrid*size)+(Vector3.forward*zGrid*size);
        }*/

        boundaryWorldPos.x = size*numVoxelX;
        boundaryWorldPos.y = size*numVoxelY;
        boundaryWorldPos.z = size*numVoxelZ;
    }

    void OnDrawGizmos()
    {
        if(showGizmos)
        {
            Gizmos.color = new Color(0,0,1,.5f);
            Gizmos.DrawCube(
                new Vector3
                (
                    (xGrid*size)+(size/2),
                    (yGrid*size)+(size/2),
                    (zGrid*size)+(size/2)
                )
                ,Vector3.one*size
            );
            for(int i=1; i<numVoxelX; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,0), new Vector3(i*size,0,numVoxelZ*size));
            }
            for(int i=1; i<numVoxelZ; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(numVoxelX*size,0,i*size));
            }
            for(int i=1; i<numVoxelX; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,numVoxelZ*size), new Vector3(i*size,numVoxelY*size,numVoxelZ*size));
            }
            for(int i=1; i<numVoxelY; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,numVoxelZ*size), new Vector3(numVoxelX*size,i*size,numVoxelZ*size));
            }
            for(int i=1; i<numVoxelZ; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(0,numVoxelY*size,i*size));
            }
            for(int i=1; i<numVoxelY; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,0), new Vector3(0,i*size,numVoxelZ*size));
            }
            //Draw boundary box gizmos
            
            //Gizmos.color = Color.black;
            //Gizmos.DrawWireCube(boundaryCenter, boundaryCenter*2);
            //Gizmos.DrawSphere(boundaryCenter,.1f);
        }
    }

    void OnValidate()
    {
        oldNumVoxel.x = numVoxelX;
        oldNumVoxel.y = numVoxelY;
        oldNumVoxel.z = numVoxelZ;

        numVoxelX = Mathf.Clamp(numVoxelX,1,(int)(maxVertex/(oldNumVoxel.y*oldNumVoxel.z))-2);
        //oldNumVoxel.x = numVoxelX;
        numVoxelY = Mathf.Clamp(numVoxelY,1,(int)(maxVertex/(oldNumVoxel.x*oldNumVoxel.z))-2);
        //oldNumVoxel.y = numVoxelY;
        numVoxelZ = Mathf.Clamp(numVoxelZ,1,(int)(maxVertex/(oldNumVoxel.y*oldNumVoxel.x))-2);
        //oldNumVoxel.z = numVoxelZ;

        xGrid = Mathf.Clamp(xGrid, 0, numVoxelX-1);
        yGrid = Mathf.Clamp(yGrid, 0, numVoxelY-1);
        zGrid = Mathf.Clamp(zGrid, 0, numVoxelZ-1);
        numVertexX = Mathf.Clamp(numVoxelX+1, 2, 65535);
        numVertexY = Mathf.Clamp(numVoxelY+1, 2, 65535);
        numVertexZ = Mathf.Clamp(numVoxelZ+1, 2, 65535);
        InitiateVoxelNodes();
        SetBoundaryCenter();
        totalVoxel = numVoxelX*numVoxelY*numVoxelZ;
        totalVertexLayer = numVertexX*numVertexZ;
        totalVertex = numVertexY*totalVertexLayer;
        if((uint)totalVertex >= maxVertex)
        {
            Debug.LogError("Warning! Total vertex must not exceed "+(maxVertex-1)+"! Current number of vertex is = "+ (uint)totalVertex+". Number of vertex in y-dimension will be adjusted!");
        }
    }
}

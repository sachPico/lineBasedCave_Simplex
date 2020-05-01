using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel:MonoBehaviour
{
    static uint maxVertex = 13107 ; //sizeof(int16)/5. Five is max number of sub-mesh's vertex in marching cube combination table
    public float size;
    [Range(1, 20)]
    public int voxelsPerAxis;
    [Header("Unit voxel position")]
    public Vector3 voxelPosition;
    public Vector3Int voxelGrid;
    [Header("Voxel boundary properties")]
    public Vector3 boundaryCenter;
    public Vector3 boundaryWorldPos;
    //[Header("Voxels size")]
    //public Vector3Int numVoxel;
    public Vector3Int numVertex;
    [Space()]
    public Vector3Int numberOfGeneratedMeshObject = Vector3Int.one;
    public long totalVoxel;
    public int totalVertex;
     [Space()]
    public bool showVoxelGridGizmos;
    [HideInInspector]
    public Node[] voxelNodes = new Node[8];
    //[HideInInspector]
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

        boundaryWorldPos.x = size*voxelsPerAxis;
        boundaryWorldPos.y = size*voxelsPerAxis;
        boundaryWorldPos.z = size*voxelsPerAxis;
    }

    void OnDrawGizmos()
    {
        if(showVoxelGridGizmos)
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
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,0), new Vector3(i*size,0,voxelsPerAxis*size));
            }
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(voxelsPerAxis*size,0,i*size));
            }
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(i*size,0,voxelsPerAxis*size), new Vector3(i*size,voxelsPerAxis*size,voxelsPerAxis*size));
            }
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,voxelsPerAxis*size), new Vector3(voxelsPerAxis*size,i*size,voxelsPerAxis*size));
            }
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,0,i*size), new Vector3(0,voxelsPerAxis*size,i*size));
            }
            for(int i=0; i<voxelsPerAxis; i++)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(0,i*size,0), new Vector3(0,i*size,voxelsPerAxis*size));
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
        //dg.numDispatchThread.x = Mathf.CeilToInt((float)voxelsPerAxis/1);
        //dg.numDispatchThread.y = Mathf.CeilToInt((float)voxelsPerAxis/1);
        //dg.numDispatchThread.z = Mathf.CeilToInt((float)voxelsPerAxis/1);

        /*int index = voxelGrid.x + (voxelGrid.z * numVertex.x) + (voxelGrid.y * totalVertexLayer);
        dg.vertexDensity[0] = dg._pointsDensity[index];
        dg.vertexDensity[1] = dg._pointsDensity[index+1];
        dg.vertexDensity[2] = dg._pointsDensity[index+numVertex.x+1];
        dg.vertexDensity[3] = dg._pointsDensity[index+numVertex.x];
        dg.vertexDensity[4] = dg._pointsDensity[index+totalVertexLayer];
        dg.vertexDensity[5] = dg._pointsDensity[index+totalVertexLayer+1];
        dg.vertexDensity[6] = dg._pointsDensity[index+totalVertexLayer+numVertex.x+1];
        dg.vertexDensity[7] = dg._pointsDensity[index+totalVertexLayer+numVertex.x];*/

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineGenerator : MonoBehaviour
{
    public DensityGenerator densityGenerator;
    public Voxel voxelGrids;
    public int toIndex;
    public Vector3 newNodePosition;

    [Header("Active line properties")]
    public int activeLineIndex;
    public float activeRotation;

    [Header("Active node properties")]
    public int activeNodeIndex;
    public float minRadius;
    public float maxRadius;

    [Header("New node properties")]
    public float newMinRadius;
    public float newMaxRadius;

    [Header("Generated Line Properties")]
    [SerializeField, HideInInspector]
    public List<Line> line = new List<Line>();

    [SerializeField, HideInInspector]
    public List<Node> node = new List<Node>();

    [SerializeField]
    public bool showGizmos;

    public bool CheckNode(int i)
    {
        node[i].ClampPosZeroMax(new Vector3(
            voxelGrids.boundaryWorldPos.x*densityGenerator.numberOfGeneratedMeshObject.x,
            voxelGrids.boundaryWorldPos.y*densityGenerator.numberOfGeneratedMeshObject.y,
            voxelGrids.boundaryWorldPos.z*densityGenerator.numberOfGeneratedMeshObject.z));
        if(node[i]._nodeProp.position.x<voxelGrids.boundaryWorldPos.x&&node[i]._nodeProp.position.x>0)
        {
            if(node[i]._nodeProp.position.y<voxelGrids.boundaryWorldPos.y&&node[i]._nodeProp.position.y>0)
            {
                if(node[i]._nodeProp.position.z<voxelGrids.boundaryWorldPos.z&&node[i]._nodeProp.position.z>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void CreateLine()
    {
        this.node.Add(new Node(Vector3.zero, 1, 2));
        this.node.Add(new Node(Vector3.up, 1, 2));
        Line newLine = new Line(0,1);
        this.line.Add(newLine);
        Debug.Log(node.Count+" "+line.Count);
    }
    public void InitializeLine()
    {
        this.line.Clear();
        this.node.Clear();
        CreateLine();
        this.activeNodeIndex=0;
        this.activeLineIndex=0;
        this.activeRotation=0;
    }
    public void AddLine(int i, Vector3 newEndPoint)
    {
        this.node.Add(new Node(newEndPoint, newMinRadius, newMaxRadius));
        Line newLine = new Line(i, node.Count-1);
        this.line.Add(newLine);
        CalculateLocalZX(line.Count-1);
        CalculateLocalY(line.Count-1);
    }
    public void ConnectLine(int i, int existingNodeIndex)
    {
        Line newLine = new Line(i, existingNodeIndex);
        this.line.Add(newLine);
        CalculateLocalZX(line.Count-1);
        CalculateLocalY(line.Count-1);
    }
    public void CalculateLocalZX(int lineIndex)
    {
        //Calculate Z local axis (local forward)
        line[lineIndex].forwardLocal = (node[line[lineIndex]._lineProp.node2]-node[line[lineIndex]._lineProp.node1]).normalized;

        //Calculate X local axis (local right)
        if(line[lineIndex].forwardLocal.x != node[line[lineIndex]._lineProp.node1]._nodeProp.position.x && line[lineIndex].forwardLocal.z != node[line[lineIndex]._lineProp.node1]._nodeProp.position.z)
        {
            line[lineIndex].rightLocalOri.x = line[lineIndex].forwardLocal.z;
            line[lineIndex].rightLocalOri.z = -line[lineIndex].forwardLocal.x;
            line[lineIndex].rightLocalOri.y = 0;
            line[lineIndex].rightLocalOri = line[lineIndex].rightLocalOri.normalized;
        }
        else if(line[lineIndex].forwardLocal.x == node[line[lineIndex]._lineProp.node1]._nodeProp.position.x && line[lineIndex].forwardLocal.z == node[line[lineIndex]._lineProp.node1]._nodeProp.position.z)
        {
            line[lineIndex].rightLocalOri = Vector3.right*line[lineIndex].forwardLocal.y/Mathf.Abs(line[lineIndex].forwardLocal.y);
        }
        else if(line[lineIndex].forwardLocal.y == node[line[lineIndex]._lineProp.node1]._nodeProp.position.y && line[lineIndex].forwardLocal.z == node[line[lineIndex]._lineProp.node1]._nodeProp.position.z)
        {
            line[lineIndex].rightLocalOri = -Vector3.forward*line[lineIndex].forwardLocal.x/Mathf.Abs(line[lineIndex].forwardLocal.x);
        }
        else if(line[lineIndex].forwardLocal.y == node[line[lineIndex]._lineProp.node1]._nodeProp.position.y && line[lineIndex].forwardLocal.x == node[line[lineIndex]._lineProp.node1]._nodeProp.position.x)
        {
            line[lineIndex].rightLocalOri = Vector3.right*line[lineIndex].forwardLocal.x/Mathf.Abs(line[lineIndex].forwardLocal.x);
        }
    }

    public void CalculateRotateLocalX(int lineIndex)
    {
        line[lineIndex]._lineProp.rightLocal = Line.RotateAroundForward(line[lineIndex]);
    }

    public void CalculateLocalY(int lineIndex)
    {
        //Calculate Y local axis (local up)
        line[lineIndex]._lineProp.upLocal = Vector3.Cross(line[lineIndex].forwardLocal, line[lineIndex]._lineProp.rightLocal);
    }

    public NodeProperties[] GetNodes()
    {
        NodeProperties[] outputArray = new NodeProperties[node.Count];
        for(int i=0; i<outputArray.Length; i++)
        {
            outputArray[i] = node[i]._nodeProp;
        }
        return outputArray;
    }

    public int[] GetConnectedNodeIndex()
    {
        int[] outputArray = new int[line.Count*2];
        for(int i=0; i<outputArray.Length; i+=2)
        {
            outputArray[i] = line[i]._lineProp.node1;
            outputArray[i+1] = line[i]._lineProp.node2;
        }
        return outputArray;
    }

    public NodeProperties[] GetNodeProperties()
    {
        NodeProperties[] outputArray = new NodeProperties[node.Count];
        for(int i=0; i<outputArray.Length; i++)
        {
            outputArray[i] = node[i]._nodeProp;
        }
        return outputArray;
    }

    public LineProperties[] GetLineProps()
    {
        LineProperties[] outputArray = new LineProperties[line.Count];
        for(int i=0; i<outputArray.Length; i++)
        {
            outputArray[i] = line[i]._lineProp;
        }
        return outputArray;
    }

    public Vector3 GetCenter(int lineIndex)
    {
        return node[line[lineIndex]._lineProp.node2]-node[line[lineIndex]._lineProp.node1];
    }

    public Node GetActiveLineNode(bool isNode1)
    {
        if(isNode1)
        {
            return node[line[activeLineIndex]._lineProp.node1];
        }
        else
        {
            return node[line[activeLineIndex]._lineProp.node2];

        }
    }
    public void UpdateActiveLine()
    {
        CalculateLocalZX(activeLineIndex);
        CalculateRotateLocalX(activeLineIndex);
        CalculateLocalY(activeLineIndex);
    }
    public void OnValidate()
    {
        activeNodeIndex = Mathf.Clamp(activeNodeIndex,0,node.Count-1);
        activeLineIndex = Mathf.Clamp(activeLineIndex,0,line.Count-1);
        line[activeLineIndex].rotation = activeRotation;
        node[activeNodeIndex]._nodeProp.minRadius = minRadius;
        maxRadius = (maxRadius<minRadius) ? minRadius : maxRadius;
        node[activeNodeIndex]._nodeProp.maxRadius = maxRadius;
        UpdateActiveLine();
    }

    public void OnDrawGizmos()
    {
        if(showGizmos)
        {
            for(int i=0; i<node.Count; i++)
            {
                //Draw node's max and minRadius range
                Gizmos.color = new Color(1,0,0,0.25f);
                Gizmos.DrawSphere(node[i]._nodeProp.position, node[i]._nodeProp.maxRadius);
                Gizmos.color = new Color(0,1,0,0.25f);
                Gizmos.DrawSphere(node[i]._nodeProp.position, node[i]._nodeProp.minRadius);
            }
        }
    }
}

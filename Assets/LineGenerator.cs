using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineGenerator : MonoBehaviour
{
    const int numThread = 8;
    public DensityGenerator densityGenerator;
    public Voxel voxelGrids;
    public int toIndex;
    public Vector3 activeNodePosition;

    [Header("Active line properties")]
    public int activeLineIndex;
    public float activeRotation;
    public float activeLineLength;
    public float activeBeta;

    [Header("Active node properties")]
    public int activeNodeIndex;
    public float minRadius;
    public float maxRadius;

    [Header("New node properties")]
    public float newMinRadius;
    public float newMaxRadius;

    [Header("Generated Line Properties")]
    [SerializeField,HideInInspector]
    public List<Line> line = new List<Line>();

    [SerializeField, HideInInspector]
    public List<Node> node = new List<Node>();

    [SerializeField]
    public bool showGizmos;

    Vector3 oldNodePosition;

    public bool CheckNode(int i)
    {
        node[i].ClampPosZeroMax(new Vector3(
            voxelGrids.boundaryWorldPos.x*voxelGrids.numberOfGeneratedMeshObject.x,
            voxelGrids.boundaryWorldPos.y*voxelGrids.numberOfGeneratedMeshObject.y,
            voxelGrids.boundaryWorldPos.z*voxelGrids.numberOfGeneratedMeshObject.z));
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
        UpdateAllLines();
    }
    public void ConnectLine(int i, int existingNodeIndex)
    {
        Line newLine = new Line(i, existingNodeIndex);
        this.line.Add(newLine);
        CalculateLocalZX(line.Count-1);
        CalculateLocalY(line.Count-1);
        UpdateAllLines();
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
    public void UpdateAllLines()
    {
        for(int i=0; i<line.Count; i++)
        {
            CalculateLocalZX(i);
            CalculateRotateLocalX(i);
            CalculateLocalY(i);
            UpdateNearestNodeToCylinderCenter(i);
            line[i]._lineProp.lineLength = Vector3.Magnitude(node[line[i]._lineProp.node2]._nodeProp.position-node[line[i]._lineProp.node1]._nodeProp.position);
        }
        DensityGenerator dg = this.gameObject.GetComponent<DensityGenerator>();//.GenerateDensity();
        if(dg.autoUpdate)
        {
            dg.GenerateDensity();
        }
    }

    public void UpdateNearestNodeToCylinderCenter(int lineIndex)
    {
        float beta, deltaR;        
        NodeProperties nodeA = node[line[lineIndex]._lineProp.node1]._nodeProp;
        NodeProperties nodeB = node[line[lineIndex]._lineProp.node2]._nodeProp;
        float lineLength = Vector3.Magnitude(nodeB.position-nodeA.position);
        activeLineLength = lineLength;
        float smallestNodeRadius, largestNodeRadius;
        if(nodeA.maxRadius > nodeB.maxRadius)
        {
            smallestNodeRadius = nodeB.maxRadius;
            largestNodeRadius = nodeA.maxRadius;
        }
        else if(nodeA.maxRadius == nodeB.maxRadius)
        {
            beta = 0;
            activeBeta = beta;

            line[lineIndex]._lineProp.AC = 0;
            line[lineIndex]._lineProp.BD = 0;
            line[lineIndex]._lineProp.CE = nodeA.maxRadius;
            line[lineIndex]._lineProp.DF = nodeA.maxRadius;

            return;
        }
        else
        {
            smallestNodeRadius = nodeA.maxRadius;
            largestNodeRadius = nodeB.maxRadius;
        }
        
        deltaR = largestNodeRadius-smallestNodeRadius;
        beta = Mathf.Asin((deltaR)/lineLength);
        activeBeta = beta*Mathf.Rad2Deg;
        
        line[lineIndex]._lineProp.AC = smallestNodeRadius * deltaR/lineLength;
        line[lineIndex]._lineProp.BD = largestNodeRadius * deltaR/lineLength;
        line[lineIndex]._lineProp.CE = Mathf.Sqrt(Mathf.Pow(smallestNodeRadius,2)-Mathf.Pow(line[lineIndex]._lineProp.AC,2));
        line[lineIndex]._lineProp.DF = Mathf.Sqrt(Mathf.Pow(largestNodeRadius,2)-Mathf.Pow(line[lineIndex]._lineProp.BD,2));
    }

    public void OnValidate()
    {
        activeNodeIndex = Mathf.Clamp(activeNodeIndex,0,node.Count-1);
        activeLineIndex = Mathf.Clamp(activeLineIndex,0,line.Count-1);

        line[activeLineIndex].rotation = activeRotation;

        node[activeNodeIndex]._nodeProp.minRadius = minRadius;
        maxRadius = (maxRadius<minRadius) ? minRadius : maxRadius;
        node[activeNodeIndex]._nodeProp.maxRadius = maxRadius;
        if(node[activeNodeIndex]._nodeProp.position.x!=activeNodePosition.x||node[activeNodeIndex]._nodeProp.position.y!=activeNodePosition.y||node[activeNodeIndex]._nodeProp.position.z!=activeNodePosition.z)
        {
            node[activeNodeIndex]._nodeProp.position = activeNodePosition;
            //activeNodePosition = node[activeNodeIndex]._nodeProp.position;
        }
        //node[activeNodeIndex]._nodeProp.position = activeNodePosition;
        
        UpdateAllLines();
    }

    public void OnDrawGizmos()
    {
        if(showGizmos)
        {
            for(int i=0; i<node.Count; i++)
            {
                //Draw node's max and minRadius range
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(node[i]._nodeProp.position, node[i]._nodeProp.maxRadius);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(node[i]._nodeProp.position, node[i]._nodeProp.minRadius);
            }

            NodeProperties smallestRadiusNode, largestRadiusNode;
            Vector3 blueVectorAC, blueVectorBD;

            for(int i=0; i<line.Count; i++)
            {
                if(node[line[i]._lineProp.node1]._nodeProp.maxRadius>node[line[i]._lineProp.node2]._nodeProp.maxRadius)
                {
                    //A as the largest radius node
                    smallestRadiusNode = node[line[i]._lineProp.node2]._nodeProp;
                    largestRadiusNode = node[line[i]._lineProp.node1]._nodeProp;
                }
                else
                {
                    //B as the largest radius node
                    smallestRadiusNode = node[line[i]._lineProp.node1]._nodeProp;
                    largestRadiusNode = node[line[i]._lineProp.node2]._nodeProp;
                }

                blueVectorAC = Vector3.Normalize(largestRadiusNode.position-smallestRadiusNode.position)*-line[i]._lineProp.AC;
                blueVectorBD = Vector3.Normalize(largestRadiusNode.position-smallestRadiusNode.position)*-line[i]._lineProp.BD;

                if(i==activeLineIndex)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.black;
                }  
                Gizmos.DrawLine(
                    smallestRadiusNode.position, 
                    blueVectorAC+smallestRadiusNode.position);
                Gizmos.DrawLine(
                    largestRadiusNode.position, 
                    blueVectorBD+largestRadiusNode.position);

                if(i==activeLineIndex)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.black;
                } 

                Gizmos.DrawLine(
                    smallestRadiusNode.position+blueVectorAC - (line[i]._lineProp.upLocal*line[i]._lineProp.CE),
                    (line[i]._lineProp.upLocal*line[i]._lineProp.CE)+smallestRadiusNode.position+blueVectorAC);
                Gizmos.DrawLine(
                    largestRadiusNode.position+blueVectorBD - (line[i]._lineProp.upLocal*line[i]._lineProp.DF), 
                    line[i]._lineProp.upLocal*line[i]._lineProp.DF+largestRadiusNode.position+blueVectorBD);
                Gizmos.DrawLine(
                    smallestRadiusNode.position+blueVectorAC - (line[i]._lineProp.rightLocal*line[i]._lineProp.CE),
                    (line[i]._lineProp.rightLocal*line[i]._lineProp.CE)+smallestRadiusNode.position+blueVectorAC);
                Gizmos.DrawLine(
                    largestRadiusNode.position+blueVectorBD - (line[i]._lineProp.rightLocal*line[i]._lineProp.DF), 
                    line[i]._lineProp.rightLocal*line[i]._lineProp.DF+largestRadiusNode.position+blueVectorBD);

                if(i==activeLineIndex)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.black;
                } 
                Gizmos.DrawLine(
                    (line[i]._lineProp.upLocal*line[i]._lineProp.CE)+smallestRadiusNode.position+blueVectorAC,
                    (line[i]._lineProp.upLocal*line[i]._lineProp.DF)+largestRadiusNode.position+blueVectorBD);
                Gizmos.DrawLine(
                    smallestRadiusNode.position+blueVectorAC - (line[i]._lineProp.upLocal*line[i]._lineProp.CE),
                    largestRadiusNode.position+blueVectorBD - (line[i]._lineProp.upLocal*line[i]._lineProp.DF));
                Gizmos.DrawLine(
                    (line[i]._lineProp.rightLocal*line[i]._lineProp.CE)+smallestRadiusNode.position+blueVectorAC,
                    (line[i]._lineProp.rightLocal*line[i]._lineProp.DF)+largestRadiusNode.position+blueVectorBD);
                Gizmos.DrawLine(
                    smallestRadiusNode.position+blueVectorAC - (line[i]._lineProp.rightLocal*line[i]._lineProp.CE),
                    largestRadiusNode.position+blueVectorBD - (line[i]._lineProp.rightLocal*line[i]._lineProp.DF));
            }
        }
    }
}

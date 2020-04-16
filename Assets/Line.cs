using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LineProperties
{
    public int node1, node2;
    public Vector3 rightLocal, upLocal;
}

[System.Serializable]
public class Line
{
    [SerializeField]
    //public Node startNode = new Node(Vector3.zero), endNode = new Node(Vector3.zero);
    public LineProperties _lineProp;
    [SerializeField,HideInInspector]
    public Vector3 forwardLocal, rightLocalOri;
    [SerializeField, HideInInspector]
    public QVector qV1 = new QVector(1,0,0,0), qV2 = new QVector(1,0,0,0);
    [SerializeField]
    public float rotation;

    /*public Line(Node start, Node end)
    {
        startNode = start;
        endNode = end;

        Node targetNode = (endNode-startNode);
        forwardLocal = targetNode.position.normalized;

        if(forwardLocal.x != startNode.position.x && forwardLocal.z != startNode.position.z)
        {
            rightLocalOri.x = forwardLocal.z;
            rightLocalOri.z = -forwardLocal.x;
            rightLocalOri.y = 0;
            rightLocalOri = rightLocalOri.normalized;
        }
        else if(forwardLocal.x == startNode.position.x && forwardLocal.z == startNode.position.z)
        {
            rightLocalOri = Vector3.right*targetNode.position.y/Mathf.Abs(targetNode.position.y);
        }
        else if(forwardLocal.y == startNode.position.y && forwardLocal.z == startNode.position.z)
        {
            rightLocalOri = -Vector3.forward*targetNode.position.x/Mathf.Abs(targetNode.position.x);
        }
        else if(forwardLocal.y == startNode.y && forwardLocal.x == startNode.x)
        {
            rightLocalOri = Vector3.right*targetNode.x/Mathf.Abs(targetNode.x);
        }

        rightLocal = rightLocalOri;
        
        upLocal = Vector3.Cross(forwardLocal, rightLocal).normalized;
    }*/
    public Line(int _n1, int _n2)
    {
        _lineProp.node1 = _n1;
        _lineProp.node2 = _n2;
    }

    public static Vector3 RotateAroundForward(Line _line)
    {
        _line.qV1.w = Mathf.Cos(Mathf.Deg2Rad*_line.rotation/2);
        _line.qV1.x = Mathf.Sin(Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.x;
        _line.qV1.y = Mathf.Sin(Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.y;
        _line.qV1.z = Mathf.Sin(Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.z;

        _line.qV2.w = Mathf.Cos(-Mathf.Deg2Rad*_line.rotation/2);
        _line.qV2.x = Mathf.Sin(-Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.x;
        _line.qV2.y = Mathf.Sin(-Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.y;
        _line.qV2.z = Mathf.Sin(-Mathf.Deg2Rad*_line.rotation/2)*_line.forwardLocal.z;

        return QVector.GetImaginaryPart(_line.qV1*QVector.SetVector3ToQuaternion(_line.rightLocalOri)*_line.qV2);
    }
}

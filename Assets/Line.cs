using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LineProperties
{
    //Index of two connected nodes
    public int node1, node2;
    //Unit vector of x-local and y-local axis. Z-local is the unit vector of the line itself
    public Vector3 rightLocal, upLocal;
    //AC and BD are the nearest distance between each node of a line and the center of one of two circles of a cylinder
    //The calculations for these two variables are done in Unity to reduce the calculations done in compute shader
    public float AC, BD, CE, DF, lineLength;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NodeProperties
{
    public Vector3 position;
    public float minRadius, maxRadius;
}

[System.Serializable]
public class Node
{
    [SerializeField]
    public NodeProperties _nodeProp;

    public Node(Vector3 _pos, float min, float max)
    {
        _nodeProp.position = _pos;
        _nodeProp.minRadius = min;
        _nodeProp.maxRadius = max;
    }

    public void ClampPosZeroMax(Vector3 max)
    {
        _nodeProp.position.x = Mathf.Clamp(_nodeProp.position.x, 0, max.x);
        _nodeProp.position.y = Mathf.Clamp(_nodeProp.position.y, 0, max.y);
        _nodeProp.position.z = Mathf.Clamp(_nodeProp.position.z, 0, max.z);
    }
    public void SetPos(Vector3 _pos)
    {
        _nodeProp.position = _pos;
    }

    public static Vector3 operator + (Node n1, Node n2)
    {
        return n1._nodeProp.position+n2._nodeProp.position;
    }

    public static Vector3 operator + (Node n, Vector3 v)
    {
        return n._nodeProp.position+v;
    }

    public static Vector3 operator - (Node n1, Node n2)
    {
        return n1._nodeProp.position-n2._nodeProp.position;
    }

    public static Vector3 operator / (Node n1, int divider)
    {
        return n1._nodeProp.position/divider;
    }

    public static Vector3 operator * (Node n1, int multiplier)
    {
        return n1._nodeProp.position*multiplier;
    }
}

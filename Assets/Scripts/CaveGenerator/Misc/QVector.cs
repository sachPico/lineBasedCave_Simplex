using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QVector
{
    [SerializeField,HideInInspector]
    public float w,x,y,z;

    public QVector(float _w, float _x, float _y, float _z)
    {
        w = _w;
        x = _x;
        y = _y;
        z = _z;
    }

    public static QVector operator *(QVector q1, QVector q2)
    {
        return new QVector(
            (q1.w*q2.w)-(q1.x*q2.x)-(q1.y*q2.y)-(q1.z*q2.z),
            (q1.w*q2.x)+(q1.x*q2.w)+(q1.y*q2.z)-(q1.z*q2.y),
            (q1.w*q2.y)-(q1.x*q2.z)+(q1.y*q2.w)+(q1.z*q2.x),
            (q1.w*q2.z)+(q1.x*q2.y)-(q1.y*q2.x)+(q1.z*q2.w)
        );
    }

    public static Vector3 GetImaginaryPart(QVector qV)
    {
        return new Vector3(qV.x,qV.y,qV.z);
    }

    public static QVector SetVector3ToQuaternion(Vector3 input)
    {
        return new QVector(0, input.x, input.y, input.z);
    }

}

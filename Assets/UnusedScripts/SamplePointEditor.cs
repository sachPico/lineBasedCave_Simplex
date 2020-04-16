using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SamplePoints))]
public class SamplePointEditor : Editor
{
    SamplePoints _samplePoint;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Calculate Sample Points Within Sample Lines"))
        {
            //_samplePoint.Draw();
        }
    }

    public void OnEnable()
    {
        _samplePoint = target as SamplePoints;
    }
}

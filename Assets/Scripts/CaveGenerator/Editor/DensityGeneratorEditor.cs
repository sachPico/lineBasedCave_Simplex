using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Script ini digunakan untuk mengubah tampilan GUI pada component "DensityGenerator" pada panel Inspector
//This script is used to change the GUI of "DensityGenerator" component on Inspector panel
[CustomEditor(typeof(DensityGenerator))]
public class DensityGeneratorEditor : Editor
{
    DensityGenerator CSTarget;
    public override void OnInspectorGUI()
    {
        //Tampilkan GUI component seperti biasa
        //Display component's GUI like usual
        DrawDefaultInspector();

        //Perintah "if" ini akan menambahkan tombol "Generate Mesh" secara otomatis pada panel Inspector
        //This "if" command will add "Generate Mesh" button automatically on Inspector panel
        if(GUILayout.Button("Generate Density and Mesh"))
        {
            CSTarget.GenerateDensity();
        }
        
        /*if(GUILayout.Button("Generate Mesh"))
        {
            CSTarget.GenerateMesh(0);
        }*/

        if(GUILayout.Button("Clear buffers"))
        {
            CSTarget.ReleaseBuffers();
        }
        if(GUILayout.Button("Show Kernel Thread Group Size"))
        {
            uint x, y, z;
            CSTarget._densityShader.GetKernelThreadGroupSizes(0, out x, out y, out z);
            Debug.Log("Kernel thread group size: "+x+", "+y+", "+z+".");
        }
    }
    public void OnEnable()
    {
        CSTarget = (DensityGenerator)target;
    }
}

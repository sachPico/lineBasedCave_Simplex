using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineGenerator))]
public class LineEditor : Editor
{
    public LineGenerator l;
    public List<Line> lines;

    public void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0&& guiEvent.control)
        {
            Undo.RecordObject(l, "Add new line with new node");
            l.AddLine(l.activeNodeIndex, mousePos);
            l.node[l.node.Count-1].ClampPosZeroMax(
            new Vector3(
            l.voxelGrids.boundaryWorldPos.x*l.densityGenerator.numberOfGeneratedMeshObject.x,
            l.voxelGrids.boundaryWorldPos.y*l.densityGenerator.numberOfGeneratedMeshObject.y,
            l.voxelGrids.boundaryWorldPos.z*l.densityGenerator.numberOfGeneratedMeshObject.z));
        }
    }

    public void Draw()
    {
        Event guiEvent = Event.current;
        //Handling every line
        for(int i=0; i<l.line.Count; i++)
        {
            Handles.color = Color.black;
            Handles.DrawLine(l.node[l.line[i]._lineProp.node1]._nodeProp.position, l.node[l.line[i]._lineProp.node2]._nodeProp.position);
            //If it's a selected line, mark its center point with red, otherwise mark it blue
            if(i==l.activeLineIndex)
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = Color.blue;
            }
            
            //The line's center point
            Vector3 lineVec = (l.node[l.line[i]._lineProp.node2]._nodeProp.position-l.node[l.line[i]._lineProp.node1]._nodeProp.position)/2;
            Vector3 linePos = Handles.FreeMoveHandle
            (
                i+300,
                l.node[l.line[i]._lineProp.node1]._nodeProp.position+lineVec,
                Quaternion.identity,
                .2f,
                Vector3.zero,
                Handles.SphereHandleCap
            );
            //Debug.Log(l.line[i]._lineProp.node2+", "+l.line[i]._lineProp.node1);
            //Check if it's selected
            if(guiEvent.button==0&&GUIUtility.hotControl==i+300)
            {
                l.activeLineIndex = i;
                l.activeRotation = l.line[i].rotation;
            }
            //Check if it's been moved
            if((l.node[l.line[i]._lineProp.node1]+((l.node[l.line[i]._lineProp.node2]-l.node[l.line[i]._lineProp.node1])/2)) != linePos)
            {
                if(l.CheckNode(l.line[i]._lineProp.node1)&&l.CheckNode(l.line[i]._lineProp.node2))
                {
                    Undo.RecordObject(l, "Moved line position");
                    l.node[l.line[i]._lineProp.node1]._nodeProp.position = linePos - lineVec;
                    l.node[l.line[i]._lineProp.node2]._nodeProp.position = linePos + lineVec;
                    l.UpdateActiveLine();
                }
            }
        }

        //Handling every node
        for(int i=0; i<l.node.Count; i++)
        {
            //If it's a selected node
            if(i==l.activeNodeIndex)
            {
                Handles.color = new Color(1,1,1,0.5f);
            }
            else
            {
                Handles.color = new Color(0,0,0,0.5f);
            }
            Vector3 nodePos = Handles.FreeMoveHandle(i+200,l.node[i]._nodeProp.position, Quaternion.identity, l.node[i]._nodeProp.minRadius, Vector3.zero, Handles.SphereHandleCap);
            //Check if it's selected
            if(guiEvent.button==0 && GUIUtility.hotControl==i+200)
            {
                l.activeNodeIndex = i;
                l.minRadius = l.node[l.activeNodeIndex]._nodeProp.minRadius;
                l.maxRadius = l.node[l.activeNodeIndex]._nodeProp.maxRadius;
            }
            

            //Check if it's selected and shift button is pressed to connect line between two nodes
            

            //Check if it's been moved
            if(l.node[i]._nodeProp.position != nodePos)
            {
                Undo.RecordObject(l, "Moved node position");
                l.node[i]._nodeProp.position = nodePos;
                l.node[i].ClampPosZeroMax(new Vector3(
            l.voxelGrids.boundaryWorldPos.x*l.densityGenerator.numberOfGeneratedMeshObject.x,
            l.voxelGrids.boundaryWorldPos.y*l.densityGenerator.numberOfGeneratedMeshObject.y,
            l.voxelGrids.boundaryWorldPos.z*l.densityGenerator.numberOfGeneratedMeshObject.z));
                l.UpdateActiveLine();
            }
        }
        Handles.color = Color.red;
        Handles.DrawLine(
            (l.GetActiveLineNode(true) + (l.GetCenter(l.activeLineIndex)/2)),
            (l.GetActiveLineNode(true) + (l.GetCenter(l.activeLineIndex)/2))+l.line[l.activeLineIndex]._lineProp.rightLocal
            );
        Handles.color = Color.green;
        Handles.DrawLine(
            (l.GetActiveLineNode(true) + (l.GetCenter(l.activeLineIndex)/2)),
            (l.GetActiveLineNode(true) + (l.GetCenter(l.activeLineIndex)/2))+l.line[l.activeLineIndex]._lineProp.upLocal
            );
    }

    public override void OnInspectorGUI()
    {
        l = (LineGenerator)target;
        //EditorGUILayout.PropertyField(linesProperty);
        if(l.line == null)
        {
            l.CreateLine();
        }
        //editorLine = l.line;
        DrawDefaultInspector();
        /*if(GUILayout.Button("Add new line with new node"))
        {
            Undo.RecordObject(l, "Add line");
            l.AddLine(l.activeNodeIndex, l.newNodePosition);
        }*/
        if(GUILayout.Button("Add new line with existing node"))
        {
            Undo.RecordObject(l, "Connect line");
            l.ConnectLine(l.activeNodeIndex, l.toIndex);
        }
        if(GUILayout.Button("Initialize points and line"))
        {
            Undo.RecordObject(l, "Initialize lines");
            l.InitializeLine();
        }
    }
    
    public void OnSceneGUI()
    {
        Input();
        Draw();
    }
    public void OnEnable()
    {
        l = (LineGenerator)target;
        if(l.line == null)
        {
            l.CreateLine();
        }
        lines = l.line;
    }
}

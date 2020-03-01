using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;

    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    private void OnEnable()
    {
        creator = (PathCreator)target;
        if(creator.path == null)
        {
            creator.CreatePath();
        }
        path = creator.path;
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 lastPoint = path.GetPointsInSegment(path.NumSegments - 1)[3];
        Vector3 newPos = lastPoint + (Vector3.up * 0.1f);
        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, "Add Segment");
            path.AddSegment(newPos);
        }
    }

    void Draw()
    {

        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }

        for(int i = 0; i < path.NumPoints; i++)
        {
            if(i % 3 == 0)
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = Color.cyan;
            }
            Vector3 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.1f, Vector3.zero, Handles.CubeHandleCap);
            if(path[i] != newPos)
            {
                Undo.RecordObject(creator, "Move Point");
                path.MovePoint(i, newPos);
            }
        }
        return;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField]
    public List<Vector3> points;

    public List<Vector3> anchorPoints;

    public Path(Vector3 center)
    {
        points = new List<Vector3>
        {
            center + Vector3.left,
            center + (Vector3.left + Vector3.up) * 0.5f,
            center + (Vector3.right + Vector3.down) * 0.5f,
            center + Vector3.right
        };
        anchorPoints = new List<Vector3>
        {
            points[0],
            points[3]
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints
    {
        get
        {
            if (points != null)
            {
                return points.Count;
            }
            else return 0;
        }
    }

    public int NumSegments
    {
        get
        {
            return (points.Count - 4) / 3 + 1;
        }
    }

    public void AddSegment (Vector3 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);
        anchorPoints.Add(anchorPos);
    }

    public Vector3[] GetPointsInSegment(int index)
    {
        return new Vector3[] { points[index * 3], points[index * 3 + 1], points[index * 3 + 2], points[index * 3 + 3] };
    }

    public void MovePoint(int i, Vector3 newPos)
    {
        Vector3 deltaMove = newPos - points[i];
        points[i] = newPos;

        if (i % 3 == 0)
        {
            if (i + 1 < points.Count)
            {
                points[i + 1] += deltaMove;
            }
            if (i - 1 >= 0)
            {
                points[i - 1] += deltaMove;
            }
            anchorPoints[i/3] = newPos;
        }
        else
        {
            bool nextPointIsAnchor = (i + 1) % 3 == 0;
            int controlIndex = nextPointIsAnchor ? i + 2 : i - 2;
            int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

            if (controlIndex >= 0 && controlIndex < points.Count)
            {
                float distance = (points[anchorIndex] - points[controlIndex]).magnitude;
                Vector3 direction = (points[anchorIndex] - newPos).normalized;

                points[controlIndex] = points[anchorIndex] + direction * distance;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToSpline : MonoBehaviour
{
    public GameObject snappedTo;
    private Path path;

    int currentPoint;
    float distance = 0f;

    private void Start()
    {
        path = GetComponent<PathCreator>().path;
        snappedTo.transform.position = path.anchorPoints[0];
        currentPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPoint + 1 < path.anchorPoints.Count)
        {
            if (snappedTo.transform.position == path.anchorPoints[currentPoint + 1])
            {
                distance = 0f;
                currentPoint++;
            }
            else
            {
                Vector3[] pointsInSegment = path.GetPointsInSegment(currentPoint);
                snappedTo.transform.position = CubicCurve(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], distance += 0.05f);
            }
        }
    }

    Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = QuadraticCurve(a, b, c, t);
        Vector3 p1 = QuadraticCurve(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
    }

}

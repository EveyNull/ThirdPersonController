using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float platformMoveSpeed = 0.01f;

    private Path path;

    Coroutine movePlatformCoroutine;
    MoveScript player;

    int currentPoint;
    float distance = 0f;
    Vector3 offset;

    private void Start()
    {
        path = GetComponent<PathCreator>().path;
        currentPoint = 0;
    }

    private void LateUpdate()
    {

        if(player != null)
        {
            player.transform.position = transform.position + offset;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (player != null)
        {
            if (collision.collider.GetComponent<MoveScript>() == player)
            {
                offset = player.transform.position - transform.position;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<MoveScript>() && movePlatformCoroutine == null)
        {
            movePlatformCoroutine = StartCoroutine(MovePlatformToEndOfSpline());
            player = collision.collider.GetComponent<MoveScript>();
            offset = player.transform.position - transform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<MoveScript>() == player)
        {
            player = null;
        }
    }

    IEnumerator MovePlatformToEndOfSpline()
    {
        while (currentPoint + 1 < path.anchorPoints.Count)
        {
            if (transform.position == path.anchorPoints[currentPoint + 1])
            {
                distance = 0f;
                currentPoint++;
            }
            else
            {
                Vector3[] pointsInSegment = path.GetPointsInSegment(currentPoint);
                Vector3 test = CubicCurve(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], distance);
                transform.position = test;
            }
            distance += platformMoveSpeed;
            yield return 0;
        }
        movePlatformCoroutine = null;
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

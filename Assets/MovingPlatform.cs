using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float platformMoveSpeed = 0.01f;

    private Path path;

    Coroutine movePlatformCoroutine;
    MoveScript player;

    List<Vector3> avgVelocity;

    bool addVelocity = true;

    int currentPoint;
    float distance = 0f;
    Vector3 offset;

    private void Start()
    {
        path = GetComponent<PathCreator>().path;
        path.points[0] = transform.position;
        currentPoint = 0;
        avgVelocity = new List<Vector3>();
    }

    private void FixedUpdate()
    {
        if (addVelocity)
        {
            avgVelocity.Add(GetComponent<Rigidbody>().velocity);
            if (avgVelocity.Count > 10)
            {
                avgVelocity.RemoveAt(0);
            }
            Vector3 avg = new Vector3();
            if (avgVelocity.Count > 0)
            {
                foreach (Vector3 velocity in avgVelocity)
                {
                    avg += velocity;
                }
                avg /= avgVelocity.Count;
            }
            if (player)
            {
                avg.y = 0f;
                player.GetComponent<Rigidbody>().velocity += avg;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<MoveScript>() && movePlatformCoroutine == null)
        {
            movePlatformCoroutine = StartCoroutine(MovePlatformToEndOfSpline());
            player = collision.collider.GetComponent<MoveScript>();
        }
        if(movePlatformCoroutine != null)
        {
            player.allowJump = false;
        }
        addVelocity = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<MoveScript>() == player)
        {
            player.allowJump = true;
            player = null;
            addVelocity = false;
        }
    }
    IEnumerator MovePlatformToEndOfSpline()
    {
        while (currentPoint + 1 < path.anchorPoints.Count)
        {
            Vector3[] pointsInSegment = path.GetPointsInSegment(currentPoint);
            Vector3 nextPoint = GetComponent<Rigidbody>().position;
            float f = distance;
            for(; f <= 1f; f += 0.001f)
            {
                nextPoint = CubicCurve(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], f);
                if (Vector3.Distance(GetComponent<Rigidbody>().position, nextPoint) >= platformMoveSpeed * Time.deltaTime)
                {
                    break;
                }
            }
            if(f >= 1)
            {
                distance = 0f;
                currentPoint++;
            }
            else
            {
                distance = f;
            }
            GetComponent<Rigidbody>().MovePosition(nextPoint);
            yield return 0;
        }
        movePlatformCoroutine = null;
        if (player)
        {
            player.allowJump = true;
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

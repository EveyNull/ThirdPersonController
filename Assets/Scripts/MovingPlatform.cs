using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float platformMoveSpeed = 0.01f;

    private Path path;

    Rigidbody platformRB;

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
            if (player)
            {
                Vector3 velocity = GetComponent<Rigidbody>().velocity;
                velocity.y = 0f;
                player.GetComponent<Rigidbody>().velocity += velocity;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (movePlatformCoroutine == null)
        {
            movePlatformCoroutine = StartCoroutine(MovePlatformToEndOfSpline());
        }
        if(collision.collider.GetComponent<MoveScript>())
        {
            player = collision.collider.GetComponent<MoveScript>();
            addVelocity = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<MoveScript>() == player)
        {
            Vector3 playerVel = player.GetComponent<Rigidbody>().velocity;
            playerVel.x = GetComponent<Rigidbody>().velocity.x;
            playerVel.z = GetComponent<Rigidbody>().velocity.z;

            player.GetComponent<Rigidbody>().velocity = playerVel;

            player = null;
            addVelocity = false;
        }
    }

    IEnumerator MovePlatformToEndOfSpline()
    {
        distance = 0.1f;
        while (currentPoint + 1 < path.anchorPoints.Count)
        {
            Vector3[] pointsInSegment = path.GetPointsInSegment(currentPoint);
            Vector3 nextPoint = CubicCurve(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], distance);

            Vector3 moveTo = GetComponent<Rigidbody>().position + (nextPoint - GetComponent<Rigidbody>().position).normalized * Time.fixedDeltaTime * platformMoveSpeed;
            
            GetComponent<Rigidbody>().MovePosition(moveTo);

            if(Vector3.Distance(GetComponent<Rigidbody>().position, nextPoint) <= 0.1f)
            {
                distance += 0.05f;
            }

            if (distance >= 1f)
            {
                currentPoint++;
                distance = 0f;
            }

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

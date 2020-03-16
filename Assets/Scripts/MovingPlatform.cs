using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float platformMoveSpeed = 0.01f;

    protected Path path;
    
    MoveScript player;

    bool addVelocity = true;

    [SerializeField]
    protected int currentPoint;
    [SerializeField]
    protected float distance = 0f;

    protected virtual void Start()
    {
        path = GetComponent<PathCreator>().path;
        path.points[0] = transform.position;
        currentPoint = 0;
    }

    protected virtual void FixedUpdate()
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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<MoveScript>())
        {
            player = collision.collider.GetComponent<MoveScript>();
            addVelocity = true;
        }
    }

    protected virtual void OnTriggerExit(Collider collision)
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

    protected Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    protected Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = QuadraticCurve(a, b, c, t);
        Vector3 p1 = QuadraticCurve(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
    }

}

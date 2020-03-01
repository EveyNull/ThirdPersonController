using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPlayerToPath : MonoBehaviour
{
    public bool cameraOnRight;
    public float moveSpeed;

    private Path path;
    MoveScript player;

    float playerProgress;

    private float horizontal;
    private Vector3 cameraOffSetInitial;

    bool cameraLookOnPlayerRight = true;

    bool startAtStart;
    bool seenPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        path = GetComponent<PathCreator>().path;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            CheckForPlayer();
        }
        else if(player != null)
        {
            MovePlayer();
        }
    }

    void CheckForPlayer()
    {
        bool foundPlayer = false;
        foreach (Collider collider in Physics.OverlapSphere(path.GetPointsInSegment(0)[0], 2f))
        {
            if (collider.GetComponent<MoveScript>() && !seenPlayer)
            {
                foundPlayer = true;
                if (!seenPlayer)
                {
                    startAtStart = true;
                    player = collider.GetComponent<MoveScript>();
                    player.enabled = false;
                    player.animator.applyRootMotion = false;
                    playerProgress = 0f;
                    Camera.main.GetComponent<AxisFollowCam>().enabled = false;
                    seenPlayer = true;
                    cameraOffSetInitial = Camera.main.transform.localPosition;

                    Vector3 playerZeroY = player.transform.position;
                    playerZeroY.y = 0f;

                    Vector3 lookAtZeroY = path.GetPointsInSegment(0)[0];
                    lookAtZeroY.y = 0f;

                    player.transform.rotation = Quaternion.LookRotation(lookAtZeroY - playerZeroY);
                }   
            }
        }
    

        foreach (Collider collider in Physics.OverlapSphere(path.GetPointsInSegment(path.NumSegments - 1)[3], 2f))
        {
            if (collider.GetComponent<MoveScript>())
            {
                foundPlayer = true;
                if (!seenPlayer)
                {
                    startAtStart = false;
                    player = collider.GetComponent<MoveScript>();
                    player.enabled = false;
                    player.animator.applyRootMotion = false;
                    playerProgress = path.NumSegments;
                    Camera.main.GetComponentInParent<AxisFollowCam>().enabled = false;
                    seenPlayer = true;
                    cameraOffSetInitial = Camera.main.transform.localPosition;

                    Vector3 playerZeroY = player.transform.position;
                    playerZeroY.y = 0f;

                    Vector3 lookAtZeroY = path.GetPointsInSegment(path.NumSegments - 1)[3];
                    lookAtZeroY.y = 0f;

                    player.transform.rotation = Quaternion.LookRotation(lookAtZeroY - playerZeroY);
                }
            }
        }
        if(!foundPlayer)
        {
            seenPlayer = false;
        }
    }

    void MovePlayer()
    {
        Vector3 startPos = player.transform.position;
        horizontal = Input.GetAxis("Horizontal");

        if ((playerProgress >= path.NumSegments && horizontal < 0)
            || (playerProgress <= 0 && horizontal > 0))
        {
            player.enabled = true;
            player.animator.applyRootMotion = true;
            player = null;
            Camera.main.GetComponentInParent<AxisFollowCam>().enabled = true;
            Camera.main.transform.localPosition = cameraOffSetInitial;
            return;
        }
        player.animator.SetFloat("z", Mathf.Abs(horizontal) * player.moveSpeedPercent);
        int playerSegment = Mathf.CeilToInt(Mathf.Clamp(playerProgress, 0, path.NumSegments));
        playerProgress -= horizontal * player.moveSpeedPercent * 0.01f;
        Vector3[] pointsInSegment = path.GetPointsInSegment(playerSegment-1);
        
        Vector3 pos = CubicCurve
            (
                pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], playerProgress - (playerSegment - 1)
            );

        RaycastHit hit;
        Physics.Raycast(pos + Vector3.up * 0.1f, Vector3.down, out hit);

        Vector3 playerZeroY = player.transform.position;
        playerZeroY.y = 0f;

        Vector3 lookAtZeroY = hit.point;
        lookAtZeroY.y = 0f;

        if (playerZeroY - lookAtZeroY != Vector3.zero)
        {
            player.transform.rotation = Quaternion.LookRotation(lookAtZeroY - playerZeroY);
        }
        player.transform.position = hit.point;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (horizontal > 0)
            {
                cameraLookOnPlayerRight = true;
            }
            else if (horizontal < 0)
            {
                cameraLookOnPlayerRight = false;
            }

            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Camera.main.GetComponentInParent<AxisFollowCam>().lookAt.position + (cameraLookOnPlayerRight ? player.transform.right * 5f : -player.transform.right * 5f), 0.1f);
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.LookRotation(Camera.main.GetComponentInParent<AxisFollowCam>().lookAt.position - Camera.main.transform.position), 2f);
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

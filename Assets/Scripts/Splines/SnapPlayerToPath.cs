using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPlayerToPath : MonoBehaviour
{
    public float moveSpeed;
    public PathCreator camPath;


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
        MoveScript foundPlayer;

        if((foundPlayer = CheckPlayerColliding(path.GetPointsInSegment(0)[0])) != null)
        {
            playerProgress = 0f;
        }
        else if((foundPlayer = CheckPlayerColliding(path.GetPointsInSegment(path.NumSegments - 1)[3])) != null)
        {
            playerProgress = path.NumSegments;
        }
        else
        {
            seenPlayer = false;
        }

        if(foundPlayer != null && !seenPlayer)
        {
            player = foundPlayer;
            player.turnToCamera = false;
            Camera.main.GetComponentInParent<AxisFollowCam>().enabled = false;
            cameraOffSetInitial = Camera.main.transform.localPosition;

            seenPlayer = true;
        }
    }

    MoveScript CheckPlayerColliding(Vector3 point)
    {

        foreach (Collider collider in Physics.OverlapSphere(point, 2f))
        {
            if (collider.GetComponent<MoveScript>())
            {
                return collider.GetComponent<MoveScript>();
            }
        }
        return null;
    }

    void MovePlayer()
    {
        Vector3 startPos = player.transform.position;
        horizontal = Input.GetAxis("Horizontal");

        player.animator.applyRootMotion = false;

        if ((playerProgress >= path.NumSegments && horizontal < 0)
            || (playerProgress <= 0 && horizontal > 0))
        {
            player.turnToCamera = true;
            player.animator.applyRootMotion = true;
            player = null;
            Camera.main.GetComponentInParent<AxisFollowCam>().enabled = true;
            return;
        }
        player.animator.SetFloat("z", Mathf.Abs(horizontal) * player.moveSpeedPercent * 1.5f);
        int playerSegment = Mathf.CeilToInt(Mathf.Clamp(playerProgress, 0f, path.NumSegments));
        playerProgress = Mathf.Clamp(playerProgress - horizontal * player.moveSpeedPercent * Time.deltaTime, 0f, path.NumSegments);
        Vector3[] pointsInSegment = path.GetPointsInSegment(Mathf.Max(0, playerSegment - 1));
        
        Vector3 pos = CubicCurve
            (
                pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], playerProgress - (Mathf.Max(0, playerSegment - 1))
            );
        pos.y = player.transform.position.y;

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
        player.transform.position = pos;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            float overallProgress = playerProgress / path.NumSegments;
            float camProgress = overallProgress * camPath.path.NumSegments;
            int camSegment = Mathf.CeilToInt(Mathf.Clamp(camProgress, 0, path.NumSegments));

            Vector3[] points = camPath.path.GetPointsInSegment(Mathf.Max(0, camSegment - 1));
            Camera.main.transform.parent.position = Vector3.MoveTowards(Camera.main.transform.parent.position, CubicCurve(points[0], points[1], points[2], points[3], camProgress - (Mathf.Max(0, camSegment - 1))), 1);
            Camera.main.transform.LookAt(Camera.main.GetComponentInParent<AxisFollowCam>().lookAt);
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

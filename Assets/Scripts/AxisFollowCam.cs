using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisFollowCam : MonoBehaviour
{
    private Camera cam;
    public MoveScript player;
    public Transform lookAt;

    public float moveDistance = 0.1f;
    public float lerpValue;

    private bool rotating = false;
    private Vector3 playerPos;

    private Vector3 offset;

    private List<Vector3> previousFrameVelocities = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        offset = transform.position - cam.transform.position;
        transform.position = lookAt.transform.position;
    }

    private void Update()
    {
        cam.transform.LookAt(transform.position);
        playerPos = lookAt.transform.position;
        previousFrameVelocities.Add(player.GetComponent<Rigidbody>().velocity);
        while(previousFrameVelocities.Count > 20)
        {
            previousFrameVelocities.RemoveAt(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveCam();
        RotateCam();
    }

    void MoveCam()
    {
        Vector3 averageVelocity = new Vector3();
        foreach(Vector3 vec in previousFrameVelocities)
        {
            averageVelocity += vec;
        }
        if (previousFrameVelocities.Count > 0)
        {
            averageVelocity = averageVelocity / previousFrameVelocities.Count;
            Vector3 addVelocity = averageVelocity * 0.5f;
            addVelocity.y = 0;
            Vector3 newPos = playerPos + addVelocity;
            transform.position = Vector3.Lerp(transform.position, newPos, 5f * Time.deltaTime);
        }
    }

    void RotateCam()
    {
        if(Mathf.Abs(Input.GetAxis("AimHorizontal")) > 0.6f
            && !rotating)
        {
            StartCoroutine(RotateCam90(Input.GetAxis("AimHorizontal") > 0));
        }
    }

    IEnumerator RotateCam90(bool right)
    {
        rotating = true;
        Vector3 startRot = transform.rotation.eulerAngles;
        startRot.y = Mathf.Round(startRot.y / 90) * 90f;
        Quaternion destRot = Quaternion.Euler(startRot + (new Vector3(0, 90, 0) * (right ? 1 : -1)));

        while(Mathf.Abs(Quaternion.Angle(transform.rotation, destRot)) > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, destRot, Mathf.Clamp(10 * Time.deltaTime, 0, 1));
            yield return 0;
        }
        Vector3 vec = transform.eulerAngles;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        transform.rotation = Quaternion.Euler(vec);
        Debug.Log("Done");
        rotating = false;
    }
}

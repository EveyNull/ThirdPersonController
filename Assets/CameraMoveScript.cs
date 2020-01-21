using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveScript : MonoBehaviour
{
    public float cameraRotateSpeed = 10f;
    public Transform targetFollow;

    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = targetFollow.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetFollow.position - offset;

        transform.Translate(-Vector3.right * Input.GetAxis("AimHorizontal") * Time.deltaTime * cameraRotateSpeed);
        offset = targetFollow.position - transform.position;

        transform.LookAt(targetFollow);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private bool doorClosed = true;

    public DoorOpener door;
    public Transform doorLookAt;
    public GameObject buttonObject;

    public Transform playerStandAt;
    public Transform playerMoveTo;

    public Transform buttonCameraPos;
    public Transform doorCameraPos;

    public float pauseBeforeDoor = 2f;
    public float pauseAfterDoor = 2f;

    private bool wait = false;
    
    public void HitButton(MoveScript target)
    {
        target.animator.ResetTrigger("AttackTrigger");
        StartCoroutine(DoorOpenCutscene(target));
    }

    IEnumerator DoorOpenCutscene(MoveScript target)
    {
        target.allowMovement = false;
        target.animator.applyRootMotion = false;
        target.animator.SetFloat("z", 0f);

        Camera camera = Camera.main;

        camera.GetComponent<CameraMoveScript>().enabled = false;

        StartCoroutine(MoveCamera(camera, buttonCameraPos));

        wait = true;
        StartCoroutine(RotateObjectToLookAt(target.transform, playerStandAt));
        while(wait) yield return 0;

        wait = true;
        StartCoroutine(MovePlayerToPos(target, playerStandAt));
        while(wait) yield return 0;

        wait = true;
        StartCoroutine(RotateObjectToLookAt(target.transform, transform));
        while (wait) yield return 0;

        target.animator.SetTrigger("AttackTrigger");
        yield return new WaitForSeconds(0.2f);
        Vector3 insideWall = buttonObject.transform.localPosition + buttonObject.transform.forward;
        while (Vector3.Distance(buttonObject.transform.localPosition, insideWall) > 0.1f)
        {
            buttonObject.transform.localPosition = Vector3.MoveTowards(buttonObject.transform.localPosition, insideWall, Time.deltaTime);
            yield return 0;
        }

        StartCoroutine(MoveCamera(camera, doorCameraPos));

        wait = true;
        StartCoroutine(RotateObjectToLookAt(target.transform, playerMoveTo));
        while (wait) yield return 0;

        wait = true;
        StartCoroutine(MovePlayerToPos(target, playerMoveTo));
        while (wait)
        {
            Debug.Log(Vector3.Distance(target.transform.position, playerMoveTo.position));
            yield return 0;
        }

        StartCoroutine(RotateObjectToLookAt(target.transform, doorLookAt));

        door.StartDoorOpen();

        while(!door.complete)
        {
            yield return 0;
        }

        target.allowMovement = true;
        target.animator.applyRootMotion = true;
        camera.GetComponent<CameraMoveScript>().enabled = true;

        yield break;
    }

    IEnumerator MoveCamera(Camera camera, Transform target)
    {
        float startDistance = Vector3.Distance(camera.transform.position, target.position);
        Quaternion startRot = camera.transform.rotation;
        while (Vector3.Distance(camera.transform.position, target.position) > 0.2f)
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, target.transform.position, Time.deltaTime * 3);
            camera.transform.rotation = Quaternion.Lerp(startRot, target.transform.rotation, 1 - Vector3.Distance(camera.transform.position, target.position) / startDistance);
            yield return 0;
        }
    }

    IEnumerator RotateObjectToLookAt(Transform target, Transform lookAt)
    {
        Vector3 lookAtFlat = lookAt.position;
        lookAtFlat.y = 0f;
        Vector3 targetFlat = target.position;
        targetFlat.y = 0f;
        while (Vector3.Dot((lookAtFlat - targetFlat).normalized, target.transform.forward) < 0.99f)
        {
            targetFlat = target.position;
            targetFlat.y = 0f;
            Vector3 targetRotation = Vector3.RotateTowards(target.transform.forward, (lookAtFlat - targetFlat), Time.deltaTime * 2f, 0f);
            target.transform.rotation = Quaternion.LookRotation(targetRotation);
            yield return 0;
        }
        wait = false;
    }

    IEnumerator MovePlayerToPos(MoveScript player, Transform pos)
    {
        player.ForceMoveToLocation(pos.position);
        while(Vector3.Distance(player.transform.position, pos.position) > 0.1f) yield return 0;
        player.animator.SetFloat("z", 0f);
        wait = false;
    }
}

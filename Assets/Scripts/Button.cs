using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private bool doorClosed = true;

    public DoorOpener door;
    public Transform doorLookAt;
    public GameObject buttonObject;

    public Transform playerMoveTo;

    public Transform buttonCameraPos;
    public Transform doorCameraPos;

    public float pauseBeforeDoor = 2f;
    public float pauseAfterDoor = 2f;
    
    public void HitButton(MoveScript target)
    {
        StartCoroutine(DoorOpenCutscene(target));
    }

    IEnumerator DoorOpenCutscene(MoveScript target)
    {
        target.allowMovement = false;
        target.animator.applyRootMotion = false;
        target.animator.SetFloat("z", 0f);


        Vector3 insideWall = buttonObject.transform.localPosition + buttonObject.transform.forward;

        while (Vector3.Distance(buttonObject.transform.localPosition, insideWall) > 0.1f)
        {
            buttonObject.transform.localPosition = Vector3.MoveTowards(buttonObject.transform.localPosition, insideWall, Time.deltaTime);
            yield return 0;
        }

        while (Vector3.Dot((playerMoveTo.position - target.transform.position).normalized, target.transform.forward) < 0.999f)
        {
            Vector3 flatPlayerPos = target.transform.position;
            flatPlayerPos.y = 0f;
            Vector3 targetRotation = Vector3.RotateTowards(target.transform.forward, (playerMoveTo.position - flatPlayerPos), Time.deltaTime * 2f, 0f);
            target.transform.rotation = Quaternion.LookRotation(targetRotation);
            yield return 0;
        }

        target.ForceMoveToLocation(playerMoveTo.position);

        while (Vector3.Distance(target.transform.position, playerMoveTo.position) > 0.1f)
        {
            yield return 0;
        }

        target.animator.SetFloat("z", 0f);

        Vector3 doorFlatPos = doorLookAt.position;
        doorFlatPos.y = 0f;
        while (Vector3.Dot((doorFlatPos - target.transform.position).normalized, target.transform.forward) < 0.99f)
        {
            Vector3 flatPlayerPos = target.transform.position;
            flatPlayerPos.y = 0f;
            Vector3 targetRotation = Vector3.RotateTowards(target.transform.forward, (doorFlatPos - flatPlayerPos), Time.deltaTime * 2f, 0f);
            target.transform.rotation = Quaternion.LookRotation(targetRotation);
            yield return 0;
        }

        door.StartDoorOpen();

        while(!door.complete)
        {
            yield return 0;
        }

        target.allowMovement = true;
        target.animator.applyRootMotion = true;

        yield break;
    }
}

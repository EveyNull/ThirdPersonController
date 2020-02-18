using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpenButton : Button
{
    private bool doorClosed = true;

    public GameObject doorCollapse;

    public DoorOpener door;
    public Transform doorLookAt;
    public GameObject buttonObject;

    public Light buttonLight;
    public ParticleSystem particles;

    public Transform playerStandAt;
    public Transform playerMoveTo;

    public Transform buttonCameraPos;
    public Transform doorCameraPos;

    public float pauseBeforeDoor = 2f;
    public float pauseAfterDoor = 2f;

    private bool wait = false;

    private void Start()
    {

        RaycastHit hit;
        Physics.Raycast(playerStandAt.position + Vector3.up, Vector3.down, out hit);
        playerStandAt.position = hit.point;

        Physics.Raycast(playerMoveTo.position + Vector3.up, Vector3.down, out hit);
        playerMoveTo.position = hit.point;

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MoveScript>() && doorClosed)
        {
            interactImage.enabled = true;
        }
    }

    public override void HitButton(MoveScript target)
    {
        if (doorClosed)
        {
            target.animator.ResetTrigger("AttackTrigger");
            target.rigidbody.velocity = Vector3.zero;
            StartCoroutine(DoorOpenCutscene(target));
        }
    }

    IEnumerator DoorOpenCutscene(MoveScript target)
    {
        doorClosed = false;
        interactImage.enabled = false;

        target.animator.SetFloat("z", 0f);
        target.allowMovement = false;
        target.animator.applyRootMotion = false;

        Camera camera = Camera.main;

        camera.GetComponent<CameraMoveScript>().enabled = false;

        if (target.GetComponent<ButtonSeenCutscene>().GetSeenCutscene())
        {
            camera.transform.position = buttonCameraPos.position;
            camera.transform.rotation = buttonCameraPos.rotation;

            target.transform.position = playerStandAt.position;
            target.transform.rotation = playerStandAt.rotation;
        }
        else
        {
            StartCoroutine(MoveCamera(camera, buttonCameraPos));

            wait = true;
            StartCoroutine(RotateObjectToLookAt(target.transform, playerStandAt));
            while (wait) yield return 0;

            wait = true;
            StartCoroutine(MovePlayerToPos(target, playerStandAt));
            while (wait) yield return 0;

            wait = true;
            StartCoroutine(RotateObjectToLookAt(target.transform, transform));
            while (wait) yield return 0;
        }
        target.animator.SetTrigger("AttackTrigger");
        target.animator.speed = 0.5f;
        yield return new WaitForSeconds(0.6f);
        target.animator.speed = 1f;

        

        if (buttonLight)
        {
            buttonLight.color = Color.green;
        }
        if(particles)
        {
            particles.Play();
        }
        if(GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }

        Vector3 insideWall = buttonObject.transform.position + (transform.forward * 0.2f);
        Vector3 original = buttonObject.transform.position;

        while (Vector3.Distance(buttonObject.transform.position, insideWall) > 0.1f)
        {
            buttonObject.transform.position = Vector3.MoveTowards(buttonObject.transform.position, insideWall, Time.deltaTime * 4);
            yield return 0;
        }
        while (Vector3.Distance(buttonObject.transform.position, original) > 0.1f)
        {
            buttonObject.transform.position = Vector3.MoveTowards(buttonObject.transform.position, original, Time.deltaTime * 4);
            yield return 0;
        }

        if(doorCollapse)
        {
            doorCollapse.GetComponent<Rigidbody>().isKinematic = false;
            yield return new WaitForSeconds(3f);
        }


        yield return new WaitForSeconds(0.5f);
        if (target.GetComponent<ButtonSeenCutscene>().GetSeenCutscene())
        {
            camera.transform.position = doorCameraPos.position;
            camera.transform.rotation = doorCameraPos.rotation;

            door.StartDoorOpen();

            yield return new WaitForSeconds(1f);

        }
        else
        {
            StartCoroutine(MoveCamera(camera, doorCameraPos));

            wait = true;
            StartCoroutine(RotateObjectToLookAt(target.transform, playerMoveTo));
            while (wait) yield return 0;

            wait = true;
            StartCoroutine(MovePlayerToPos(target, playerMoveTo));
            while (wait)
            {
                yield return 0;
            }

            StartCoroutine(RotateObjectToLookAt(target.transform, doorLookAt));

            door.StartDoorOpen();

            while (!door.complete)
            {
                yield return 0;
            }
        }

        target.allowMovement = true;
        target.animator.applyRootMotion = true;
        camera.GetComponent<CameraMoveScript>().enabled = true;

        target.GetComponent<ButtonSeenCutscene>().SetSeenCutscene();

        yield break;
    }

    IEnumerator MoveCamera(Camera camera, Transform target)
    {
        float startDistance = Vector3.Distance(camera.transform.position, target.position);
        Quaternion startRot = camera.transform.rotation;
        float y = 0f;
        while (Vector3.Distance(camera.transform.position, target.position) > 0.2f)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, target.transform.position, y += 0.0005f);
            //camera.transform.position = Vector3.MoveTowards(camera.transform.position, target.transform.position, Time.deltaTime * 3);
            float x = 1 - Vector3.Distance(camera.transform.position, target.position) / startDistance;
            float lerpValue = 1 / (1 + Mathf.Exp((-12f * (x - 0.5f))));
            camera.transform.rotation = Quaternion.Slerp(startRot, target.transform.rotation, lerpValue);
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

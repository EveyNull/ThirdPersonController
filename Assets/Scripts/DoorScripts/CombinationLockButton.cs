using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLockButton : Button
{
    public Transform buttonCameraPos;
    public Transform playerStandAt;
    public GameObject buttonObject;

    public CombinationLock combinationLock;

    private bool buttonPressing;


    private void Start()
    {
        RaycastHit hit;
        Physics.Raycast(playerStandAt.position, Vector3.down, out hit);
        playerStandAt.position = hit.point;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MoveScript>() && !combinationLock.opened)
        {
            interactImage.enabled = true;
        }
    }


    public override void HitButton(MoveScript target)
    {
        if (!combinationLock.opened && !buttonPressing)
        {
            target.animator.ResetTrigger("AttackTrigger");
            buttonPressing = true;
            StartCoroutine(PressButton(target));
            target.rigidbody.velocity = Vector3.zero;
        }
    }

    IEnumerator PressButton(MoveScript target)
    {
        interactImage.enabled = false;
        Camera camera = Camera.main;
        target.animator.SetFloat("z", 0f);
        target.allowMovement = false;
        target.animator.applyRootMotion = false;
        camera.GetComponent<CameraMoveScript>().enabled = false;


        camera.transform.position = buttonCameraPos.position;
        camera.transform.rotation = buttonCameraPos.rotation;

        target.transform.position = playerStandAt.position;
        target.transform.rotation = playerStandAt.rotation;

        target.animator.SetTrigger("AttackTrigger");
        target.animator.speed = 0.5f;
        yield return new WaitForSeconds(0.6f);
        target.animator.speed = 1f;

        GetComponentInChildren<ParticleSystem>().Play();
        if (GetComponent<AudioSource>())
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
        while (Vector3.Distance(buttonObject.transform.position, original) > 0f)
        {
            buttonObject.transform.position = Vector3.MoveTowards(buttonObject.transform.position, original, Time.deltaTime * 4);
            yield return 0;
        }

        yield return new WaitForSeconds(0.5f);

        bool complete = combinationLock.PressedButton(this);

        if (complete)
        {
            camera.transform.position = combinationLock.doorLook.position;
            camera.transform.rotation = combinationLock.doorLook.rotation;
        }
        else
        {
            camera.transform.position = combinationLock.lightLook.position;
            camera.transform.rotation = combinationLock.lightLook.rotation;
        }

        yield return new WaitForSeconds(1f);


        target.allowMovement = true;
        target.animator.applyRootMotion = true;
        camera.GetComponent<CameraMoveScript>().enabled = true;

        interactImage.enabled = !complete;

        buttonPressing = false;
    }
}

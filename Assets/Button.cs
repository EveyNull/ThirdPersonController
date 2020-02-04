using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private bool doorClosed = true;

    public DoorOpenScript door;
    public GameObject buttonObject;

    public Transform buttonCameraPos;
    public Transform doorCameraPos;

    public float pauseBeforeDoor = 2f;
    public float pauseAfterDoor = 2f;

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("AttackL") && doorClosed)
        {
            doorClosed = false;
            StartCoroutine(DoorOpenCutscene(other.GetComponent<Rigidbody>()));
        }
    }

    IEnumerator DoorOpenCutscene(Rigidbody player)
    {
        Camera.main.GetComponent<CameraMoveScript>().enabled = false;
        Camera.main.transform.position = buttonCameraPos.position;
        Camera.main.transform.rotation = buttonCameraPos.rotation;

        player.GetComponent<MoveScript>().allowMovement = false;
        player.transform.LookAt(new Vector3(GetComponent<Collider>().transform.position.x, player.transform.position.y, GetComponent<Collider>().transform.position.z));

        if(player.transform.position != GetComponent<Collider>().bounds.center)
        {
            player.transform.position = new Vector3(GetComponent<Collider>().bounds.center.x, player.transform.position.y, GetComponent<Collider>().bounds.center.z);
        }

        player.GetComponent<Animator>().SetTrigger("AttackTrigger");

        Vector3 insideWall = new Vector3(buttonObject.transform.localPosition.x, buttonObject.transform.localPosition.y, buttonObject.transform.localPosition.z - 0.5f);

        while(Vector3.Distance(buttonObject.transform.localPosition, insideWall) > 0.1f)
        {
            buttonObject.transform.localPosition = Vector3.MoveTowards(buttonObject.transform.localPosition, insideWall, Time.deltaTime);
            yield return 0;
        }

        yield return new WaitForSeconds(pauseBeforeDoor);

        Camera.main.transform.position = doorCameraPos.position;
        Camera.main.transform.rotation = doorCameraPos.rotation;

        door.SetOpening(true);
        StartCoroutine(door.OpenDoor());
        while (door.GetOpening())
        {
            yield return 0;
        }

        yield return new WaitForSeconds(pauseAfterDoor);

        player.GetComponent<MoveScript>().allowMovement = true;

        GetComponent<AudioSource>().Play();

        Camera.main.GetComponent<CameraMoveScript>().enabled = true;

        yield break;
    }
}

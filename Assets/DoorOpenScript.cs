using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenScript : MonoBehaviour
{
    public Vector3 newPos;
    public float doorOpenSpeed;

    private bool opening = false;
    public IEnumerator OpenDoor()
    {
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
        while(Vector3.Distance(transform.localPosition, newPos) > 0.1f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, newPos, Time.deltaTime * doorOpenSpeed);
            yield return 0;
        }

        opening = false;
        GetComponent<AudioSource>().loop = false;
        yield break;
    }

    public bool GetOpening()
    {
        return opening;
    }

    public void SetOpening(bool n_opening)
    {
        opening = n_opening;
    }
}

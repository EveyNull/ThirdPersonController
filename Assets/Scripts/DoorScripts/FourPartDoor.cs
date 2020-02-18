using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPartDoor : DoorOpener
{
    private FourPartDoorOpen[] doorParts = new FourPartDoorOpen[4];


    public float doorSpeed = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < doorParts.Length; i++)
        {
            doorParts[i] = transform.GetChild(i).gameObject.AddComponent<FourPartDoorOpen>();
        }
    }

    private void Update()
    {
        if (!complete)
        {
            bool allDone = true;
            foreach (FourPartDoorOpen doorPart in doorParts)
            {
                if (!doorPart.complete)
                {
                    allDone = false;
                    break;
                }
            }
            if (allDone)
            {
                doorOpen.Stop();
                jingle.Play();
                complete = true;
            }
        }
    }

    public override void StartDoorOpen()
    {
        doorParts[0].StartOpening(0f, doorSpeed);
        doorParts[1].StartOpening(0.5f, doorSpeed);
        doorParts[2].StartOpening(1f, doorSpeed);
        doorParts[3].StartOpening(1.5f, doorSpeed);
        doorOpen.Play();
    }
}

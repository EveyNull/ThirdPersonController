using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPartDoor : DoorOpener
{
    private FourPartDoorOpen[] doorParts = new FourPartDoorOpen[4];

    public float doorSpeed = 0.75f;

    bool opened = false;

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
        bool allDone = true;
        foreach(FourPartDoorOpen doorPart in doorParts)
        {
            if(!doorPart.complete)
            {
                allDone = false;
                break;
            }
        }
        if(allDone)
        {
            complete = true;
        }
    }

    public override void StartDoorOpen()
    {
        opened = true;
        doorParts[0].StartOpening(0f, doorSpeed);
        doorParts[1].StartOpening(0.25f, doorSpeed);
        doorParts[2].StartOpening(0.5f, doorSpeed);
        doorParts[3].StartOpening(0.75f, doorSpeed);
    }
}

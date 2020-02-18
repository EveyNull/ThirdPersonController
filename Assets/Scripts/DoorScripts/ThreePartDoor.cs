using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreePartDoor : DoorOpener
{
    private ThreePartDoorOpen[] doorParts = new ThreePartDoorOpen[3];

    public float doorSpeed = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < doorParts.Length; i++)
        {
            doorParts[i] = transform.GetChild(i).gameObject.AddComponent<ThreePartDoorOpen>();
        }
    }

    private void Update()
    {
        if(!complete)
        {
            bool allDone = true;
            foreach (ThreePartDoorOpen doorPart in doorParts)
            {
                if (!doorPart.complete)
                {
                    allDone = false;
                    break;
                }
            }
            if (allDone)
            {
                complete = true;
                doorOpen.Stop();
                jingle.Play();
            }

        }
    }

    public override void StartDoorOpen()
    {
        doorParts[0].StartOpening(false, doorSpeed);
        doorParts[1].StartOpening(true, doorSpeed);
        doorParts[2].StartOpening(false, doorSpeed);
        doorOpen.Play();
    }
}

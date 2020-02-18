using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDoor : DoorOpener
{
    public float doorSpeed = 1f;

    public ParticleSystem sparkL;
    public ParticleSystem sparkR;

    private BrokenDoorOpen[] doorParts = new BrokenDoorOpen[2];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < doorParts.Length; i++)
        {
            doorParts[i] = transform.GetChild(i).gameObject.AddComponent<BrokenDoorOpen>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!complete)
        {
            bool allDone = true;
            foreach (BrokenDoorOpen doorPart in doorParts)
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
        doorOpen.Play();
        doorParts[0].StartOpening(doorSpeed, sparkL);
        doorParts[1].StartOpening(doorSpeed, sparkR);
    }
}

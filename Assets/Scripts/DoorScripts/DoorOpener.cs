using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorOpener : MonoBehaviour
{
    public AudioSource doorOpen;
    public AudioSource jingle;
    [HideInInspector]
    public bool complete = false;
    public abstract void StartDoorOpen();
}

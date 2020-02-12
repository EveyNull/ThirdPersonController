using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorOpener : MonoBehaviour
{
    [HideInInspector]
    public bool complete = false;
    public abstract void StartDoorOpen();
}

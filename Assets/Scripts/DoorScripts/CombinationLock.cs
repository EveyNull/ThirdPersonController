using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    public Light[] doorLights = new Light[4];

    public AudioSource correct;
    public AudioSource incorrect;

    public DoorOpener door;

    public Transform doorLook;
    public Transform lightLook;

    [SerializeField]
    private bool[] locks = new bool[4];

    public bool opened = false;

    public CombinationLockButton[] buttonOrder = new CombinationLockButton[4];

    private void Start()
    {
        for(int i = 0; i < locks.Length; i++)
        {
            locks[i] = false;
        }
    }

    public bool PressedButton(CombinationLockButton buttonPressed)
    {
        for(int i = 0; i < locks.Length; i++)
        {
            if(locks[i])
            {
                continue;
            }

            if(buttonOrder[i] == buttonPressed)
            {
                locks[i] = true;
                doorLights[i].color = Color.green;
                correct.Play();
                if(i == locks.Length-1)
                {
                    door.StartDoorOpen();
                    opened = true;
                    return true;
                }
            }
            else
            {
                incorrect.Play();
                ResetAllLocks();
            }
            break;
        }
        return false;
    }

    private void ResetAllLocks()
    {
        for (int i = 0; i < locks.Length; i++)
        {
            locks[i] = false;
            doorLights[i].color = Color.red;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Button : MonoBehaviour
{
    public Image interactImage;
    public abstract void HitButton(MoveScript target);

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MoveScript>())
        {
            interactImage.enabled = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<MoveScript>())
        {
            interactImage.enabled = false;
        }
    }
}

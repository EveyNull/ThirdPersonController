using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float duration = 10f;
    protected virtual void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

}

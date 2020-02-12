using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float duration = 10f;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MoveScript>())
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
    }
}

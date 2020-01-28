using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUp
{
    public float speedIncrease = 2f;

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MoveScript>())
        {
            other.GetComponent<MoveScript>().IncreaseSpeed(speedIncrease, duration);
        }
        base.OnTriggerEnter(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPowerUp : PowerUp
{
    public int numJumps = 2;
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MoveScript>())
        {
            other.GetComponent<MoveScript>().IncreaseJumpNum(numJumps, duration);
        }
        base.OnTriggerEnter(other);
    }
}

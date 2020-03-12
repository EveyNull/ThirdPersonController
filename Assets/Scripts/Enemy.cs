using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponentInParent<Player>() && !collision.collider.CompareTag("AttackHitBox"))
        {
            collision.collider.GetComponentInParent<Player>().LoseHealth(2f, collision.GetContact(0).normal);
        }
    }
}

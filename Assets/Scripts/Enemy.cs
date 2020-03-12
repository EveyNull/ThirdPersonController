using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponentInParent<Player>() && !collision.collider.CompareTag("AttackHitBox"))
        {
            collision.collider.GetComponentInParent<Player>().LoseHealth(2, transform.position);
        }
    }

    public override void LoseHealth(int damage, Vector3 collisionNormal)
    {
        base.LoseHealth(damage, collisionNormal);

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

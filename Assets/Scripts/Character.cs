using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected bool takeDamage = true;
    protected float health = 10f;

    public virtual void LoseHealth(float damage, Vector3 collisionNormal)
    {
        if (takeDamage)
        {
            health -= damage;
            StartCoroutine(KnockBack(collisionNormal));
        }
    }

    protected virtual IEnumerator KnockBack(Vector3 normal)
    {
        float timer = 1f;
        GetComponent<Rigidbody>().AddForce(-normal * 10f, ForceMode.Impulse);
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }
    }
}

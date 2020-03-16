using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public bool takeDamage = true;
    public int health = 10;
    public Color color;

    private void Start()
    {
    }

    public virtual void LoseHealth(int damage, Vector3 otherPos)
    {
        if (takeDamage)
        {
            takeDamage = false;
            health -= damage;
            StartCoroutine(DisplayDamage());
            StartCoroutine(KnockBack(otherPos));
        }
    }

    protected virtual IEnumerator KnockBack(Vector3 otherPos)
    {
        float timer = 1f;
        Vector3 offset = (transform.position - otherPos).normalized;
        offset.y = 0.2f;
        GetComponent<Rigidbody>().AddForce(offset * 10f, ForceMode.Impulse);
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }
        takeDamage = true;
    }

    protected IEnumerator DisplayDamage()
    {
        GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.1f);
        GetComponentInChildren<Renderer>().material.SetColor("_Color", color);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private bool attacking = false;

    public Collider[] hitBoxes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void LoseHealth(int damage, Vector3 normal)
    {
        base.LoseHealth(damage, normal);
        GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponentInParent<Enemy>() && attacking)
        {
            collision.collider.GetComponentInParent<Enemy>().LoseHealth(4, transform.position);
        }
    }

    protected override IEnumerator KnockBack(Vector3 otherPos)
    {
        GetComponent<MoveScript>().animator.applyRootMotion = false;
        GetComponent<MoveScript>().animator.SetFloat("z", 0f);
        GetComponent<MoveScript>().allowMovement = false;
        float timer = 1f;
        Vector3 offset = (transform.position - otherPos).normalized;
        offset.y = 0.2f;
        GetComponent<Rigidbody>().AddForce(offset * 10f, ForceMode.Impulse);
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }
        GetComponent<MoveScript>().animator.applyRootMotion = true;
        GetComponent<MoveScript>().allowMovement = true;

        takeDamage = true;
    }

    public void SetAttacking(bool n_attacking)
    {
        attacking = n_attacking;
        if(n_attacking)
        {
            StartCoroutine(Attack());
        }
        else
        {
            foreach (Collider collider in hitBoxes)
            {
                collider.enabled = false;
            }
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.3f);

        foreach (Collider collider in hitBoxes)
        {
            collider.enabled = true;
        }
    }
}

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
        Debug.Log(health);
    }

    public override void LoseHealth(float damage, Vector3 normal)
    {
        base.LoseHealth(damage, normal);
        GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponentInParent<Enemy>() && attacking)
        {
            collision.collider.GetComponentInParent<Enemy>().LoseHealth(4f, collision.GetContact(0).normal);
        }
    }

    protected override IEnumerator KnockBack(Vector3 normal)
    {
        float timer = 1f;
        GetComponent<MoveScript>().animator.applyRootMotion = false;
        GetComponent<MoveScript>().animator.SetFloat("z", 0f);
        GetComponent<MoveScript>().enabled = false;
        normal = -normal;
        normal.y = 0.2f;
        GetComponent<Rigidbody>().AddForce(normal * 5f, ForceMode.Impulse);
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return 0;
        }
        GetComponent<MoveScript>().animator.applyRootMotion = true;
        GetComponent<MoveScript>().enabled = true;
    }

    public void SetAttacking(bool n_attacking)
    {
        attacking = n_attacking;
        foreach(Collider collider in hitBoxes)
        {
            collider.enabled = n_attacking;
        }
    }
}

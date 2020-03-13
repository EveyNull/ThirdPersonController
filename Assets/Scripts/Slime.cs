using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    public GameObject slimePrefab;
    public int slimeActiveLayer;
    public int slimeInactiveLayer;
    private List<GameObject> subSlimes;
    public int slimeSize;
    public float countdownTakeDamage;
    private int startHealth;

    private void Start()
    {
        countdownTakeDamage = 1f;
        startHealth = health;
        GetComponentInChildren<Renderer>().material.SetColor("_Color", color);
    }

    public override void LoseHealth(int damage, Vector3 otherPos)
    {
        health -= damage;
        if (health <= 0)
        {
            if (slimeSize > 1)
            {
                GetComponent<Collider>().enabled = false;
                subSlimes = new List<GameObject>();
                for (int i = 0; i < 2; i++)
                {
                    if (subSlimes.Count < 2)
                    {
                        GameObject newGameObject = Instantiate(slimePrefab);
                        subSlimes.Add(newGameObject);
                        subSlimes[i].GetComponent<Slime>().slimeSize = slimeSize - 1;
                        Vector3 normalizedOffset = (otherPos - transform.position).normalized;
                        Vector3 perpOffset = new Vector3(normalizedOffset.z, normalizedOffset.y, normalizedOffset.x);

                        subSlimes[i].transform.position = transform.position + (i == 0 ? perpOffset : -perpOffset) * 3f;
                        subSlimes[i].GetComponent<Slime>().health = Mathf.FloorToInt(startHealth / 2);
                        subSlimes[i].transform.localScale /= 3 - (slimeSize - 1);
                        Vector3 random = new Vector3(0, Random.Range(normalizedOffset.y - 30, normalizedOffset.y + 30), 0);
                        subSlimes[i].transform.rotation = Quaternion.Euler(random);
                        subSlimes[i].GetComponent<Slime>().enabled = true;
                        subSlimes[i].GetComponent<Slime>().takeDamage = false;
                        subSlimes[i].GetComponent<Collider>().enabled = true;
                        Vector3 force = subSlimes[i].transform.forward;
                        force.y = 2f;
                        subSlimes[i].GetComponent<Rigidbody>().AddForce(force * 13f, ForceMode.Impulse);
                        subSlimes[i].layer = slimeInactiveLayer;
                    }
                }
            }
            Destroy(gameObject);
            return;
        }
        StartCoroutine(KnockBack(otherPos));
        takeDamage = false;
        StartCoroutine(DisplayDamage());
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if(!collision.collider.GetComponentInParent<Player>() && !collision.collider.CompareTag("AttackHitBox"))
        {
            gameObject.layer = slimeActiveLayer;
            takeDamage = true;
        }
        base.OnCollisionEnter(collision);
    }
}

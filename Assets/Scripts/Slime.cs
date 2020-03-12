using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    private List<GameObject> subSlimes;
    public int slimeSize;
    public float countdownTakeDamage;

    private void Start()
    {
        countdownTakeDamage = 1f;
    }

    private void Update()
    {
        if(countdownTakeDamage <= 0f)
        {
            if (!takeDamage)
            {
                takeDamage = true;
            }
        }
        else
        {
            countdownTakeDamage -= Time.deltaTime;
        }
    }
    public override void LoseHealth(int damage, Vector3 otherPos)
    {
        base.LoseHealth(damage, otherPos);

        if (health <= 0 && slimeSize > 1)
        {
            GetComponent<Collider>().enabled = false;
            subSlimes = new List<GameObject>();
            for (int i = 0; i < 2; i++)
            {
                if (subSlimes.Count < 2)
                {
                    subSlimes.Add(Instantiate(gameObject));
                    subSlimes[i].GetComponent<Slime>().slimeSize = slimeSize - 1;
                    Vector3 normalizedOffset = (otherPos - transform.position).normalized;
                    Vector3 perpOffset = new Vector3(normalizedOffset.z, normalizedOffset.y, normalizedOffset.x);

                    subSlimes[i].transform.position = transform.position + (i == 0 ? perpOffset : -perpOffset) * 3f;
                    subSlimes[i].GetComponent<Slime>().health = 4;
                    subSlimes[i].transform.localScale /= 3 - (slimeSize - 1);
                    subSlimes[i].GetComponent<Collider>().enabled = true;
                    subSlimes[i].GetComponent<Slime>().takeDamage = false;
                    subSlimes[i].GetComponent<Slime>().enabled = true;
                }
            }
        }
    }
}

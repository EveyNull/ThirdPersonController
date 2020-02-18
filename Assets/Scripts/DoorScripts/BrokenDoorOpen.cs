using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDoorOpen : MonoBehaviour
{
    private bool opening = false;
    private float doorSpeed;

    private float moved = 0f;

    public bool complete = false;

    private bool broken = false;

    private float breakAfter = 0.5f;
    private float breakTimer = 0f;

    private float repairAfter = 2.5f;
    private float repairTimer = 0f;

    private ParticleSystem sparks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool move = false;
        if (opening && !complete)
        {
            if(broken)
            {
                if (repairTimer <= repairAfter)
                {
                    repairTimer += Time.deltaTime;
                }
                else
                {
                    if (!transform.parent.GetComponent<BrokenDoor>().doorOpen.isPlaying)
                    {
                        transform.parent.GetComponent<BrokenDoor>().doorOpen.Play();
                    }
                    move = true;
                }
            }
            else if(breakTimer <= breakAfter)
            {
                breakTimer += Time.deltaTime;
                move = true;
            }
            else
            {
                broken = true;
                doorSpeed *= 4f;
                sparks.Play();
                transform.parent.GetComponent<BrokenDoor>().doorOpen.Pause();
                GetComponent<AudioSource>().Play();
            }

            if(move)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, transform.position + transform.right, Time.deltaTime * doorSpeed * Mathf.Pow((1 + moved / 2f), 2));
                if (moved < 2f)
                {
                    moved += Vector3.Distance(transform.position, newPos);
                    transform.position = newPos;
                }
                else
                {
                    complete = true;
                }
            }
        }
    }

    public void StartOpening(float speed, ParticleSystem particleSystem)
    {
        opening = true;
        sparks = particleSystem;
        doorSpeed = speed;
    }


}

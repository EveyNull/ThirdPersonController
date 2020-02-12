using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreePartDoorOpen : MonoBehaviour
{
    private bool opening = false;
    private bool pullForward;
    private float openDelay;

    private float doorSpeed = 1f;

    private float openTimer = 0f;
    private float movedForwards = 0f;
    private float movedApart = 0f;

    [HideInInspector]
    public bool complete = false;
    

    // Update is called once per frame
    void Update()
    {
        if (opening && ! complete)
        {
            if (openTimer >= openDelay)
            {
                Vector3 newPos = new Vector3();
                if (movedApart < (pullForward ? 2.666f : 1.333f))
                {
                    if (pullForward && movedForwards <= 0.1f)
                    {
                        newPos = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * doorSpeed);
                        movedForwards += Vector3.Distance(transform.position, newPos);

                    }
                    else
                    {
                        newPos = Vector3.MoveTowards(transform.position, transform.position + (transform.right * (pullForward ? 2 : 1)), Time.deltaTime * doorSpeed);
                        movedApart += Vector3.Distance(transform.position, newPos);
                    }
                    transform.position = newPos;
                }
                else
                {
                    complete = true;
                }
            }
            else
            {
                openTimer += Time.deltaTime;
            }
        }
    }

    public void StartOpening(bool n_pullForward, float speed)
    {
        opening = true;
        pullForward = n_pullForward;
        doorSpeed = speed;
        openDelay = pullForward ? 0 : 1.333f / speed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPartDoorOpen : MonoBehaviour
{
    private bool opening = false;
    private float openDelay;
    private float doorSpeed;

    private float openTimer = 0f;

    private float moved = 0f;

    public bool complete = false;

    // Update is called once per frame
    void Update()
    {
        if(opening && !complete)
        {
            if(openTimer >= openDelay)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, transform.position + transform.right, Time.deltaTime * doorSpeed * Mathf.Pow((1 + moved/2f),2));
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
            else
            {
                openTimer += Time.deltaTime;
            }
        }
    }

    public void StartOpening(float delay, float speed)
    {
        opening = true;
        openDelay = delay;
        doorSpeed = speed;
    }
}

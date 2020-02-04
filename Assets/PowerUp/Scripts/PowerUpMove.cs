using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpMove : MonoBehaviour
{

	public float rotationSpeed = 60.0f;
    public float bobSpeed = 0.01f;


    private Vector3 startPos;
    private Vector3 targetPos;
    private bool up = true;


    private void Awake()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    void Update ()
	{
		transform.Rotate(new Vector3(0f,1f,0f) * Time.deltaTime * this.rotationSpeed);
        if(Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            if(up)
            {
                targetPos = startPos;
            }
            else
            {
                targetPos = startPos + Vector3.up * 1f;
            }
            up = !up;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * bobSpeed);
    }


}

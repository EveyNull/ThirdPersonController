using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAutomatic : MovingPlatform
{
    public bool pauseOnChangeDir = true;

    private bool movingForwards = true;
    private bool moving = true;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (moving)
        {

            if (distance >= 1f && movingForwards)
            {
                currentPoint++;
                distance = 0f;
            }
            else if (distance <= 0f && !movingForwards)
            {
                currentPoint--;
                distance = 1f;
            }
            if ((currentPoint + 1 < path.anchorPoints.Count && movingForwards) || (currentPoint - 1 >= -1 && !movingForwards))
            {
                Vector3[] pointsInSegment = path.GetPointsInSegment(currentPoint);
                Vector3 nextPoint = CubicCurve(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], distance);

                Vector3 moveTo = GetComponent<Rigidbody>().position + (nextPoint - GetComponent<Rigidbody>().position).normalized * Time.fixedDeltaTime * platformMoveSpeed;

                GetComponent<Rigidbody>().MovePosition(moveTo);

                if (Vector3.Distance(GetComponent<Rigidbody>().position, nextPoint) <= 0.1f)
                {
                    distance += movingForwards ? 0.05f : -0.05f;
                }
            }
            else
            {
                movingForwards = !movingForwards;
                if (pauseOnChangeDir)
                {
                    moving = false;
                    StartCoroutine(PauseOnChangeDir());
                }
            }
        }
        base.FixedUpdate();
    }

    IEnumerator PauseOnChangeDir()
    {
        yield return new WaitForSeconds(3f);
        moving = true;
    }

}

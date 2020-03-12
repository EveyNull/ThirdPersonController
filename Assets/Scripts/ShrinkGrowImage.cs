using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShrinkGrowImage : MonoBehaviour
{
    public int growRate = 2;

    private void Update()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x + growRate, GetComponent<RectTransform>().sizeDelta.y + growRate);
        if (GetComponent<RectTransform>().sizeDelta.x >= 70 || GetComponent<RectTransform>().sizeDelta.x <= 40)
        {
            growRate *= -1;
        }
    }

}

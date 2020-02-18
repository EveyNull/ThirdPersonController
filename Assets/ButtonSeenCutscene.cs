using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSeenCutscene : MonoBehaviour
{
    private bool cutsceneSeen = false;

    public void SetSeenCutscene()
    {
        cutsceneSeen = true;
    }

    public bool GetSeenCutscene()
    {
        return cutsceneSeen;
    }
}

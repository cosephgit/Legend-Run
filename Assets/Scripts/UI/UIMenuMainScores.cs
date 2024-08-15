using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// UIMenuMainScores
// main menu scores display
// created 5/9/23
// last modified 5/9/23

public class UIMenuMainScores : MonoBehaviour
{
    [SerializeField] private UIOdometer distanceCounter;

    public void Initialise()
    {
        int distance = GameManager.instance.distanceBest;

        if (distance > 0 || GameManager.instance.GetFlag(GlobalVars.SAVEFLAGDISTANCE))
        {
            distanceCounter.gameObject.SetActive(true);
            distanceCounter.SetDistance(distance);
            GameManager.instance.SetFlag(GlobalVars.SAVEFLAGDISTANCE);
        }
        else
            distanceCounter.gameObject.SetActive(false);
    }

    public void UpdateScores()
    {
        int distance = GameManager.instance.distanceBest;

        if (distance > 0 || GameManager.instance.GetFlag(GlobalVars.SAVEFLAGDISTANCE))
        {
            distanceCounter.gameObject.SetActive(true);
            distanceCounter.SetDistance(distance);
            GameManager.instance.SetFlag(GlobalVars.SAVEFLAGDISTANCE);
        }
    }
}

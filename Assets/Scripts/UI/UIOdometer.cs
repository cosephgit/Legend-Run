using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

// UIOdometer
// manages the distance display
// created 1/9/23
// last modified 1/9/23

public class UIOdometer : UIBase
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [Header("Array sizes must match and be in descending distance order")]
    [SerializeField] private float[] pingDistances;
    [SerializeField] private float[] pingMagnitudes;
    float distanceOld;
    int pingCount;

    private void Awake()
    {
        distanceText.text = "0 m";
        pingCount = Mathf.Min(pingDistances.Length, pingMagnitudes.Length);
    }

    public void SetDistance(float distance)
    {
        // go through the defined ping distances to find how big of a ping we should use for the latest update (if any)
        for (int i = 0; i < pingCount; i++)
        {
            if (Mathf.FloorToInt(distance / pingDistances[i]) > Mathf.FloorToInt(distanceOld / pingDistances[i]))
            {
                float magnitude = Mathf.Log10(distance) * pingMagnitudes[i];
                UIPopManager.instance.ShowPops(transform.position, magnitude, Color.cyan);
                AddShake(magnitude);
                break;
            }
        }

        if (distance < 1000)
        {
            distanceText.text = distance.ToString("N0") + " m";
        }
        else if (distance < 1000000)
        {
            // you're not going to get here, but just in case
            distanceText.text = (Mathf.Floor(distance / 10f) / 100f).ToString("N2") + " km";
        }
        else
        {
            distanceText.text = (Mathf.Floor(distance / 10f) / 100f).ToString("E2") + " km";
        }
        distanceOld = distance;
    }
}

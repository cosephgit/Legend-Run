using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// shows the current coin count of the player
// created 22/8/23
// last modified 22/8/23

public class UICoinBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private float updateTime = 0.1f; // time between counter updates
    private int coinsCurrent;
    private int coinsDisplay;
    private float updateNext;
    
    private void DisplayCoins()
    {
        if (coinsDisplay > 999999)
            counter.text = coinsDisplay.ToString("E2");
        else
            counter.text = coinsDisplay.ToString("N0");
    }

    public void SetCoins(int amount, bool force = false)
    {
        coinsCurrent = amount;
        if (force) DisplayCoins();
    }

    private void Update()
    {
        if (coinsCurrent != coinsDisplay)
        {
            if (updateNext > 0)
            {
                updateNext -= Time.deltaTime;
            }
            else
            {
                int difference = coinsCurrent - coinsDisplay;
                bool loss = (difference < 0);
                int change = 1;

                if (loss) difference = -difference;

                Debug.Log("Difference actual " + difference + " log(difference)" + Mathf.Log10(difference));

                int exponent = Mathf.FloorToInt(Mathf.Log10(difference));
                int coefficient = Mathf.FloorToInt(difference / Mathf.Pow(10f, exponent));

                Debug.Log("coinsCurrent " + coinsCurrent + " coinsDisplay " + coinsDisplay + " exponent " + exponent + " coefficient " + coefficient);

                if (coefficient > 5)
                    change = 5;

                Debug.Log("change original " + change + " change adjusted " + Mathf.FloorToInt(change * Mathf.Pow(10, exponent)));

                change = Mathf.FloorToInt(change * Mathf.Pow(10, exponent));

                coinsDisplay += (loss ? -1 : 1) * change;

                DisplayCoins();

                updateNext = updateTime;
            }
        }
    }
}
